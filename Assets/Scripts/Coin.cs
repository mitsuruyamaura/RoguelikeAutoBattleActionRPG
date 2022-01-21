using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropBoxBase
{
    public override void TriggerDropBoxEffect(StageManager_Presenter presenter) {
        UserDataManager.instance.CalculateCoin(itemValue);

        base.TriggerDropBoxEffect(presenter);
    }
}
