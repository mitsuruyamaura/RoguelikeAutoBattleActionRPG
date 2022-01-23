using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine;

public class Treasure : DropBoxBase {

    public WeaponData[] weaponDatas;

    //protected override void TriggerDropBoxEffect(PlayerController_All playerController) {

    //    // TODO エフェクト

    //    // 武器取得ポップアップを開く命令
    //    playerController.ShowWeaponSelectPopUp(weaponDatas);

    //    base.TriggerDropBoxEffect(playerController);
    //}

    //public void SetUpTreasure(WeaponData[] weaponDatas) {
    //    this.weaponDatas = weaponDatas;

    //    base.SetUpDropBox();
    //}

    public override void TriggerDropBoxEffect(StageManager_Presenter presenter) {

        //presenter.ShowWeaponSelectPopUp(weaponDatas);

        base.TriggerDropBoxEffect(presenter);

        Debug.Log(weaponDatas[0].name);
    }


    public void SetUpTreasure(WeaponData[] weaponDatas) {

        this.weaponDatas = weaponDatas;

        EffectBase effect = Instantiate(EffectManager.instance.GetEffect(EffectType.TresureDrop), transform.position, Quaternion.identity);
        Destroy(effect.gameObject, 3.0f);
    }
}
