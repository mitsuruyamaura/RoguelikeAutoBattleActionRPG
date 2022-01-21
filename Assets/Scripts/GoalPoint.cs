using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : DropBoxBase
{
    protected override void TriggerDropBoxEffect(PlayerController_All playerController) {

        // �G�t�F�N�g
        Debug.Log("�S�[��");

        playerController.PrepareNextStage(itemValue);

        //base.TriggerDropBoxEffect(playerController);
    }

    public override void TriggerDropBoxEffect(StageManager_Presenter presenter) {

        // �G�t�F�N�g
        Debug.Log("�S�[��");

        presenter.ClearStage(itemValue);

        base.TriggerDropBoxEffect(presenter);
    }
}
