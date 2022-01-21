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
    /// ����̃f�[�^�擾
    /// </summary>
    /// <param name="getWeaponNo"></param>
    /// <returns></returns>
    public WeaponData GetWeaponData(int getWeaponNo) {
        return weaponDataSO.weaponDatasList.Find(x => x.no == getWeaponNo);
    }


    /// <summary>
    /// �����񂩂畐��̃X�L���f�[�^���X�g���쐬
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
}
