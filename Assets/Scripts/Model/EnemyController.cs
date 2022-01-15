using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : ObstacleBase
{
    [SerializeField]
    private float speed;
    
    private Rigidbody2D rb;
    private Animator anim;
    private bool isVertical;

    private float moveTimer;

    private int direction = 1;
    private int moveChangeCount;
    private int changeCounter;

    private void Start() {
        SetUpObstacleBase(ObstacleState.Move);
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    public override void SetUpObstacleBase(ObstacleState defaultState) {
        base.SetUpObstacleBase(defaultState);

        TryGetComponent(out rb);
        TryGetComponent(out anim);

        // 初期移動の方向を設定
        isVertical = Random.Range(0, 2) == 0 ? false : true;
        moveTimer = Random.Range(2, 4);

        cururentObstacleState = ObstacleState.Move;

        SetMoveChangeCount();
    }

    void Update() {
        if(cururentObstacleState == ObstacleState.Stop) {
            return;
        }

        ObserveMove();
    }

    /// <summary>
    /// 移動時間と移動回数の監視
    /// </summary>
    private void ObserveMove() {
        moveTimer -= Time.deltaTime;

        if (moveTimer < 0) {
            direction = -direction;
            moveTimer = Random.Range(2, 4);
            changeCounter++;
            if (changeCounter >= moveChangeCount) {
                SetMoveChangeCount();
            }
        }
    }

    /// <summary>
    /// ランダムな方向切り替えの設定
    /// </summary>
    private void SetMoveChangeCount() {
        isVertical = !isVertical;
        changeCounter = 0;
        moveChangeCount = Random.Range(1, 5);
    }

    void FixedUpdate() {
        if (cururentObstacleState == ObstacleState.Stop) {
            return;
        }

        MoveEnemy();
    }

    /// <summary>
    /// 指定方向への敵の上下左右移動
    /// </summary>
    private void MoveEnemy() {
        Vector2 pos = rb.position;

        if (isVertical) {
            pos.y = pos.y + Time.deltaTime * speed * direction;
            anim.SetFloat("X", 0);
            anim.SetFloat("Y", direction);
        } else {
            pos.x = pos.x + Time.deltaTime * speed * direction;
            anim.SetFloat("X", direction);
            anim.SetFloat("Y", 0);
        }
        rb.MovePosition(pos);
    }

    /// <summary>
    /// バトルの準備処理
    /// </summary>
    public override void PrapareBattle() {
        cururentObstacleState = ObstacleState.Stop;
        rb.velocity = Vector2.zero;
    }
}
