using UnityEngine;

public class ShowCurrentFrame : MonoBehaviour
{
    private GUIStyle textStyle;

    void Start()
    {
        textStyle = new()
        {
            fontSize = 60
        };
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 100, 25), $"Frame {Time.renderedFrameCount.ToString("000000")}", textStyle);
    }
}
