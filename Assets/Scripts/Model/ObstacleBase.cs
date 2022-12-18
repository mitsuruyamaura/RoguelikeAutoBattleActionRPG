using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ObstacleBase : MonoBehaviour
{
    public int hp;

    /// <summary>
    /// hp �̃v���p�e�B
    /// </summary>
    public int HP
    {
        get => hp;
        set => hp = value;
    }

    [SerializeField]
    private int attackPower;
    public int AttackPower { get => attackPower; set => attackPower = value; }


    // mi

    public int maxHp;

    public ReactiveProperty<int> Hp = new ReactiveProperty<int>();

    public int attackSpeed;

    public int treasureDropRate;
    public Rarity[] rarities;

    public int coinBoxRate;

    public enum ObstacleState {
        Move,
        Stop,

    }
    public ObstacleState cururentObstacleState;

    public ObstacleType obstacleType;
    public bool isGoal;
    private StageManager_Presenter stageManager;


    void Start() {

        // �f�o�b�O�p
        //SetUpObstacleBase();

    }

    /// <summary>
    /// �����ݒ�
    /// </summary>
    public virtual void SetUpObstacleBase(ObstacleState defaultState, StageManager_Presenter stageManager) {
        Hp.Value = hp;
        maxHp = hp;
        cururentObstacleState = defaultState;

        this.stageManager = stageManager;
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
        //stageManager.RemoveObstacleList(this);
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
