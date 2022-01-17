using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropBoxBase
{
    protected override void TriggerDropBoxEffect(PlayerController playerController) {
        CalculateCoin(itemValue);

        base.TriggerDropBoxEffect(playerController);
    }

    /// <summary>
    /// コイン計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateCoin(int amount) {
        UserDataManager.instance.User.Coin.Value += amount;
        Debug.Log("コイン更新");
    }
}
