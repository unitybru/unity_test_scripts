using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This behaviour exports images of your project to disk when a key is pressed.
/// </summary>
public class ImageExporter : MonoBehaviour
{
    public enum ImageExportMethod
    {
        Screen,
        GameView,
        GameViewRenderTexture,
    };

    public KeyCode recordKey = KeyCode.Space;
    public ImageExportMethod method = ImageExportMethod.GameViewRenderTexture;
    private RenderTexture captureTexture;

    void Start()
    {
        CreateRenderTexture();
    }

    void CreateRenderTexture()
    {
        captureTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32)
        {
            wrapMode = TextureWrapMode.Repeat
        };
        captureTexture.Create();
        captureTexture.name = "ImageExporter_captureTexture";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(recordKey))
        {
            StartCoroutine(RecordAFrame());
        }
    }

    private FileInfo GetCurrentFileName()
    {
        var paddedFrameString = Time.frameCount.ToString().PadLeft(8, '0');
        var currentFile = new FileInfo($"test_{paddedFrameString}.png");
        return currentFile;
    }

    private void OutputTextureToDisk(Texture2D tex2d)
    {
        var currentFile = GetCurrentFileName();
        var imgBytes = tex2d.EncodeToPNG();
        File.WriteAllBytes(currentFile.FullName, imgBytes);
        Debug.Log($"Wrote frame #{Time.frameCount} to disk as {currentFile.FullName}");
    }

    IEnumerator RecordAFrame()
    {
        // Wait until after the frame has been prepared
        yield return new WaitForEndOfFrame();

        Texture2D tex2d = null;
        switch (method)
        {
            case ImageExportMethod.GameView:
                tex2d = ScreenCapture.CaptureScreenshotAsTexture();
                OutputTextureToDisk(tex2d);
                break;
            case ImageExportMethod.Screen:
                tex2d = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
                tex2d.ReadPixels(new Rect(0,0, Screen.width, Screen.height), 0 ,0, false);
                tex2d.Apply();
                OutputTextureToDisk(tex2d);
                break;
            case ImageExportMethod.GameViewRenderTexture:
                ScreenCapture.CaptureScreenshotIntoRenderTexture(captureTexture);
                AsyncGPUReadback.Request(captureTexture, 0, TextureFormat.RGBA32, ReadbackCompleted);
                break;
            default:
                throw new InvalidEnumArgumentException($"Unexpected image export method '{method}'");
        }
    }

    private void ReadbackCompleted(AsyncGPUReadbackRequest obj)
    {
        // Render texture no longer needed, it has been read back.
        DestroyImmediate(captureTexture);

        using (var imgBytes = obj.GetData<byte>())
        {
            var tex2d = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            tex2d.LoadRawTextureData(imgBytes);
            OutputTextureToDisk(tex2d);
        }

        CreateRenderTexture();
    }
}
