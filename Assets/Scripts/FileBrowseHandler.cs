using System;
using SFB;

public class FileBrowseHandler {
    public enum MediaType {
        image, video
    }

    public delegate void MediaPickCallback(string path);
    
    public static void OpenFileBrowser(MediaType mediaType, MediaPickCallback callback) { 
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        var extensions = GetExtensions(mediaType);
        var result = StandaloneFileBrowser.OpenFilePanel("Select " + mediaType, "", extensions, false);
        var path = result.Length == 0 ? null : result[0];
        callback(path);
        return;
#elif UNITY_IOS || UNITY_ANDROID
        switch (mediaType) {
            case MediaType.image:
                NativeGallery.GetImageFromGallery((resultPath) => { callback(resultPath); });
                break;
            case MediaType.video:
                NativeGallery.GetVideoFromGallery((resultPath) => { callback(resultPath); });
                break;
        }
#endif
    }

    private static ExtensionFilter[] GetExtensions(MediaType type) {
        switch (type) {
            case MediaType.image:
                return new [] {
                    new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
                };
            case MediaType.video:
                return new [] {
                    new ExtensionFilter("Video Files", "mp4")
                };
            default:
                return new ExtensionFilter[]{};
        }
    }
    
}
