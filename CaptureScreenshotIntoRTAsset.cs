using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This behaviour copies a screenshot into a local RenderTexture asset.
/// </summary>
public class CaptureScreenshotIntoRTAsset : MonoBehaviour
{
    public RenderTexture rt;

    // Update is called once per frame
    void Update()
    {
        ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);
    }
}
