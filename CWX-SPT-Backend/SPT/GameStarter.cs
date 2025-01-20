/* GameStarter.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 * waffle.lord
 * reider123
 */

using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using CWX_SPT_Launcher_Backend.CWX;
using CWX_SPT_Launcher_Backend.EFT;
using CWX_SPT_Launcher_Backend.SPT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SPT.Launcher.Helpers;

namespace SPT.Launcher
{
    public class GameStarter
    {
        private ILogger _logger;
        private readonly bool _showOnly;
        private readonly string _originalGamePath;
        private readonly string[] _excludeFromCleanup;
        private const string registryInstall = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";

        private const string registrySettings = @"Software\Battlestate Games\EscapeFromTarkov";
        public GameStarter(ILogger logger) //string gamePath = null, string originalGamePath = null, string[] excludeFromCleanup = null)
        {
            _logger = logger;
            // _originalGamePath = originalGamePath ??= DetectOriginalGamePath();
            // _excludeFromCleanup = excludeFromCleanup ?? LauncherSettingsProvider.Instance.ExcludeFromCleanup;
        }

        private static string DetectOriginalGamePath()
        {
            // We can't detect the installed path on non-Windows
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return null;

            var installLocation = Registry.LocalMachine.OpenSubKey(registryInstall, false)
                ?.GetValue("InstallLocation");
            var info = (installLocation is string key) ? new DirectoryInfo(key) : null;
            return info?.FullName;
        }
        
        private string SerializeSingleQuotes<T>(T data)
        {
            StringBuilder sb = new StringBuilder();
            
            using (StringWriter sw = new StringWriter(sb))
            using (JsonTextWriter jw = new JsonTextWriter(sw))
            {
                jw.QuoteChar = '\'';
                
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jw, data);
            }
            
            return sb.ToString();
        }

        public async Task<Result<GameStarter>> LaunchGame(Server server, ServerProfile account, string gamePath)
        {
            _logger.Information(">>> Launching Game");
            _logger.Information($">>> Account: {account.Username}");
            _logger.Information($">>> Server : {server.BackenUrl}");
            // setup directories
            if (IsInstalledInLive())
            {
                _logger.Error("[LaunchGame] Installed in Live :: YES");
                return Result<GameStarter>.FromError("Installed in live");
            }

            // Confirm core.dll version matches version server is running
            if (IsCoreDllVersionMismatched(gamePath))
            {
                _logger.Error("[LaunchGame] Core dll mismatch :: FAILED");
                return Result<GameStarter>.FromError("Core dll mismatch");
            }
            
            _logger.Information("[LaunchGame] Installed in Live :: NO");
            
            _logger.Information("[LaunchGame] Setup Game Files ...");
            SetupGameFiles(gamePath);

            if (!ValidationUtil.Validate())
            {
                _logger.Error("[LaunchGame] Game Validation   :: FAILED");
                return Result<GameStarter>.FromError("Game validation failed");
            }
            
            _logger.Information("[LaunchGame] Game Validation   :: OK");

            if (account.wipe)
            {
                _logger.Information("[LaunchGame] Wipe profile requested");
                RemoveProfileRegistryKeys(account.ProfileID);
                CleanTempFiles();
            }

            // check game path
            var clientExecutable = Path.Join(gamePath, "EscapeFromTarkov.exe");

            if (!File.Exists(clientExecutable))
            {
                _logger.Error("[LaunchGame] Valid Game Path   :: FAILED");
                var message = $"Could not find {clientExecutable}";
                _logger.Error(message);
                return Result<GameStarter>.FromError(message);
            }
            
            _logger.Information("[LaunchGame] Valid Game Path   :: OK");

            // apply patches
            ProgressReportingPatchRunner patchRunner = new ProgressReportingPatchRunner(gamePath);

            try
            {
                await _frontend.CompletePatchTask(patchRunner.PatchFiles());
            }
            catch (TaskCanceledException)
            {
                _logger.Error("[LaunchGame] Applying Patch    :: FAILED");
                return Result<GameStarter>.FromError("Patch failed to apply");
            }
            
            _logger.Information("[LaunchGame] Applying Patch    :: OK");
            
            //start game
            var args =
                $"-force-gfx-jobs native -token={account.ProfileID} -config={SerializeSingleQuotes(new ClientConfig(server.BackenUrl.AbsolutePath))}";

            if (_showOnly)
            {
                Console.WriteLine($"{clientExecutable} {args}");
                _logger.Information("[LaunchGame] NOOP :: show only");
            }
            else
            {
                var clientProcess = new ProcessStartInfo(clientExecutable)
                {
                    Arguments = args,
                    UseShellExecute = false,
                    WorkingDirectory = gamePath,
                };

                try
                {
                    Process.Start(clientProcess);
                    _logger.Information("[LaunchGame] Game process started");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An exception occured while starting the game");
                    return Result<GameStarter>.FromError(ex.Message);
                }
            }

            return Result<GameStarter>.FromSuccess("Game started");
        }

