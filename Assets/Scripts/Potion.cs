using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : DropBoxBase
{
    protected override void TriggerDropBoxEffect(PlayerController_All playerController) {

        playerController.GainLife(itemValue);
        
        base.TriggerDropBoxEffect(playerController);
    }
}
