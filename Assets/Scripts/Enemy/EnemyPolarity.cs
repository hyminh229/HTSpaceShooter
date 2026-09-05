using UnityEngine;

public class EnemyPolarity : MonoBehaviour
{
    [SerializeField] private ElementColor startingColor = ElementColor.RED;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color redColor = Color.red;

    public ElementColor CurrentColor { get; private set; }

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        CurrentColor = startingColor;

        UpdateVisual();
    }

    public void SetColor(ElementColor newColor)
    {
        CurrentColor = newColor;

        UpdateVisual();
    }

    public void SwitchColor()
    {
        if (CurrentColor == ElementColor.BLUE)
        {
            SetColor(ElementColor.RED);
        }
        else
        {
            SetColor(ElementColor.BLUE);
        }
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (CurrentColor == ElementColor.BLUE)
        {
            spriteRenderer.color = blueColor;
        }
        else
        {
            spriteRenderer.color = redColor;
        }
    }
}