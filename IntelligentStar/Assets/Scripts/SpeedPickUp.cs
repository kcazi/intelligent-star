using UnityEngine;

public class SpeedPickUp : BasePickUp
{
    
    
    public override void OnPickUp()
    {
        powerName = "Speed Up";

        base.OnPickUp();

        //TODO: Speeds up charcter
    }
}
