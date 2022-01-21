using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : DropBoxBase
{
    protected override void TriggerDropBoxEffect(PlayerController_All playerController) {

        // エフェクト
        Debug.Log("ゴール");

        playerController.PrepareNextStage(itemValue);

        //base.TriggerDropBoxEffect(playerController);
    }

    public override void TriggerDropBoxEffect(StageManager_Presenter presenter) {

        // エフェクト
        Debug.Log("ゴール");

        presenter.ClearStage(itemValue);

        base.TriggerDropBoxEffect(presenter);
    }
}
