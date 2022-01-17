using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : DropBoxBase 
{

    protected override void TriggerDropBoxEffect(PlayerController playerController) {

        CalculateFood(itemValue);
        
        base.TriggerDropBoxEffect(playerController);
    }

    /// <summary>
    /// �t�[�h�v�Z
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateFood(int amount) {
        UserDataManager.instance.User.Food.Value = Mathf.Clamp(UserDataManager.instance.User.Food.Value += amount, 0, UserDataManager.instance.CurrentCharacter.maxFood);
        Debug.Log("�t�[�h�X�V ���v�l :" + UserDataManager.instance.User.Food.Value);
    }
}
