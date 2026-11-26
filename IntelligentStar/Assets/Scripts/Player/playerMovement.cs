using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [Header("Damage Feedback")]
    public float flashDuration = 0.15f;

    private SpriteRenderer playerSprite;
    private Color originalColor;

    private SpriteRenderer gunSprite;
    private Color originalGunColor;

    [Header("Knockback Settings")]
    public float knockbackForceX = 3f;
    public float knockbackForceY = 2f;
    public float knockbackDuration = 0.1f;

    private float knockbackTimer = 0f;

    [Header("Health Regeneration")]
    public int maxHealth = 5;
    public float regenDelay = 5f;
    public float regenInterval = 1f;

    private float lastDamageTime;
    private float nextRegenTime;

    [Header("Damage Immunity")]
    public float immunityDuration = 0f;

    private float nextAllowedDamageTime = 0f;

    void Start()
    {
        gravity = gravityBase; // Set active gravity to base on start

        // find obj rb
        rb = GetComponent<Rigidbody2D>();

        // find animator on this object/children
        if (playerAnim == null) 
            playerAnim = GetComponentInChildren<Animator>();

        playerSprite = GetComponent<SpriteRenderer>();
        originalColor = playerSprite.color;

        gunSprite = arms.GetComponent<SpriteRenderer>();

        if (gunSprite != null)
        {
            originalGunColor = gunSprite.color;
        }

        lastDamageTime = Time.time;
        nextRegenTime = Time.time + regenDelay;
    }

    void Update()
    {
        if (playerAnim.GetBool("isDead"))
            return;

        if (health <= 0)
        {
            playerDie();
        }

        RegenerateHealth();

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
            playerAnim.SetBool("isClimbing", false);
            gravity = gravityBase;
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

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            // Stop climbing
            isClimbing = false;
            playerAnim.SetBool("isClimbing", false);
            playerAnim.speed = 1f;

            // Restore normal gravity
            gravity = gravityBase;

            // Jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Update grounded state
            isGrounded = false;

            // Play jump animation
            playerAnim.SetTrigger("Jump");
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

        // Count down the knockback timer
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;

            // Preserve horizontal knockback velocity
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
        }
        else
        {
            // Normal movement
            rb.linearVelocity = new Vector2(direction * moveSpeed, verticalVelocity);
        }
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
        if (collision.gameObject.CompareTag("World"))
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
            gravity = gravityBase; // restore original base gravity
        }
    }

    // IMMUNITY FRAME STUFF SHOULD WORK, I WAS UNSURE HOW TO TEST IT IN GAME
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        // Don't take damage if already dead
        if (health <= 0 || playerAnim.GetBool("isDead"))
            return;

        // Ignore damage during immunity frames
        if (Time.time < nextAllowedDamageTime)
            return;

        // Apply damage
        health -= damage;

        // Start immunity period
        nextAllowedDamageTime = Time.time + immunityDuration;

        // Reset health regeneration timer
        lastDamageTime = Time.time;
        nextRegenTime = Time.time + regenDelay;

        // Flash player and gun red
        StopCoroutine(nameof(DamageFlash));
        StartCoroutine(DamageFlash());

        // Determine knockback direction
        float knockbackDirection = transform.position.x >= attackerPosition.x ? 1f : -1f;

        // Apply knockback
        rb.linearVelocity = new Vector2(knockbackDirection * knockbackForceX, knockbackForceY);

        // Temporarily disable normal horizontal movement
        knockbackTimer = knockbackDuration;
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        // Turn the player red
        playerSprite.color = Color.red;

        // Turn the gun red
        if (gunSprite != null)
        {
            gunSprite.color = Color.red;
        }

        // Wait briefly
        yield return new WaitForSeconds(flashDuration);

        // Restore original player color
        playerSprite.color = originalColor;

        // Restore original gun color
        if (gunSprite != null)
        {
            gunSprite.color = originalGunColor;
        }
    }

    void RegenerateHealth()
    {
        if (health <= 0 || health >= maxHealth)
            return;

        if (Time.time - lastDamageTime < regenDelay)
            return;

        if (Time.time >= nextRegenTime)
        {
            health = Mathf.Min(health + 1, maxHealth);
            nextRegenTime = Time.time + regenInterval;
        }
    }

    public IEnumerator speedUp(float duration)
    {
        float originalSpeed = moveSpeedBase;

        moveSpeedBase *= 2f;

        yield return new WaitForSeconds(duration);

        moveSpeedBase = originalSpeed;
    }
}
