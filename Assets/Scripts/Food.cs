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
    /// フード計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateFood(int amount) {
        UserDataManager.instance.User.Food.Value = Mathf.Clamp(UserDataManager.instance.User.Food.Value += amount, 0, UserDataManager.instance.CurrentCharacter.maxFood);
        Debug.Log("フード更新 合計値 :" + UserDataManager.instance.User.Food.Value);
    }
}
