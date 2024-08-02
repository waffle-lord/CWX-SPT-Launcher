using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.FileProviders;

namespace CWX_SPT_Frontend;

public class EmbeddedBlazorWebView : BlazorWebView
{
    public bool UseEmbeddedResources { get; init; }
    public IFileProvider EmbeddedFilesProvider { get; set; }

    public override IFileProvider CreateFileProvider(string contentRootDir)
    {
        if (!UseEmbeddedResources)
        {
            return base.CreateFileProvider(contentRootDir);
        }

        EmbeddedFilesProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "Resources");
        return EmbeddedFilesProvider;
    }
}