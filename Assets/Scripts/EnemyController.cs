using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : ObstacleBase
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private bool isVertical;

    [SerializeField]
    private float chargeTime;

    private Rigidbody2D rb;
    private Animator anim;
    private float timer;
    private int direction = 1;
    private int moveChangeCount;
    private int changeCounter;

    public override void SetUpObstacleBase() {
        base.SetUpObstacleBase();

        TryGetComponent(out rb);
        TryGetComponent(out anim);

        timer = chargeTime;
        moveChangeCount = Random.Range(1, 5);
    }

    void Update() {
        timer -= Time.deltaTime;

        if (timer < 0) {
            direction = -direction;
            timer = chargeTime;
            changeCounter++;
            if (changeCounter >= moveChangeCount) {
                SetMoveChangeCount();
            }
        }
    }

    /// <summary>
    /// ÉâÉìÉ_ÉÄÇ»ï˚å¸êÿÇËë÷Ç¶ÇÃê›íË
    /// </summary>
    private void SetMoveChangeCount() {

        isVertical = !isVertical;
        changeCounter = 0;
        moveChangeCount = Random.Range(1, 5);
    }

    void FixedUpdate() {
        Vector2 pos = rb.position;

        if (isVertical) {
            pos.y = pos.y + Time.deltaTime * speed * direction;
            anim.SetFloat("X", 0);
            anim.SetFloat("Y", direction);
        } else {
            pos.x = pos.x + Time.deltaTime * speed * direction;
            anim.SetFloat("X",direction);
            anim.SetFloat("Y", 0);
        }
        rb.MovePosition(pos);
    }
}
