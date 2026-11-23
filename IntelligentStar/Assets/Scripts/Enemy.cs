using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float HP;
    public float speed;

    public Animator ZombieAnim;

    [Header("Attack Settings")]
    public GameObject attackHitbox;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;
    private bool playerInRange = false;
    private bool isAttacking = false;

    public bool isDead = false;

    void Start()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (playerInRange && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Attack()
    {
        if (isDead || isAttacking) return;

        isAttacking = true;

        nextAttackTime = Time.time + attackCooldown;

        ZombieAnim.SetTrigger("isAttack");
    }

    // Called by Animation Event
    public void EnableAttackHitbox()
    {
        if (isDead) return;

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }
    }

    // Called by Animation Event
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    // Called at the end of attack animation
    public void EndAttack()
    {
        DisableAttackHitbox();

        isAttacking = false;
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        HP -= dmg;

        if (HP <= 0)
        {
            DespawnEnemy();
        }
    }

    public void DespawnEnemy()
    {
        if (isDead) return;

        isDead = true;
        playerInRange = false;
        isAttacking = false;

        DisableAttackHitbox();

        ZombieAnim.ResetTrigger("isAttack");
        ZombieAnim.SetBool("isDead", true);

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        Destroy(gameObject, 2f);
    }

}