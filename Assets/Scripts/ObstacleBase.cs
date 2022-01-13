using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
    [SerializeField]
    private int hp;

    [SerializeField]
    private int attackPower;

    public int maxHp;

    public int Hp { get => hp; set => hp = value; }
    public int AttackPower { get => attackPower; set => attackPower = value; }

    public int attackSpeed;

    public int treasureDropRate;
    public Rarity[] rarities;

    public int coinBoxRate;

    public enum ObstacleState {
        Move,
        Stop,

    }
    public ObstacleState cururentObstacleState;


    void Start() {

        // �f�o�b�O�p
        //SetUpObstacleBase();
    }

    /// <summary>
    /// �����ݒ�
    /// </summary>
    public virtual void SetUpObstacleBase(ObstacleState defaultState) {
        maxHp = Hp;
        cururentObstacleState = defaultState;
    }

    /// <summary>
    /// �o�g���O�̏�������
    /// </summary>
    public virtual void PrapareBattle() {
        // TODO �o�g���O�̏�������������΁A������L�q����

    }

    /// <summary>
    /// �j�󏈗�
    /// </summary>
    public virtual void DestroyObstacle() {
        Destroy(gameObject);
    }

    /// <summary>
    /// �g���W���[���h���b�v���邩����
    /// </summary>
    /// <returns></returns>
    public (bool, Rarity[]) JudgeDropTreasure() {
        return (treasureDropRate > Random.Range(0, 100) ? true : false, rarities);
    }
}
