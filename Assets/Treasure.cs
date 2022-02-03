using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine;

/// <summary>
/// ����A�A�[�e�B�t�@�N�g�ƕ������ɊǗ����邽�߂̃N���X
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

    //    // TODO �G�t�F�N�g

    //    // ����擾�|�b�v�A�b�v���J������
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

        // T �^�́A��x object �^�ɃL���X�g���Ă���A���ۂɃL���X�g�������^�ɂ�蒼��
        // ���̂��߁A���\�b�h�̌^�����͐e�̃N���X�̂��̂����̂܂ܗ��p����
        // http://var.blog.jp/archives/67580859.html
        weaponDatas = (WeaponData[])(object)t;

        if (itemType == ItemType.Artifact) {

        }

        EffectBase effect = Instantiate(EffectManager.instance.GetEffect(EffectType.TresureDrop), transform.position, Quaternion.identity);
        Destroy(effect.gameObject, 3.0f);
    }
}
