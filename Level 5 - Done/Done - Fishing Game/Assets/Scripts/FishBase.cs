using UnityEngine;

public abstract class FishBase : MonoBehaviour
{
    [Header("Base Fish Properties")]
    public float fishSize = 1f;
    public float movementSpeed = 5f;
    
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void FlipSprite(float directionX)
    {
        if (spriteRenderer == null) return;
        
        if (directionX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}
