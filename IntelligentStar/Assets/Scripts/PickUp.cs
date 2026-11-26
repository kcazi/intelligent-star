using UnityEngine;
using System;

public class PickUp : MonoBehaviour
{
    public string powerName;
    public playerMovement player;
    public string powerTime;

    public enum powers
    {
        SPEEDUP,
        HEALTHUP,
        NUKE,
        INSTAKILL
    }
    
    public powers P;
    public Sprite[] powerUpSprites;
    public SpriteRenderer spriteRenderer;

    public WaveManager WM;

    void Start()
    {
        WM = FindFirstObjectByType<WaveManager>();
        
        Array powerArray = Enum.GetValues(typeof(powers));

        int i = UnityEngine.Random.Range(0, powerArray.Length);

        if(i == 2)
        {
            i = UnityEngine.Random.Range(0, powerArray.Length);
        }

        P = (powers)powerArray.GetValue(i);

        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = powerUpSprites[i];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<playerMovement>();

            OnPickUp();
        }
    }

    public virtual void OnPickUp()
    {
        switch (P)
        {
            case(powers.SPEEDUP):
                powerName = "Speed Up";

                player.StartCoroutine(player.speedUp(3f));
            break;
            case(powers.HEALTHUP):
                powerName = "Max Health Up";

                player.StartCoroutine(player.maxHealthUp(10f));
            break;
            case(powers.NUKE):
                powerName = "Nuke";

                WM.StartCoroutine(WM.killAllZombies());
            break;
            case(powers.INSTAKILL):
                powerName = "Instant Kill";

                player.arms.GetComponent<Gun>().StartCoroutine(player.arms.GetComponent<Gun>().instaKill(3f));
            break;
        }

        Debug.Log(powerName + " has been picked up");

        Destroy(gameObject);
    }
}
