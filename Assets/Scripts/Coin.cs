using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropBoxBase
{
    protected override void TriggerDropBoxEffect(PlayerController playerController) {
        UserDataManager.instance.CalculateCoin(itemValue);

        base.TriggerDropBoxEffect(playerController);
    }
}
