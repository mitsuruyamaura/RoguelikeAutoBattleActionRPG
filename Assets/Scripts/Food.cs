using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : DropBoxBase 
{

    public override void TriggerDropBoxEffect(StageManager_Presenter presenter) {
        UserDataManager.instance.CalculateFood(itemValue);
        
        base.TriggerDropBoxEffect(presenter);
    }
}
