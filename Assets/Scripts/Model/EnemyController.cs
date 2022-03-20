using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : ObstacleBase {

    [SerializeField, Header("移動距離の基準値")]
    private float speed;

    [SerializeField, Header("移動時間の基準値")]
    private float duration;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isVertical;

    private int direction = 1;
    private Tween tween;


    private void Start() {
        // デバッグ
        //SetUpObstacleBase(ObstacleState.Move);
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    public override void SetUpObstacleBase(ObstacleState defaultState, StageManager_Presenter stageManager_Presenter) {
        base.SetUpObstacleBase(defaultState, stageManager_Presenter);

        TryGetComponent(out rb);
        TryGetComponent(out anim);

        MoveObstacle();

        //moveTimer = Random.Range(2, 4);
        //SetMoveChangeCount();
    }

    /// <summary>
    /// バトルの準備処理
    /// </summary>
    public override void PrapareBattle() {
        PauseMove();
    }

    /// <summary>
    /// DOTween による Rigidbody2D を使った移動(MovePosition メソッドと同じになるので、コライダーと接触する)
    /// Rigidbody2D の BodyType は Dynamic にし、IsKinematic は使わない(使うと接触しない)
    /// </summary>
    private void MoveObstacle() {
        cururentObstacleState = ObstacleState.Move;

        // 初期移動の方向をランダム設定
        isVertical = Random.Range(0, 2) == 0 ? false : true;

        // 向きを設定
        direction = -direction;
        //direction = Random.Range(0, 2) == 0 ? -direction : direction;

        if (isVertical) {
            tween = rb.DOMoveY(transform.position.y + Random.Range(speed, speed + 2) * direction, Random.Range(duration, duration + 2))
                .SetEase(Ease.Linear)
                .OnComplete(() => MoveObstacle());
            anim.SetFloat("X", 0);
            anim.SetFloat("Y", direction);
        } else {
            tween = rb.DOMoveX(transform.position.x + Random.Range(speed, speed + 2) * direction, Random.Range(duration, duration + 2))
                .SetEase(Ease.Linear)
                .OnComplete(() => MoveObstacle());
            anim.SetFloat("X", direction);
            anim.SetFloat("Y", 0);
        }
    }

    /// <summary>
    /// 移動再開
    /// </summary>
    public void ResumeMove() {
        cururentObstacleState = ObstacleState.Move;
        rb.isKinematic = false;
        tween.Play();
    }

    /// <summary>
    /// 移動の一時停止
    /// </summary>
    public void PauseMove() {
        cururentObstacleState = ObstacleState.Stop;
        tween.Pause();
        rb.isKinematic = true;    //　反動で弾かれるのを防ぐため
    }


/************ Rigidbody2D による移動 *************/

    //private float moveTimer;
    //private int moveChangeCount;
    //private int changeCounter;


    //void Update() {
    //    if (cururentObstacleState == ObstacleState.Stop) {
    //        return;
    //    }

    //    ObserveMove();
    //}

    ///// <summary>
    ///// 移動時間と移動回数の監視
    ///// </summary>
    //private void ObserveMove() {
    //    moveTimer -= Time.deltaTime;

    //    if (moveTimer < 0) {
    //        direction = -direction;
    //        moveTimer = Random.Range(2, 4);
    //        changeCounter++;
    //        if (changeCounter >= moveChangeCount) {
    //            SetMoveChangeCount();
    //        }
    //    }
    //}

    ///// <summary>
    ///// ランダムな方向切り替えの設定
    ///// </summary>
    //private void SetMoveChangeCount() {
    //    isVertical = !isVertical;
    //    changeCounter = 0;
    //    moveChangeCount = Random.Range(1, 5);
    //}

    //void FixedUpdate() {
    //    if (cururentObstacleState == ObstacleState.Stop) {
    //        rb.velocity = Vector2.zero;
    //        return;
    //    }

    //    MoveEnemy();
    //}

    ///// <summary>
    ///// 指定方向への敵の上下左右移動
    ///// </summary>
    //private void MoveEnemy() {
    //    Vector2 pos = rb.position;

    //    if (isVertical) {
    //        pos.y = pos.y + Time.deltaTime * speed * direction;
    //        anim.SetFloat("X", 0);
    //        anim.SetFloat("Y", direction);
    //    } else {
    //        pos.x = pos.x + Time.deltaTime * speed * direction;
    //        anim.SetFloat("X", direction);
    //        anim.SetFloat("Y", 0);
    //    }
    //    rb.MovePosition(pos);
    //}
}
