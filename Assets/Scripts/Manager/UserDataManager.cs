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
    /// �R�C���v�Z
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateCoin(int amount) {
        User.Coin.Value += amount;
        Debug.Log("�R�C���X�V");
    }

    /// <summary>
    /// �t�[�h�v�Z
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateFood(int amount) {
        User.Food.Value = Mathf.Clamp(User.Food.Value += amount, 0, CurrentCharacter.maxFood);
        Debug.Log("�t�[�h�X�V ���v�l :" + User.Food.Value);
    }
}
