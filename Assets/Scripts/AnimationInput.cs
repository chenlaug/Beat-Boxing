using UnityEngine;

public class AnimationInput : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private float holdTime = 0f;
    public float holdThreshold = 0.01f;

    private bool isHolding = false;

    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite dodgeSprite;

    private Vector3 initialPosition;
    public float attackOffset = 0.5f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
        initialPosition = transform.position;
    }

    void Update()
    {
        // Key press begins
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHolding = true;
            holdTime = 0f;
        }

        // Holding the key
        if (isHolding && Input.GetKey(KeyCode.Space))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= holdThreshold)
            {
                spriteRenderer.sprite = dodgeSprite;
                transform.position = initialPosition;
            }
        }

        // Key released
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (holdTime < holdThreshold)
            {
                spriteRenderer.sprite = attackSprite;
                transform.position = initialPosition + new Vector3(attackOffset, 0, 0);
            }
            else
            {
                spriteRenderer.sprite = dodgeSprite;
                transform.position = initialPosition;
            }

            // Reset
            isHolding = false;
            Invoke(nameof(ResetToIdle), 0.15f); // Petite pause avant de revenir à l'idle
        }
    }

    private void ResetToIdle()
    {
        spriteRenderer.sprite = idleSprite;
        transform.position = initialPosition;
    }
}