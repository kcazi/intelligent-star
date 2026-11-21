using UnityEngine;

public class ZombieAttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private bool hasHitPlayer = false;

    private void OnEnable()
    {
        // Reset whenever a new swipe starts
        hasHitPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Swipe collided with: " + other.gameObject.name);
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Swipe overlapping: " + other.gameObject.name);
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (hasHitPlayer) return;

        playerMovement player = other.GetComponentInParent<playerMovement>();

        if (player == null) return;

        if (!player.CompareTag("Player")) return;

        hasHitPlayer = true;

        player.TakeDamage(damage, transform.root.position);

        Debug.Log("Zombie swipe hit player!");
    }
}