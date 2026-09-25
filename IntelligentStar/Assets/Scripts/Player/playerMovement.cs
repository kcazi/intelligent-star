using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;

    public float jumpForce = 12f;
    public float gravity = 30f;
    public float fallGravity = 40f;

    // can change if jump feels awkward
    public float jumpCutMultiplier = 0.1f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        // find obj rb
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        // player isnt pressing a move key
        float direction = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            // multiplies movement vector and goes left
            direction = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            // multiplies movement vector and goes right
            direction = 1f;
        }

        // Start jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            // applies jumpforce value to obj y value
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // releasing Space early cuts the upward velocity
        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0f)
        {
            // if space is released multiply upward velocity by cutmultiplier
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if ((GetComponent<SpriteRenderer>().flipX && (direction == 1f))
            || (!GetComponent<SpriteRenderer>().flipX && (direction == -1f)))
        {
            moveSpeed = 2.5f;
        }
        else
        {
            moveSpeed = 5f;
        }

        // Horizontal movement
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    // gravity for player/creatures/objs
    void FixedUpdate()
    {
        float currentGravity = gravity;

        if (rb.linearVelocity.y < 0f)
        {
            // lowkey dont know if this is necessary I was just following a video that said this was a good idea
            currentGravity = fallGravity;
        }

        rb.linearVelocity += Vector2.down * currentGravity * Time.fixedDeltaTime;
    }

    // collision with ground, checks if ground is actually whats colliding (prevents jumping off wall collision)
    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Check that the surface is underneath the player
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // sets false after stops touching ground
        isGrounded = false;
    }
}
