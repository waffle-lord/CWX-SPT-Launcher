/* License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 * Basuro
 */

using System.Security.Cryptography;
using CWX_SPT_Launcher_Backend.Patcher;
using CWX_SPT_Launcher_Backend.Patcher.Enums;

namespace CWX_SPT_Frontend_wf.Helpers;

public static class PatchHelper
{
    private static DiffResult Diff(byte[] original, byte[] patched)
    {
        PatchInfo pi = new PatchInfo
        {
            OriginalLength = original.Length,
            PatchedLength = patched.Length,
            OriginalChecksum = SHA256.HashData(original),
            PatchedChecksum = SHA256.HashData(patched)
        };

        if ((pi.OriginalLength == pi.PatchedLength) && ArraysMatch(pi.OriginalChecksum, pi.PatchedChecksum))
        {
            return new DiffResult(EDiffResultType.FilesMatch, null!);
        }

        int minLength = Math.Min(pi.OriginalLength, pi.PatchedLength);

        List<PatchItem> items = [];
        List<byte> currentData = null;
        int diffOffsetStart = 0;

        for (int i = 0; i < minLength; i++)
        {
            if (original[i] != patched[i])
            {
                if (currentData == null)
                {
                    diffOffsetStart = i;
                    currentData = [];
                }

                currentData.Add(patched[i]);
            }
            else
            {
                if (currentData != null)
                {
                    items.Add(new PatchItem { Offset = diffOffsetStart, Data = currentData.ToArray() });
                }

                currentData = null;
                diffOffsetStart = 0;
            }
        }

        if (currentData != null)
        {
            items.Add(new PatchItem { Offset = diffOffsetStart, Data = currentData.ToArray() });
        }

        if (pi.PatchedLength > pi.OriginalLength)
        {
            byte[] buf = new byte[pi.PatchedLength - pi.OriginalLength];
            Array.Copy(patched, pi.OriginalLength, buf, 0, buf.Length);
            items.Add(new PatchItem { Offset = pi.OriginalLength, Data = buf });
        }

        pi.Items = items.ToArray();

        return new DiffResult(EDiffResultType.Success, pi);
    }

    public static DiffResult Diff(string originalFile, string patchedFile)
    {
        if (string.IsNullOrWhiteSpace(originalFile))
        {
            return new DiffResult(EDiffResultType.OriginalFilePathInvalid, null!);
        }

        if (string.IsNullOrWhiteSpace(patchedFile))
        {
            return new DiffResult(EDiffResultType.PatchedFilePathInvalid, null!);
        }

        if (!File.Exists(originalFile))
        {
            return new DiffResult(EDiffResultType.OriginalFileNotFound, null!);
        }

        if (!File.Exists(patchedFile))
        {
            return new DiffResult(EDiffResultType.PatchedFileNotFound, null!);
        }

        byte[] originalData;

        try
        {
            originalData = File.ReadAllBytes(originalFile);
        }
        catch
        {
            return new DiffResult(EDiffResultType.OriginalFileReadFailed, null!);
        }
        
        byte[] patchedData;

        try
        {
            patchedData = File.ReadAllBytes(patchedFile);
        }
        catch
        {
            return new DiffResult(EDiffResultType.PatchedFileReadFailed, null!);
        }

        return Diff(originalData, patchedData);
    }

    public static PatchResult Patch(byte[] input, PatchInfo pi)
    {
        var inputHash = SHA256.HashData(input);

        if (ArraysMatch(inputHash, pi.PatchedChecksum))
        {
            return new PatchResult(EPatchResultType.AlreadyPatched, null!);
        }

        if (!ArraysMatch(inputHash, pi.OriginalChecksum))
        {
            return new PatchResult(EPatchResultType.InputChecksumMismatch, null!);
        }

        if (input.Length != pi.OriginalLength)
        {
            return new PatchResult(EPatchResultType.InputLengthMismatch, null!);
        }

        byte[] patchedData = new byte[pi.PatchedLength];
        long minLen = Math.Min(pi.OriginalLength, pi.PatchedLength);
        Array.Copy(input, patchedData, minLen);

        foreach (PatchItem itm in pi.Items)
        {
            Array.Copy(itm.Data, 0, patchedData, itm.Offset, itm.Data.Length);
        }

        var patchedHash = SHA256.HashData(patchedData);

        return !ArraysMatch(patchedHash, pi.PatchedChecksum) ? new PatchResult(EPatchResultType.OutputChecksumMismatch, null!) : new PatchResult(EPatchResultType.Success, patchedData);
    }

    private static bool ArraysMatch(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }
}