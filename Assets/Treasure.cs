using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine;

/// <summary>
/// 武器、アーティファクトと分けずに管理するためのクラス
/// </summary>
[Serializable]
public class ItemInfo {
    public int no;
    public Rarity rarity;
    public ItemType itemType;
}


public class Treasure : DropBoxBase {

    public WeaponData[] weaponDatas;

    public ItemInfo[] itemInfos;


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


    //public void SetUpTreasure(WeaponData[] weaponDatas) {

    //    this.weaponDatas = weaponDatas;

    //    EffectBase effect = Instantiate(EffectManager.instance.GetEffect(EffectType.TresureDrop), transform.position, Quaternion.identity);
    //    Destroy(effect.gameObject, 3.0f);
    //}


    public override void SetUpDropBox<T>(T[] t, ItemType itemType = ItemType.Weapon) {
        base.SetUpDropBox(t);

        // T 型は、一度 object 型にキャストしてから、実際にキャストしたい型にやり直す
        // そのため、メソッドの型引数は親のクラスのものをそのまま利用する
        // http://var.blog.jp/archives/67580859.html
        weaponDatas = (WeaponData[])(object)t;

        if (itemType == ItemType.Artifact) {

        }

        EffectBase effect = Instantiate(EffectManager.instance.GetEffect(EffectType.TresureDrop), transform.position, Quaternion.identity);
        Destroy(effect.gameObject, 3.0f);
    }
}
