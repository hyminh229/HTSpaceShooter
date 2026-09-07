using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ProjectileBase : MonoBehaviour, IDestroyable
{
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Color blueColor = Color.blue;
    [SerializeField] protected Color redColor = Color.red;

    public ElementColor ColorType { get; private set; }
    public int Damage => damage;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    public void SetColor(ElementColor newColor)
    {
        ColorType = newColor;
        UpdateVisual();
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    protected virtual void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = ColorType == ElementColor.BLUE ? blueColor : redColor;
    }

    public virtual void DestroyObject()
    {
        Destroy(gameObject);
    }
}