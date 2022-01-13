using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ���[�U�[�f�[�^�̊Ǘ��p�B��� MonoBehaviour �̌p�����Ȃ���
/// </summary>
public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    public User user;

    public Character currentCharacter;

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
