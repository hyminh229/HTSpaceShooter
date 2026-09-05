using UnityEngine;

public class PlayerColorController : MonoBehaviour
{
    [SerializeField] private ElementColor startingColor = ElementColor.BLUE;
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

    private void Update()
    {
        HandleColorSwitch();
    }

    private void HandleColorSwitch()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
        {
            SwitchColor();
        }
    }

    public void SwitchColor()
    {
        if (CurrentColor == ElementColor.BLUE)
        {
            CurrentColor = ElementColor.RED;
        }
        else
        {
            CurrentColor = ElementColor.BLUE;
        }

        UpdateVisual();

        Debug.Log("Player switched color to: " + CurrentColor);
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