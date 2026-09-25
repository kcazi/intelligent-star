using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject player;

    private SpriteRenderer playerRenderer;
    private SpriteRenderer gunRenderer;

    public GameObject bullet;

    public bool canFire;
    public float timer;
    public float shotTime;

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

        float rotZ = MathF.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        //if mousePos is on left of player look left otherwise flip right
        if (mousePos.x < player.transform.position.x)
        {
            playerRenderer.flipX = true;
            gunRenderer.flipX = true;

            rotZ = rotZ - 180;
        } else
        {
            playerRenderer.flipX = false;
            gunRenderer.flipX = false;
    
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
            Debug.Log("Shooting");
            canFire = false;
            Instantiate(bullet, transform.position, Quaternion.identity);

        }

    }
}
