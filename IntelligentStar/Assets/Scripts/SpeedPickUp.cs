using UnityEngine;

public class SpeedPickUp : BasePickUp
{
    
    
    public override void OnPickUp()
    {
        powerName = "Speed Up";

        base.OnPickUp();

        player.StartCoroutine(player.speedUp(3f));

        Destroy(gameObject);

    }
}
