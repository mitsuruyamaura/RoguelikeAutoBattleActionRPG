using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : ObstacleBase {

    [SerializeField, Header("�ړ������̊�l")]
    private float speed;

    [SerializeField, Header("�ړ����Ԃ̊�l")]
    private float duration;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isVertical;

    private int direction = 1;
    private Tween tween;


    private void Start() {
        // �f�o�b�O
        //SetUpObstacleBase(ObstacleState.Move);
    }

    /// <summary>
    /// �����ݒ�
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
    /// �o�g���̏�������
    /// </summary>
    public override void PrapareBattle() {
        PauseMove();
    }

    /// <summary>
    /// DOTween �ɂ�� Rigidbody2D ���g�����ړ�(MovePosition ���\�b�h�Ɠ����ɂȂ�̂ŁA�R���C�_�[�ƐڐG����)
    /// Rigidbody2D �� BodyType �� Dynamic �ɂ��AIsKinematic �͎g��Ȃ�(�g���ƐڐG���Ȃ�)
    /// </summary>
    private void MoveObstacle() {
        cururentObstacleState = ObstacleState.Move;

        // �����ړ��̕����������_���ݒ�
        isVertical = Random.Range(0, 2) == 0 ? false : true;

        // ������ݒ�
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
    /// �ړ��ĊJ
    /// </summary>
    public void ResumeMove() {
        cururentObstacleState = ObstacleState.Move;
        rb.isKinematic = false;
        tween.Play();
    }

    /// <summary>
    /// �ړ��̈ꎞ��~
    /// </summary>
    public void PauseMove() {
        cururentObstacleState = ObstacleState.Stop;
        tween.Pause();
        rb.isKinematic = true;    //�@�����Œe�����̂�h������
    }


/************ Rigidbody2D �ɂ��ړ� *************/

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
    ///// �ړ����Ԃƈړ��񐔂̊Ď�
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
    ///// �����_���ȕ����؂�ւ��̐ݒ�
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
    ///// �w������ւ̓G�̏㉺���E�ړ�
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
