using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public SkillDataSO skillDataSO;

    public WeaponDataSO weaponDataSO;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 武器のデータ取得
    /// </summary>
    /// <param name="getWeaponNo"></param>
    /// <returns></returns>
    public WeaponData GetWeaponData(int getWeaponNo) {
        return weaponDataSO.weaponDatasList.Find(x => x.no == getWeaponNo);
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
            return rarity switch {
                Rarity.Common => commonList,
                Rarity.Uncommon => uncommonList,
                _ => null
            };
        }
    }
}
