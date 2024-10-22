using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public RectTransform CanvasRectTransform;
    public TMP_Text TooltipText;
    public RectTransform TooltipContainer;
    public RectTransform TooltipFrame;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        SetPosition();
    }

    public void SetEnabled(bool enabled)
    {
        TooltipContainer.gameObject.SetActive(enabled);
    }

    public void SetPosition()
    {
        // Get the current mouse position in screen space
        Vector3 screenPosition = Input.mousePosition;

        // Convert the screen position to local point in the Canvas RectTransform
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CanvasRectTransform,       // Parent RectTransform (the Canvas)
                screenPosition,            // The screen position (Input.mousePosition)
                Camera.main,                      // Camera reference, null for Screen Space - Overlay
                out localPoint))           // Output parameter for local coordinates
        {
            // Now localPoint is in local coordinates relative to the canvas
            TooltipContainer.anchoredPosition = localPoint;
        }
    }

    public void SetTooltipText(string text)
    {
        SetPosition();
        SetEnabled(true);
        TooltipText.text = "> " + text;
        int textLength = text.Length + 2;
        Rect rect = TooltipFrame.rect;
        TooltipFrame.sizeDelta = new Vector2(20 + 13 * textLength, rect.height);
    }
}