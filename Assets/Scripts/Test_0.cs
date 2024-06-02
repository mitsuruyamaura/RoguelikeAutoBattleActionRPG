using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Test_0 : MonoBehaviour
{
    public SkillDataSO skillDataSO;
    public WeaponDataSO weaponDataSO;

    Dictionary<Rarity, List<SkillData>> skillList = new Dictionary<Rarity, List<SkillData>>();

    public List<SkillData> commonSkillDatasList = new List<SkillData>();
    public List<SkillData> uncommonSkillDatasList = new List<SkillData>();
    public List<SkillData> rareSkillDatasList = new List<SkillData>();
    public List<SkillData> misticSkillDatasList = new List<SkillData>();

    public int[] totalWeights;
    public int totalSkillWeight;

    public List<SkillData> currentSkillDatasList = new List<SkillData>();

    public int limitBreakCount;
    public int initSkillCount;

    [SerializeField]
    private Button btnUseSkill;

    private int currentSkillTotalWeight;

    public WeaponData currentWeaponData;
    public int currentUseCount;


    [SerializeField]
    private WeaponSelectPopUp weaponSelectPopUpPrefab;
    [SerializeField]
    private Transform canvasTran;

    private WeaponSelectPopUp weaponSelectPopUp;
    public PlayerController_All playerController;


    void Awake() {
        totalWeights = new int[(int)Rarity.Count];
        for (int i = 0; i < (int)Rarity.Count; i++) {

            (int totalWeight, List<SkillData> list) = GetSkillDatasListByRarity((Rarity)i);

            skillList.Add((Rarity)i, list);
            totalWeights[i] = totalWeight;   // skillList[(Rarity)i].Select(x => x.weight).Sum();
        }
        //Debug.Log(skillList.Count);

        // デバッグ用
        commonSkillDatasList = skillList[Rarity.Common].ToList();
        uncommonSkillDatasList = skillList[Rarity.Uncommon].ToList();
        rareSkillDatasList = skillList[Rarity.Rare].ToList();
        misticSkillDatasList = skillList[Rarity.Mistic].ToList();

        //totalWeights = new int[(int)Rarity.Count];

        //totalWeights[0] = commonSkillDatasList.Select(x => x.weight).Sum();
        //totalWeights[1] = uncommonSkillDatasList.Select(x => x.weight).Sum();
        //totalWeights[2] = rareSkillDatasList.Select(x => x.weight).Sum();
        //totalWeights[3] = misticSkillDatasList.Select(x => x.weight).Sum();

        // デバッグ用
        totalSkillWeight = totalWeights.Sum();  // skillDataSO.skillDatasList.Select(x => x.weight).Sum();

        if (GameData.instance.stageNo == 0) {
            // コモンの武器をランダムで１つ付与(本番は３つの中から選択可能にする)
            SetWeapon(weaponDataSO.weaponDatasList[UnityEngine.Random.Range(0, 3)]);
        } else {
            SetWeapon(GameData.instance.currentWeaponData, GameData.instance.currentUseCount);
        }

        // 武器取得時のポップアップの生成
        weaponSelectPopUp = Instantiate(weaponSelectPopUpPrefab,canvasTran,false);
        //weaponSelectPopUp.SetUpPopUp(currentWeaponData, this);


        /// <summary>
        /// 引数に指定したレアリティのみのスキルデータの List を取得
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        (int, List<SkillData>) GetSkillDatasListByRarity(Rarity rarity) {
            List<SkillData> list = skillDataSO.skillDatasList.Where(x => x.rarity == rarity).ToList();
            return (list.Sum(x => x.weight), list);
        }
    }


    void Start() {
        btnUseSkill?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => UseRandomSkill());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            InitSkillDataList();
        }

        if (Input.GetMouseButtonDown(0)) {
            UseRandomSkill();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void InitSkillDataList() {
        currentSkillDatasList.Clear();

        for (int i = 0; i < initSkillCount; i++) {
            currentSkillDatasList.Add(GetRandomSkill());
        }
        Debug.Log("スキル初期化");

        currentSkillTotalWeight = currentSkillDatasList.Sum(x => x.weight);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SkillData GetRandomSkill() {
        return GetSkillData(GetRandomRarity());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Rarity GetRandomRarity() {
            int randomValue = UnityEngine.Random.Range(0, totalWeights.Sum());

            for (int i = 0; i < (int)Rarity.Count; i++) {
                if (totalWeights[i] > randomValue) {
                    return (Rarity)i;
                } else {
                    randomValue -= totalWeights[i];
                }
            }
            return Rarity.Common;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        SkillData GetSkillData(Rarity rarity) {
            int randomValue = UnityEngine.Random.Range(0, totalWeights[(int)rarity]);

            foreach (SkillData skillData in skillList[rarity]) {
                if (skillData.weight > randomValue) {
                    return skillData;
                } else {
                    randomValue -= skillData.weight;
                }
            }
            return null;
        }
    }


    public void UseRandomSkill() {
        int randomValue = UnityEngine.Random.Range(0, currentSkillTotalWeight);

        for (int i = 0; i < currentSkillDatasList.Count; i++) {
            if (currentSkillDatasList[i].weight > randomValue) {
                TriggerSkill(currentSkillDatasList[i]);
                break;
            } else {
                randomValue -= currentSkillDatasList[i].weight;
            }
        }
    }


    public void TriggerSkill(SkillData skillData) {
        skillData.count--;

        if (skillData.count <= 0) {

            int index = currentSkillDatasList.IndexOf(skillData);

            currentSkillDatasList.Insert(index, GetRandomSkill());

            currentSkillDatasList.Remove(skillData);
        }
    }

    /// <summary>
    /// 攻撃時に実行し、ランダムなスキルを重み付けによって取得
    /// </summary>
    /// <returns></returns>
    public SkillData GetUseRandomSkillData() {
        int randomValue = UnityEngine.Random.Range(0, currentSkillTotalWeight);

        for (int i = 0; i < currentWeaponData.skillDatasList.Count; i++) {
            if (currentWeaponData.skillDatasList[i].weight > randomValue) {
                return currentWeaponData.skillDatasList[i];
            } else {
                randomValue -= currentWeaponData.skillDatasList[i].weight;
            }
        }
        return null;
    }

    /// <summary>
    /// 重複させない武器データの抽出
    /// </summary>
    /// <param name="searchRarities"></param>
    /// <returns></returns>
    public WeaponData[] GetWeaponDataByRarity(Rarity[] searchRarities) {
        List<WeaponData> commonList = weaponDataSO.weaponDatasList.Where(x => x.rarity == Rarity.Common).ToList();
        List<WeaponData> uncommonList = weaponDataSO.weaponDatasList.Where(x => x.rarity == Rarity.Uncommon).ToList();

        WeaponData[] weaponDatas = new WeaponData[searchRarities.Length];

        for (int i = 0; i < searchRarities.Length; i++) {
            (int randomValue, int loopCount) = (UnityEngine.Random.Range(0, GetTotalWeight(searchRarities[i])), GetListCount(searchRarities[i]));
            //Debug.Log(GetTotalWeight(searchRarities[i]));
            //Debug.Log(loopCount);

            for (int x = 0; x < loopCount; x++) {

                int totalWeight = GetTotalWeight(searchRarities[i]);

                if (GetList(searchRarities[i])[x].weight > randomValue) {
                    weaponDatas[i] = GetList(searchRarities[i])[x];

                    // 重複防止
                    GetList(searchRarities[i]).Remove(GetList(searchRarities[i])[x]);

                    //Debug.Log(commonList.Count);
                    //Debug.Log(uncommonList.Count);

                    break;
                } else {
                    randomValue -= GetList(searchRarities[i])[x].weight;
                }
            }
        }

        return weaponDatas;


        int GetTotalWeight(Rarity rarity) {
            return rarity switch {
                Rarity.Common => commonList.Sum(x => x.weight),
                Rarity.Uncommon => uncommonList.Sum(x => x.weight),
                _ => 0
            };
        }


        int GetListCount(Rarity rarity) {
            return rarity switch {
                Rarity.Common => commonList.Count,
                Rarity.Uncommon => uncommonList.Count,
                _ => 0
            };
        }

        List<WeaponData> GetList(Rarity rarity) {
            return rarity switch{
                Rarity.Common => commonList,
                Rarity.Uncommon => uncommonList,
                _ => null
            };
        }
    }


    public void SetWeapon(WeaponData newWeaponData, int newUseCount = 0) {

        currentWeaponData = newWeaponData;

        currentWeaponData.skillDatasList = GetWeaponSkillDatas(currentWeaponData.skillNos);

        currentSkillTotalWeight = currentWeaponData.skillDatasList.Sum(x => x.weight);
        currentUseCount = newUseCount;        
    }

    /// <summary>
    /// 文字列から武器のスキルデータリストを作成
    /// </summary>
    /// <param name="skillNos"></param>
    /// <returns></returns>
    public List<SkillData> GetWeaponSkillDatas(string skillNos) {
        List<SkillData> list = new List<SkillData>();

        int[] skillNoArray = skillNos.Split(',').Select(x => int.Parse(x)).ToArray();

        for (int i = 0; i < skillNoArray.Length; i++) {
            list.Add(skillDataSO.skillDatasList.Find(x => x.no == skillNoArray[i]));
        }
        return list;
    }

    public void ShowWeaponSelectPopUp(WeaponData newWeaponData) {
        weaponSelectPopUp.ShowPopUp(newWeaponData, currentWeaponData, currentUseCount);
    }


    public void HideWeaponSelectPopUp() {
        playerController.currentPlayerState = PlayerController_All.PlayerState.Move;
    }


    public void NextStage() {

        GameData.instance.currentWeaponData = currentWeaponData;
        GameData.instance.currentUseCount = currentUseCount;

        GameData.instance.stageNo++;

        SceneManager.LoadScene("Main");
    }
}
