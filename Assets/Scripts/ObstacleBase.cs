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

        // デバッグ用
        //SetUpObstacleBase();
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    public virtual void SetUpObstacleBase(ObstacleState defaultState) {
        maxHp = Hp;
        cururentObstacleState = defaultState;
    }

    /// <summary>
    /// バトル前の準備処理
    /// </summary>
    public virtual void PrapareBattle() {
        // TODO バトル前の準備処理があれば、それを記述する

    }

    /// <summary>
    /// 破壊処理
    /// </summary>
    public virtual void DestroyObstacle() {
        Destroy(gameObject);
    }

    /// <summary>
    /// トレジャーをドロップするか判定
    /// </summary>
    /// <returns></returns>
    public (bool, Rarity[]) JudgeDropTreasure() {
        return (treasureDropRate > Random.Range(0, 100) ? true : false, rarities);
    }
}
