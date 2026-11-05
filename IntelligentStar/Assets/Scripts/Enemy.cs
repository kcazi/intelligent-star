using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float HP;
    public Animator ZombieAnim;

    private bool playerInRange = false;
    private bool isDead = false;

    private playerMovement playerScript;

    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<playerMovement>();
    }

    void Update()
    {
        if (isDead) return;

        if (playerInRange)
        {
            Attack();

        }
    }

    // Detect player entering attack range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Detect player leaving attack range
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            ZombieAnim.ResetTrigger("isAttack");
        }
    }

    void Attack()
    {
        
        if (isDead) return;

        // Don't attack if already playing attack animation
        if (ZombieAnim.GetCurrentAnimatorStateInfo(0).IsName("ZombieAttack"))
            return;

        ZombieAnim.SetTrigger("isAttack");

        playerScript.health -= 1;
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

        ZombieAnim.SetBool("isDead", true);

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        Destroy(gameObject, 2f);
    }
}