        bool IsInstalledInLive()
        {
            var isInstalledInLive = false;

            try
            {
                var files = new FileInfo[]
                {
                    // SPT files
                    new FileInfo(Path.Combine(_originalGamePath, @"SPT.Launcher.exe")),
                    new FileInfo(Path.Combine(_originalGamePath, @"SPT.Server.exe")),

                    // bepinex files
                    new FileInfo(Path.Combine(_originalGamePath, @"doorstep_config.ini")),
                    new FileInfo(Path.Combine(_originalGamePath, @"winhttp.dll")),

                    // licenses
                    new FileInfo(Path.Combine(_originalGamePath, @"LICENSE-BEPINEX.txt")),
                    new FileInfo(Path.Combine(_originalGamePath, @"LICENSE-ConfigurationManager.txt")),                                    
                    new FileInfo(Path.Combine(_originalGamePath, @"LICENSE-Launcher.txt")),
                    new FileInfo(Path.Combine(_originalGamePath, @"LICENSE-Modules.txt")),
                    new FileInfo(Path.Combine(_originalGamePath, @"LICENSE-Server.txt"))
                };
                var directories = new DirectoryInfo[]
                {
                    new DirectoryInfo(Path.Combine(_originalGamePath, @"SPT_Data")),
                    new DirectoryInfo(Path.Combine(_originalGamePath, @"BepInEx"))
                };

                foreach (var file in files)
                {
                    if (File.Exists(file.FullName))
                    {
                        File.Delete(file.FullName);
                        _logger.Warning($"File removed :: found in live dir: {file.FullName}");
                        isInstalledInLive = true;
                    }
                }

                foreach (var directory in directories)
                {
                    if (Directory.Exists(directory.FullName))
                    {
                        RemoveFilesRecurse(directory);
                        _logger.Warning($"Directory removed :: found in live dir: {directory.FullName}");
                        isInstalledInLive = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }

            return isInstalledInLive;
        }

        bool IsCoreDllVersionMismatched(string gamePath)
        {
            try
            {
                var serverVersion = new SPTVersion(ServerManager.GetVersion());

                var coreDllVersionInfo = FileVersionInfo.GetVersionInfo(Path.Join(gamePath, @"\BepinEx\plugins\spt", "spt-core.dll"));
                var dllVersion = new SPTVersion(coreDllVersionInfo.FileVersion);

                _logger.Information($"[LaunchGame] spt-core.dll version: {dllVersion}");

                // Edge case, running on locally built modules dlls, ignore check and return ok
                if (dllVersion.Major == 1) return false;

                // check 'X'.x.x
                if (serverVersion.Major != dllVersion.Major) return true;

                // check x.'X'.x
                if (serverVersion.Minor != dllVersion.Minor) return true;

                // check x.x.'X'
                if (serverVersion.Build != dllVersion.Build) return true;

                return false; // Versions match, hooray
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }

            return true;
        }

        void SetupGameFiles(string gamePath)
        {
            var files = new []
            {
                GetFileForCleanup("BattlEye", gamePath),
                GetFileForCleanup("Logs", gamePath),
                GetFileForCleanup("ConsistencyInfo", gamePath),
                GetFileForCleanup("EscapeFromTarkov_BE.exe", gamePath),
                GetFileForCleanup("Uninstall.exe", gamePath),
                GetFileForCleanup("UnityCrashHandler64.exe", gamePath),
                GetFileForCleanup("WinPixEventRuntime.dll", gamePath)
            };

            foreach (var file in files)
            {
                if (file == null)
                {
                    continue;
                }

                if (Directory.Exists(file))
                {
                    RemoveFilesRecurse(new DirectoryInfo(file));
                }

                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        private string GetFileForCleanup(string fileName, string gamePath)
        {
            if (_excludeFromCleanup.Contains(fileName))
            {
                _logger.Information($"Excluded {fileName} from file cleanup");
                return null;
            }
            
            return Path.Combine(gamePath, fileName);
        }

        /// <summary>
        /// Remove the SPT JSON-based registry keys associated with the given profile ID
        /// </summary>
		public void RemoveProfileRegistryKeys(string profileId)
        {
            var registryFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "user\\sptRegistry\\registry.json"));

            if (!registryFile.Exists)
            {
                return;
            }

            JObject registryData = JObject.Parse(File.ReadAllText(registryFile.FullName));

            // Find any property that has a key containing the profileId, and remove it
            var propsToRemove = registryData.Properties().Where(prop => prop.Name.Contains(profileId, StringComparison.CurrentCultureIgnoreCase)).ToList();
            propsToRemove.ForEach(prop => prop.Remove());

            File.WriteAllText(registryFile.FullName, registryData.ToString());
        }

        /// <summary>
        /// Clean the temp folder
        /// </summary>
        /// <returns>returns true if the temp folder was cleaned succefully or doesn't exist. returns false if something went wrong.</returns>
		public bool CleanTempFiles()
        {
            var rootdir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "user\\sptappdata"));

            if (!rootdir.Exists)
            {
                return true;
            }

            return RemoveFilesRecurse(rootdir);
        }

        bool RemoveFilesRecurse(DirectoryInfo basedir)
        {
            _logger.Information($"Recursive Removal: {basedir}");
            
            if (!basedir.Exists)
            {
                return true;
            }

            try
            {
                // remove subdirectories
                foreach (var dir in basedir.EnumerateDirectories())
                {
                    RemoveFilesRecurse(dir);
                }

                // remove files
                var files = basedir.GetFiles();

                foreach (var file in files)
                {
                    file.IsReadOnly = false;
                    file.Delete();
                }

                // remove directory
                basedir.Delete();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to remove files");
                return false;
            }

            return true;
        }
    }
}
