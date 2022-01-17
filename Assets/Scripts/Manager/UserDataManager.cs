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
}
