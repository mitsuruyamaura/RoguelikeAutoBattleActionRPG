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


    public void SetUpPopUp(WeaponData baseWeaponData) {

        canvasGroup.alpha = 0;
        imgFrame.transform.localScale = Vector3.zero;

        gameObject.SetActive(false);

        for (int i = 0; i < baseWeaponData.skillDatasList.Count; i++) {
            newSkillDetailsList.Add(Instantiate(skillDetailPrefab, newSkillDetailsTran, false));
            baseSkillDetalisList.Add(Instantiate(skillDetailPrefab, baseSkillDetailsTran, false));
        }

        btnChangeWeapon?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => OnClickChange())
            .AddTo(this);

        btnHoldWeapon?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => OnClickHold())
            .AddTo(this);
    }


    public void ShowPopUp(WeaponData newWeaponData, WeaponData baseWeaponData, int currentBp) {

        gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.5f).SetEase(Ease.Linear));
        sequence.Join(imgFrame.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.InQuart));
        sequence.Append(imgFrame.transform.DOScale(Vector3.one, 0.15f));

        this.newWeaponData = newWeaponData;

        //imgNewWeaponSprite.sprite = newWeaponData.sprite;
        txtNewWeaponName.text = newWeaponData.name;
        txtNewWeaponRarity.text = newWeaponData.rarity.ToString();
        txtNewWeaponBp.text = 0 + " / " + newWeaponData.useCount.ToString();

        //imgBaseWeaponSprite.sprite = baseWeaponData.sprite;
        txtBaseWeaponName.text = baseWeaponData.name;
        txtBaseWeaponRarity.text = baseWeaponData.rarity.ToString();
        txtBaseWeaponBp.text = currentBp + " / " + baseWeaponData.useCount;

        for (int i = 0; i < newWeaponData.skillDatasList.Count; i++) {
            newSkillDetailsList[i].SetUpSkillDetail(newWeaponData.skillDatasList[i], newWeaponData.skillDatasList.Sum(x => x.weight));
            baseSkillDetalisList[i].SetUpSkillDetail(baseWeaponData.skillDatasList[i], baseWeaponData.skillDatasList.Sum(x => x.weight));
        }
    }


    public void HidePopUp() {

        canvasGroup.DOFade(0, 0.5f);
        imgFrame.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuart).OnComplete(() => gameObject.SetActive(false));
    }

    private void OnClickChange() {

        // ïêäÌåä∑
        UserDataManager.instance.SetUpWeapon(newWeaponData.no);

        Debug.Log("ïêäÌåä∑");

        HidePopUp();
    }


    private void OnClickHold() {
        HidePopUp();
    }
}
