using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : DropBoxBase 
{

    protected override void TriggerDropBoxEffect(PlayerController playerController) {
        UserDataManager.instance.CalculateFood(itemValue);
        
        base.TriggerDropBoxEffect(playerController);
    }
}
