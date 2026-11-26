using UnityEngine;

public class BasePickUp : MonoBehaviour
{
    public string powerName;
    public playerMovement player;
    public string powerTime;
    //TODO: add variable that tracks time limit of power up 
    //  (Nuke is different but could probably be killing enemies over a certain amount of time)

    //TODO: Function that occurs on collision
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
        Debug.Log(powerName + " has been picked up");
    }
}
