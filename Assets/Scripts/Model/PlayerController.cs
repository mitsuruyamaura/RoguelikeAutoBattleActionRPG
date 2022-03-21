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
        Battle_Before,
        Battle_After,
        Result,
        Info,
        GameUp
    }

    public PlayerState currentPlayerState;

    public ReactiveProperty<PlayerState> CurrentPlayerState = new ReactiveProperty<PlayerState>(PlayerState.Move);
    public ReactiveProperty<bool> IsPause = new ReactiveProperty<bool>(false);


    void Start() {
        TryGetComponent(out rb);
        TryGetComponent(out anim);

        this.UpdateAsObservable()
            .Where(_ => currentPlayerState == PlayerState.Move || CurrentPlayerState.Value == PlayerState.Move)
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
            .Where(_ => rb && currentPlayerState == PlayerState.Move || CurrentPlayerState.Value == PlayerState.Move)
            .Subscribe(_ => Move()).AddTo(this);

        this.OnCollisionEnter2DAsObservable()
            .Where(_ => currentPlayerState == PlayerState.Move || CurrentPlayerState.Value == PlayerState.Move)
            .Subscribe(col => {
                if (col.gameObject.TryGetComponent(out ObstacleBase enemy)) {
                    StartCoroutine(AutoBattle(enemy));
                }
        }).AddTo(this);
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move() {
        if (currentPlayerState == PlayerState.GameUp) {
            StopMove();
        }

        Vector2 dir = new Vector3(horizontal, vertical).normalized;
        float speed = isDash ? moveSpeed * 2.0f : moveSpeed;

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

    /// <summary>
    /// 自動バトル
    /// </summary>
    /// <param name="obstacle"></param>
    /// <returns></returns>
    private IEnumerator AutoBattle(ObstacleBase obstacle) {

        Debug.Log("バトル開始");

        // 双方の Hp ゲージが表示中か確認　アニメ中なら停止(自動で購読させるので、不要)

        obstacle.PrapareBattle();

        // 障害物のHp ゲージを最大値にする(自動で購読させるので、不要)

        // プレイヤーの移動を停止
        rb.velocity = Vector2.zero;

        // ステートを変更して、移動の入力を受け付けないようにする
        currentPlayerState = PlayerState.Battle_Before;
        CurrentPlayerState.Value = PlayerState.Battle_Before;

        // 双方の Hp ゲージを画面に移動して表示(自動で購読させるので、不要)

        // ズームイン(自動で購読させるので、不要)

        // バトル時のエフェクト表示
        EffectBase[] effect = new EffectBase[EffectManager.instance.GetEffects(EffectType.Battle).Length];
        for (int i = 0; i < effect.Length; i++) {
            effect[i] = Instantiate(EffectManager.instance.GetEffects(EffectType.Battle)[i], transform.position, EffectManager.instance.GetEffects(EffectType.Battle)[i].transform.rotation);
        }

        // バトル監視
        while (currentPlayerState == PlayerState.Battle_Before || CurrentPlayerState.Value == PlayerState.Battle_Before) {

            // 一時停止
            if (IsPause.Value) {
                yield return null;
            }

            // 同じ値を通知
            IsPause.SetValueAndForceNotify(false);

            // カメラ振動(自動で購読させるので、不要)

            // 使用するスキル決定(スキルの番号を指定すると、そのスキルを利用。それ以外はランダム)
            SkillData useSkillData = UserDataManager.instance.GetUseSkillData();
            Debug.Log("使用スキル : " + useSkillData.name);

            // TODO アーティファクトによる攻撃速度を加算
            int totalAttackSpeed = Random.Range(3, 10);

            // 攻撃回数カウント
            UserDataManager.instance.currentUseCount++;

            // TODO 指定した回数攻撃を行ったら(購読する形に変える)
            
            // TODO ボーナスの候補
            // アーティファクト購入用のポイント獲得 => 任意のベース能力値アップ
            // 武器のレアリティ基準のアーティファクトを１つランダムで入手


            // 攻撃順番確認
            if (totalAttackSpeed >= obstacle.attackSpeed) {
                AttackPlayer(useSkillData);
                yield return new WaitForSeconds(0.25f);
                AttackEnemy();
            } else {
                AttackEnemy();
                yield return new WaitForSeconds(0.25f);
                AttackPlayer(useSkillData);
            }
            yield return new WaitForSeconds(0.25f);
        }

        void AttackPlayer(SkillData skillData = null) {

            // スキルデータがあるかないかで処理を変更
            int totalAttackPower = skillData != null ? skillData.attackPower : Random.Range(1, 4);
            bool isCritical = skillData != null ? JudgeCriticalHit() : Random.Range(0, 2) == 0 ? true : false;
            totalAttackPower *= isCritical ? 3 : 1;

            Debug.Log(totalAttackPower);

            obstacle.Hp.Value -= totalAttackPower;
            Debug.Log("障害物の残り HP : " + obstacle.Hp);

            // Hp ゲージの同期(自動で購読させるので、不要)

            // フローティングメッセージの生成(自動で購読させるので、不要)

            if (obstacle.Hp.Value <= 0) {
                currentPlayerState = PlayerState.Result;
                CurrentPlayerState.Value = PlayerState.Result;
            }

            /// <summary>
            /// クリティカル判定
            /// </summary>
            bool JudgeCriticalHit() {
                return skillData.criticalRate > Random.Range(0, 100) ? true : false;
            }
        }

        void AttackEnemy() {
            UserDataManager.instance.Hp.Value -= obstacle.AttackPower;
            Debug.Log("プレイヤーの残り HP : " + UserDataManager.instance.Hp.Value);

            // HP ゲージが表示されている場合には、ゲージの移動処理を止める

            // ゲージの移動と同期(自動で購読させるので、不要)

            //imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);
            //FloatingMessage playerFloatingMessage = Instantiate(floatingMessagePrefab, playerHpGaugeTrans[1].transform, false);
            //playerFloatingMessage.ShowMessage(-enemy.AttackPower);

            // Hp が 0 以下かどうか判定してステート変更(購読しているので処理不要)
        }

        //Debug.Log(currentPlayerState == PlayerState.Result ? "勝利" : "敗北");
        Debug.Log(CurrentPlayerState.Value == PlayerState.Result ? "勝利" : "敗北");

        // バトルのエフェクトを破棄
        for (int i = 0; i < effect.Length; i++) {
            Destroy(effect[i].gameObject);
        }

        // ズームアウト(自動で購読させるので、不要)

        // リザルト処理入れる(それまで hp を見せておく)
        if (currentPlayerState == PlayerState.Result || CurrentPlayerState.Value == PlayerState.Result) {

            // ゴール地点の設定がある場合、ゴールを生成する
            if (obstacle.isGoal) {
                DropItemManager.instance.GenerateDropItem(ItemType.Goal, new Vector2(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f)));
                //DropBoxBase goal = Instantiate(DropItemManager.instance.GetDropItemPrefab(ItemType.Goal),
                //    new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
            }

            // ドロップアイテムがあるか判定
            (bool isDropTreasure, Rarity[] rarities) = obstacle.JudgeDropTreasure();

            if (isDropTreasure) {

                WeaponData[] weaponDatas = new WeaponData[rarities.Length];

                weaponDatas = DataBaseManager.instance.GetWeaponDataByRarity(rarities);
                DropBoxBase treasure = DropItemManager.instance.GenerateDropItem(ItemType.Weapon, new Vector2(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f)));
                //treasure.GetComponent<Treasure>().SetUpTreasure(weaponDatas);
                treasure.SetUpDropBox(weaponDatas);
            } else {

                // コインかフードの生成
                GenerateNormalItem();
            }
        }
        // 障害物の削除
        obstacle.DestroyObstacle();

        yield return new WaitForSeconds(0.5f);

        currentPlayerState = PlayerState.Battle_After;
        CurrentPlayerState.Value = PlayerState.Battle_After;

        yield return new WaitForSeconds(0.25f);

        // すぐに武器を取得している場合には、移動状態にはしない
        if (currentPlayerState == PlayerState.Info || CurrentPlayerState.Value == PlayerState.Info) {
            yield break;
        }

        currentPlayerState = PlayerState.Move;
        CurrentPlayerState.Value = PlayerState.Move;

        // HP ゲージを画面外へ移動(自動で購読させるので、不要)
    }

    /// <summary>
    /// お金かフードを生成
    /// </summary>
    private void GenerateNormalItem() {
        DropItemManager.instance.GenerateDropItem(Random.Range(0, 100) > 50 ? ItemType.Coin : ItemType.Food, 
            new Vector2(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f)));

        //DropBoxBase itemPrefab = DropItemManager.instance.GetDropItemPrefab(Random.Range(0, 100) > 50 ?  ItemType.Coin : ItemType.Food);
        //DropBoxBase item = Instantiate(itemPrefab, new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
    }


    public void StopMove() {
        rb.velocity = Vector2.zero;
        return;
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
