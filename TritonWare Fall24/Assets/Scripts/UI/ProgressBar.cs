using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public RectTransform Foreground;
    public SpriteRenderer Sprite;

    public void SetColor(Color color)
    {
        Sprite.color = color;
    }

    public void SetProgress(float progress)
    {
        Foreground.localScale = new Vector3(progress, 1f, 1f);
    }

}