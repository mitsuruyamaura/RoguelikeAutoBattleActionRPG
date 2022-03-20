using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System.Linq;
using System;

public class WeaponSelectPopUp : MonoBehaviour
{
    [SerializeField]
    private Button btnChangeWeapon;

    [SerializeField]
    private Button btnHoldWeapon;

    [SerializeField]
    private Image imgNewWeaponSprite;

    [SerializeField]
    private Image imgBaseWeaponSprite;

    [SerializeField]
    private Image imgFrame;

    [SerializeField]
    private Text txtNewWeaponName;

    [SerializeField]
    private Text txtNewWeaponRarity;

    [SerializeField]
    private Text txtNewWeaponBp;

    [SerializeField]
    private Text txtBaseWeaponName;

    [SerializeField]
    private Text txtBaseWeaponRarity;

    [SerializeField]
    private Text txtBaseWeaponBp;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private SkillDetail skillDetailPrefab;

    [SerializeField]
    private List<SkillDetail> newSkillDetailsList = new List<SkillDetail>();

    [SerializeField]
    private List<SkillDetail> baseSkillDetalisList = new List<SkillDetail>();

    [SerializeField]
    private Transform newSkillDetailsTran;

    [SerializeField]
    private Transform baseSkillDetailsTran;

    private WeaponData newWeaponData;
    private StageManager_Presenter stageManager;


    /// <summary>
    /// ポップアップの初期設定
    /// </summary>
    /// <param name="baseWeaponData"></param>
    public void SetUpPopUp(WeaponData baseWeaponData, StageManager_Presenter stageManager) {
        this.stageManager = stageManager;

        // 非表示処理
        canvasGroup.alpha = 0;
        imgFrame.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);

        // SkillDetail を２つ生成。現在の武器と新しい武器の表示用
        for (int i = 0; i < baseWeaponData.skillDatasList.Count; i++) {
            newSkillDetailsList.Add(Instantiate(skillDetailPrefab, newSkillDetailsTran, false));
            baseSkillDetalisList.Add(Instantiate(skillDetailPrefab, baseSkillDetailsTran, false));
        }

        // ボタンの購読
        btnChangeWeapon?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => OnClickChange())
            .AddTo(this);

        btnHoldWeapon?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => OnClickHold())
            .AddTo(this);
    }

    /// <summary>
    /// ポップアップの表示
    /// </summary>
    /// <param name="newWeaponData"></param>
    /// <param name="baseWeaponData"></param>
    /// <param name="currentBp"></param>
    public void ShowPopUp(WeaponData newWeaponData, WeaponData baseWeaponData, int currentBp) {

        // アニメさせて表示
        gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.5f).SetEase(Ease.Linear));
        sequence.Join(imgFrame.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.InQuart));
        sequence.Append(imgFrame.transform.DOScale(Vector3.one, 0.15f));

        this.newWeaponData = newWeaponData;

        // 新しい武器の情報の表示
        //imgNewWeaponSprite.sprite = newWeaponData.sprite;
        txtNewWeaponName.text = newWeaponData.name;
        txtNewWeaponRarity.text = newWeaponData.rarity.ToString();
        txtNewWeaponBp.text = 0 + " / " + newWeaponData.useCount.ToString();

        // 現在の武器の情報の表示
        //imgBaseWeaponSprite.sprite = baseWeaponData.sprite;
        txtBaseWeaponName.text = baseWeaponData.name;
        txtBaseWeaponRarity.text = baseWeaponData.rarity.ToString();
        txtBaseWeaponBp.text = currentBp + " / " + baseWeaponData.useCount;

        // 武器ごとのスキル情報の設定と表示
        for (int i = 0; i < newWeaponData.skillDatasList.Count; i++) {
            newSkillDetailsList[i].SetUpSkillDetail(newWeaponData.skillDatasList[i], newWeaponData.skillDatasList.Sum(x => x.weight));
            baseSkillDetalisList[i].SetUpSkillDetail(baseWeaponData.skillDatasList[i], baseWeaponData.skillDatasList.Sum(x => x.weight));
        }
    }

    /// <summary>
    /// ポップアップの非表示
    /// </summary>
    public void HidePopUp() {

        // アニメしながら非表示
        canvasGroup.DOFade(0, 0.5f);
        imgFrame.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuart).OnComplete(() => gameObject.SetActive(false));

        stageManager.SwitchPause(false);
    }

    /// <summary>
    /// 新しい武器への交換
    /// </summary>
    private void OnClickChange() {

        // 武器交換
        UserDataManager.instance.SetUpWeapon(newWeaponData.no);

        Debug.Log("武器交換");

        HidePopUp();
    }

    /// <summary>
    /// 現在の武器の保持
    /// </summary>
    private void OnClickHold() {
        HidePopUp();
    }
}
