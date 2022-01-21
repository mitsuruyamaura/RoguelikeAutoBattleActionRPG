using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
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
}
