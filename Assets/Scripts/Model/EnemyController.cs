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
    /// �����ݒ�
    /// </summary>
    public override void SetUpObstacleBase(ObstacleState defaultState) {
        base.SetUpObstacleBase(defaultState);

        TryGetComponent(out rb);
        TryGetComponent(out anim);

        // �����ړ��̕�����ݒ�
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
    /// �ړ����Ԃƈړ��񐔂̊Ď�
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
    /// �����_���ȕ����؂�ւ��̐ݒ�
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
    /// �w������ւ̓G�̏㉺���E�ړ�
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
    /// �o�g���̏�������
    /// </summary>
    public override void PrapareBattle() {
        cururentObstacleState = ObstacleState.Stop;
        rb.velocity = Vector2.zero;
    }
}
