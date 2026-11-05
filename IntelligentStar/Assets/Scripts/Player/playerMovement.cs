using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    public float moveSpeedBase = 5f;
    public float moveSpeed;

    public float jumpForce = 12f;
    // Renamed to keep a backup of your original gravity
    public float gravityBase = 30f;     
    // The active gravity variable
    private float gravity;              
    public float fallGravity = 40f;
    // Added a variable to tweak how fast you climb
    public float climbSpeed = 3f;       

    // can change if jump feels awkward
    public float jumpCutMultiplier = 0.1f;

    // health
    public int health = 5;

    private Rigidbody2D rb;
    private bool isGrounded;

    public Animator playerAnim;
    public GameObject arms;

    private bool canClimb;
    // Track if the player is actively climbing right now
    private bool isClimbing;

    void Start()
    {
        gravity = gravityBase; // Set active gravity to base on start

        // find obj rb
        rb = GetComponent<Rigidbody2D>();

        // find animator on this object/children
        if (playerAnim == null) 
            playerAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (playerAnim.GetBool("isDead"))
            return;

        if (health <= 0)
        {
            playerDie();
        }

        #region Ladder Climbing Setup
        // Check if the player wants to start climbing while near a ladder
        if (canClimb)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed)
            {
                isClimbing = true;
                playerAnim.SetBool("isClimbing", true);
                gravity = 0f; // Turn off normal gravity calculations
            }
        }
        else
        {
            isClimbing = false;
        }
        #endregion

        #region Movement
        float direction = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            direction = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            direction = 1f;
        }

        // Only allow regular jumping if we are NOT actively climbing
        if (Keyboard.current.spaceKey.isPressed && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if ((GetComponent<SpriteRenderer>().flipX && (direction == 1f))
            || (!GetComponent<SpriteRenderer>().flipX && (direction == -1f)))
        {
            moveSpeed = moveSpeedBase * 0.75f;
        }
        else
        {
            moveSpeed = moveSpeedBase;
        }

        // Handle Horizontal and Vertical movement together
        float verticalVelocity = rb.linearVelocity.y;

        if (isClimbing)
        {
            // Calculate precise vertical movement based on holding keys
            float verticalDirection = 0f;
            if (Keyboard.current.wKey.isPressed) verticalDirection = 1f;
            if (Keyboard.current.sKey.isPressed) verticalDirection = -1f;

            verticalVelocity = verticalDirection * climbSpeed;
            
            // Optional: If you want to pause animations when the player stops moving on the ladder
            playerAnim.speed = (verticalDirection != 0f) ? 1f : 0f; 
        }
        else
        {
            // Reset animator playback speed back to normal when not climbing
            playerAnim.speed = 1f; 
        }

        // Apply velocities back to the Rigidbody
        rb.linearVelocity = new Vector2(direction * moveSpeed, verticalVelocity);
        #endregion

        // update animations
        playerAnim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        playerAnim.SetBool("isGrounded", isGrounded);
    }

    // gravity for player/creatures/objs
    void FixedUpdate()
    {
        // Completely skip custom gravity calculation if climbing
        if (isClimbing)
        {
            return;
        }

        float currentGravity = gravity;

        if (rb.linearVelocity.y < 0f)
        {
            currentGravity = fallGravity;
        }

        rb.linearVelocity += Vector2.down * currentGravity * Time.fixedDeltaTime;
    }

    // collision with ground, checks if ground is actually whats colliding
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("World"))
            return;
        
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("World") && Keyboard.current.spaceKey.isPressed)
        {
            isGrounded = false;
        }
    }

    void playerDie()
    {
        gameObject.tag = "DeadPlayer";
        arms.SetActive(false);
        playerAnim.SetBool("isDead", true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            canClimb = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            playerAnim.SetBool("isClimbing", false);
            playerAnim.speed = 1f; // Restore animator speed
            canClimb = false;
            isClimbing = false;
            gravity = gravityBase; // Safely restore original base gravity
        }
    }
}
