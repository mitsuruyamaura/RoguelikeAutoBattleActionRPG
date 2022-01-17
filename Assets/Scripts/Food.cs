using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : DropBoxBase
{

    protected override void TriggerDropBoxEffect(PlayerController_All playerController) {

        playerController.CalculateFood(itemValue);
        
        base.TriggerDropBoxEffect(playerController);
    }
}
