using TMPro;
using UnityEngine;

public enum TooltipType
{
    Interactable, Ally, Neutral, Patient, Enemy
}

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public RectTransform CanvasRectTransform;
    public TMP_Text TooltipText;
    public RectTransform TooltipContainer;
    public RectTransform TooltipFrame;

    public Color InteractableColor;
    public Color AllyColor;
    public Color NormalColor;
    public Color PatientColor;
    public Color EnemyColor;

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

    public void SetTooltipText(string text, TooltipType type = TooltipType.Interactable)
    {
        if (text.Length == 0)
        {
            SetEnabled(false);
            return;
        }
        SetPosition();
        SetEnabled(true);
        int textLength = text.Length;
        if (type == TooltipType.Interactable)
        {
            TooltipText.text = "<u>> " + text + "</u>";
            textLength += 2;
            TooltipText.color = InteractableColor;
        }
        else
        {
            TooltipText.text = text;
            switch (type)
            {
                case TooltipType.Neutral: TooltipText.color = NormalColor; break;
                case TooltipType.Ally: TooltipText.color = AllyColor; break;
                case TooltipType.Patient: TooltipText.color = PatientColor; break;
                case TooltipType.Enemy: TooltipText.color = EnemyColor; break;
            }
        }
        
        
        Rect rect = TooltipFrame.rect;
        TooltipFrame.sizeDelta = new Vector2(15 + 13 * textLength, rect.height);
    }
}