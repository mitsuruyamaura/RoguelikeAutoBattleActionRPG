using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField]
    private float moveSpeed = 3.0f;

    private float horizontal;
    private float vertical;

    private Vector2 lookDirection = new Vector2(1, 0);
    private bool isDash;

    public enum PlayerState {
        Move,
        Battle,
        Result,
        Info,
        GameUp
    }

    public PlayerState currentPlayerState;


    // Start is called before the first frame update
    void Start() {
        TryGetComponent(out rb);
        TryGetComponent(out anim);

        this.UpdateAsObservable()
            .Where(_ => currentPlayerState == PlayerState.Move)
            .Subscribe(_ => {
#if UNITY_EDITOR
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
#elif UNITY_ANDROID
                horizontal = joystick.Horizontal;
                vertical = joystick.Vertical;
#endif
                isDash = Input.GetKey(KeyCode.LeftShift) ? true : false;
                
                if (anim) {
                    SyncMoveAnimation();
                }
            }).AddTo(this);

        this.FixedUpdateAsObservable()
            .Where(_ => rb && currentPlayerState == PlayerState.Move)
            .Subscribe(_ => Move()).AddTo(this);
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move() {
        Vector2 dir = new Vector3(horizontal, vertical).normalized;
        float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * 2.0f : moveSpeed;
        speed = isDash ? moveSpeed * 2.0f : moveSpeed;

        rb.velocity = dir * speed;
    }

    /// <summary>
    /// 移動する方向と移動アニメの同期
    /// </summary>
    private void SyncMoveAnimation() {
        if (!Mathf.Approximately(horizontal, 0.0f) || !Mathf.Approximately(vertical, 0.0f)) {
            lookDirection.Set(horizontal, vertical);
            lookDirection.Normalize();

            anim.SetFloat("Look X", lookDirection.x);
            anim.SetFloat("Look Y", lookDirection.y);

            // ダッシュ有無に応じてアニメの再生速度を調整
            anim.SetFloat("Speed", isDash ? 2 : lookDirection.sqrMagnitude);
        } else {
            anim.SetFloat("Speed", 0);
        }
    }


    /***** UniRX を使わない場合  *****/

    //void Update()
    //{
    //    horizontal = Input.GetAxis("Horizontal");
    //    vertical = Input.GetAxis("Vertical");
    //    isDash = Input.GetKey(KeyCode.LeftShift)? true : false;
    //
    //    if (anim) {
    //        SyncMoveAnimation();
    //    }
    //}

    //void FixedUpdate() {
    //    if (rb) {
    //        Move();
    //    }
    //}
}
