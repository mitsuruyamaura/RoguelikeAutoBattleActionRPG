using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

/// <summary>
/// ユーザーデータの管理用。後で MonoBehaviour の継承をなくす
/// </summary>
public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    private User user;
    public User User { get => user; set => user = value; }

    private Character currentCharacter;
    public Character CurrentCharacter { get => currentCharacter; set => currentCharacter = value; }

    public ReactiveProperty<int> Hp = new ReactiveProperty<int>();

    private WeaponData currentWeapon;
    public WeaponData CurrentWeapon { get => currentWeapon; set => currentWeapon = value; }
    private int currentSkillTotalWeight;
    public int currentUseCount;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 使用する武器のセット
    /// </summary>
    /// <param name="weaponNo"></param>
    /// <param name="newUseCount"></param>
    public void SetUpWeapon(int weaponNo = 0, int newUseCount = 0) {

        // 武器の情報を取得して現在の使用武器の情報として登録
        currentWeapon = DataBaseManager.instance.GetWeaponData(weaponNo);

        // 武器のスキル情報を取得して登録
        currentWeapon.skillDatasList = DataBaseManager.instance.GetWeaponSkillDatas(currentWeapon.skillNos);

        // スキルの重み付けの合計値
        currentSkillTotalWeight = currentWeapon.skillDatasList.Sum(x => x.weight);

        currentUseCount = newUseCount;
    }

    /// <summary>
    /// コイン計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateCoin(int amount) {
        User.Coin.Value += amount;
        Debug.Log("コイン更新");
    }

    /// <summary>
    /// フード計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateFood(int amount) {
        User.Food.Value = Mathf.Clamp(User.Food.Value += amount, 0, CurrentCharacter.maxFood);
        Debug.Log("フード更新 合計値 :" + User.Food.Value);
    }

    /// <summary>
    /// 使用するスキルの情報を取得
    /// </summary>
    /// <param name="skillNoIndex"></param>
    /// <returns></returns>
    public SkillData GetUseSkillData(int skillNoIndex = -1) {

        // -1 の場合はランダムなスキルを重み付けした中から１つ取得
        if(skillNoIndex == -1) {

            int randomValue = Random.Range(0, currentSkillTotalWeight);

            for (int i = 0; i < currentWeapon.skillDatasList.Count; i++) {
                if (currentWeapon.skillDatasList[i].weight > randomValue) {
                    return currentWeapon.skillDatasList[i];
                } else {
                    randomValue -= currentWeapon.skillDatasList[i].weight;
                }
            }
            return null;
        } else {
            // -1 以外の場合は指定したスキル
            return currentWeapon.skillDatasList[skillNoIndex];
        }
    }
}
