using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Gun : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject player;

    private SpriteRenderer playerRenderer;
    private SpriteRenderer gunRenderer;

    public GameObject bullet;

    [Header("Bullet Shell Settings")]
    public GameObject shellPrefab;
    public Transform shellEjectPoint;

    public float shellEjectForce = 1f;
    public float shellUpwardForce = 1f;
    public float shellSpin = 300f;
    public float shellLifetime = 2f;

    public bool canFire;
    public float timer;
    public float shotTime;
    public float baseDamage = 1;

    private void Awake()
    {
        playerRenderer = player.GetComponent<SpriteRenderer>();
        gunRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Determines where the mouse is on screen
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector3 rotation = mousePos - transform.position;

        Vector3 pos = transform.position;

        float rotZ = MathF.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        //if mousePos is on left of player look left otherwise flip right
        if (mousePos.x < player.transform.position.x)
        {
            playerRenderer.flipX = true;
            gunRenderer.flipX = true;

            rotZ = rotZ - 180;
            pos.x = player.transform.position.x + .1f;
            transform.position = pos;
        } else
        {
            playerRenderer.flipX = false;
            gunRenderer.flipX = false;
            pos.x = player.transform.position.x + -.1f;
            transform.position = pos;
    
        }

        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if(timer > shotTime)
            {
                canFire = true;
                timer = 0;
            }
        }
        
        if (Mouse.current.leftButton.isPressed && canFire)
        {
            canFire = false;

            // Fire bullet
            Instantiate(bullet, transform.position, Quaternion.identity);

            // Eject bullet shell
            EjectShell();
        }

    }

    private void EjectShell()
    {
        if (shellPrefab == null || shellEjectPoint == null)
            return;

        GameObject shell = Instantiate(shellPrefab, shellEjectPoint.position, Quaternion.identity);

        Rigidbody2D shellRb = shell.GetComponent<Rigidbody2D>();

        if (shellRb != null)
        {
            // Determine which direction the player is facing
            float direction = playerRenderer.flipX ? 1f : -1f;

            // Add random variation to shell movement
            float randomHorizontal = UnityEngine.Random.Range(-1.5f, 1.5f);
            float randomVertical = UnityEngine.Random.Range(-1f, 1f);
            float randomSpin = UnityEngine.Random.Range(150f, 450f);

            // Launch shell backward and upward
            shellRb.linearVelocity = new Vector2(direction * shellEjectForce + randomHorizontal, shellUpwardForce + randomVertical);

            // Give each shell a slightly different spin
            shellRb.angularVelocity = direction * randomSpin;
        }

        Destroy(shell, shellLifetime);
    }

    public IEnumerator instaKill(float duration)
    {
        baseDamage += 10f;

        yield return new WaitForSeconds(duration);

        baseDamage -= 10f;
    }
}
