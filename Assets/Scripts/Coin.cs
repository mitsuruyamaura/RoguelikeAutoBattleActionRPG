using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropBoxBase
{
    protected override void TriggerDropBoxEffect(PlayerController_All playerController) {

        playerController.CalculateCoin(itemValue);

        base.TriggerDropBoxEffect(playerController);
    }
}
