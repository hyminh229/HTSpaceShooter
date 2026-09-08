using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ChromaPolarityBase : MonoBehaviour
{
    [Header("Chroma Polarity")]
    [SerializeField] protected ElementColor startingColor = ElementColor.BLUE;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Color blueColor = Color.blue;
    [SerializeField] protected Color redColor = Color.red;

    public ElementColor CurrentColor { get; protected set; }
    public float LastSwitchTime { get; protected set; } = -Mathf.Infinity;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        CurrentColor = startingColor;
        UpdateVisual();
    }

    public virtual void SetColor(ElementColor newColor)
    {
        CurrentColor = newColor;
        LastSwitchTime = Time.time;
        UpdateVisual();
    }

    public void SwitchColor()
    {
        SetColor(CurrentColor == ElementColor.BLUE ? ElementColor.RED : ElementColor.BLUE);
    }

    protected virtual void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = CurrentColor == ElementColor.BLUE ? blueColor : redColor;
    }
}