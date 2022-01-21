using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

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
    /// �g�p���镐��̃Z�b�g
    /// </summary>
    /// <param name="weaponNo"></param>
    /// <param name="newUseCount"></param>
    public void SetUpWeapon(int weaponNo = 0, int newUseCount = 0) {

        // ����̏����擾���Č��݂̎g�p����̏��Ƃ��ēo�^
        currentWeapon = DataBaseManager.instance.GetWeaponData(weaponNo);

        // ����̃X�L�������擾���ēo�^
        currentWeapon.skillDatasList = DataBaseManager.instance.GetWeaponSkillDatas(currentWeapon.skillNos);

        // �X�L���̏d�ݕt���̍��v�l
        currentSkillTotalWeight = currentWeapon.skillDatasList.Sum(x => x.weight);

        currentUseCount = newUseCount;
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

    /// <summary>
    /// �g�p����X�L���̏����擾
    /// </summary>
    /// <param name="skillNoIndex"></param>
    /// <returns></returns>
    public SkillData GetUseSkillData(int skillNoIndex = -1) {

        // -1 �̏ꍇ�̓����_���ȃX�L�����d�ݕt������������P�擾
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
            // -1 �ȊO�̏ꍇ�͎w�肵���X�L��
            return currentWeapon.skillDatasList[skillNoIndex];
        }
    }
}
