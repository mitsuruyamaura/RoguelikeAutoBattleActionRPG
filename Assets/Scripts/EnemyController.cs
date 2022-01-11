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

    /// <summary>
    /// ‰Šúİ’è
    /// </summary>
    public override void SetUpObstacleBase() {
        base.SetUpObstacleBase();

        TryGetComponent(out rb);
        TryGetComponent(out anim);

        // ‰ŠúˆÚ“®‚Ì•ûŒü‚ğİ’è
        isVertical = Random.Range(0, 2) == 0 ? false : true;
        moveTimer = Random.Range(2, 4);

        SetMoveChangeCount();
    }

    void Update() {
        ObserveMove();
    }

    /// <summary>
    /// ˆÚ“®ŠÔ‚ÆˆÚ“®‰ñ”‚ÌŠÄ‹
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
    /// ƒ‰ƒ“ƒ_ƒ€‚È•ûŒüØ‚è‘Ö‚¦‚Ìİ’è
    /// </summary>
    private void SetMoveChangeCount() {
        isVertical = !isVertical;
        changeCounter = 0;
        moveChangeCount = Random.Range(1, 5);
    }

    void FixedUpdate() {
        MoveEnemy();
    }

    /// <summary>
    /// w’è•ûŒü‚Ö‚Ì“G‚Ìã‰º¶‰EˆÚ“®
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
}
