using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// This behaviour exports images of your project to disk when a key is pressed.
/// </summary>
public class ImageExporter : MonoBehaviour
{
    public KeyCode recordKey = KeyCode.Space;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(recordKey))
        {
            StartCoroutine(RecordAFrame());
        }
    }

    IEnumerator RecordAFrame()
    {
        // Wait until after the frame has been prepared
        yield return new WaitForEndOfFrame();

        var paddedFrameString = Time.frameCount.ToString().PadLeft(4, '0');
        var currentFile = new FileInfo($"test_{paddedFrameString}.png");

        // Add video frame
        Texture2D texture = new Texture2D(Screen.width , Screen.height , TextureFormat.RGBA32 , false);
        texture.ReadPixels(new Rect(0,0,Screen.width, Screen.height), 0 ,0, false);
        texture.Apply();
        var imgBytes = texture.EncodeToPNG();
        File.WriteAllBytes(currentFile.FullName, imgBytes);
        Debug.Log($"Wrote frame #{Time.frameCount} to disk as {currentFile.FullName}");

    }
}
