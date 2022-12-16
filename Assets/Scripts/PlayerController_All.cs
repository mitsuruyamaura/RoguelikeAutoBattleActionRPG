using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController_All : MonoBehaviour
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
    public ReactiveProperty<PlayerState> CurrentPlayerState = new ReactiveProperty<PlayerState>(PlayerState.Move);

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    private float originLensOrthoSize;
    private float zoomLensOrthoSize = 3.0f;
    private float zoomDuration = 0.5f;

    public bool isPause;
    public ReactiveProperty<bool> IsPause = new ReactiveProperty<bool>(false);

    [SerializeField]
    private Transform[] playerHpGaugeTrans;

    [SerializeField]
    private Transform[] enemyHpGaugeTrans;

    [SerializeField]
    private Image imgPlayerHpGauge;

    [SerializeField]
    private Image imgEnemyHpGauge;

    [SerializeField]
    private CanvasGroup playerCanvasGroup;

    [SerializeField]
    private CanvasGroup enemyCanvasGroup;

    private Tween tweenPlayerGauge;
    private Tween tweenEnemyGauge;

    [SerializeField]
    private GameObject[] battleEffectPrefabs;

    [SerializeField]
    private FloatingMessage floatingMessagePrefab;

    [SerializeField]
    private DropBoxBase coinPrefab;

    [SerializeField]
    private DropBoxBase foodPrefab;


    //mi 

    [SerializeField]
    private int hp;

    private int maxHp;

    [SerializeField]
    private int attackPower;

    //private int attackSpeed = 3;

    [SerializeField]
    private Test_0 dataBase;

    [SerializeField]
    private Treasure treasurePrefab;



    [SerializeField]
    private DropBoxBase potionPrefab;

    [SerializeField]
    private DropBoxBase coinBoxPrefab;

    [SerializeField]
    private DropBoxBase goalPrefab;

    public ReactiveProperty<int> Coin = new ReactiveProperty<int>();
    [SerializeField]
    private Text txtCoin;

    public ReactiveProperty<int> Food = new ReactiveProperty<int>();
    [SerializeField]
    private Text txtFood;
    [SerializeField]
    private Slider slider;

    public int maxFood;
    public int startFood;

    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private Button btnDash;

    
    private IEnumerator coroutine;
    private int fromAmountCoin;
    private int fromAmountFood;

    public bool isCoin;

    private UnityAction<int, bool> gaugeEvent;


    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out anim);

        this.UpdateAsObservable()
            .Where(_ => currentPlayerState == PlayerState.Move || CurrentPlayerState.Value == PlayerState.Move)
            .Subscribe(_ => 
            {
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
            .Subscribe(_ => 
            {
                Move();               
            }).AddTo(this);

        this.OnTriggerEnter2DAsObservable()
            .Where(_ => currentPlayerState == PlayerState.Move || CurrentPlayerState.Value == PlayerState.Move)
            .Subscribe(col => 
            {
                if (col.TryGetComponent(out ObstacleBase enemy)) {
                    StartCoroutine(AutoBattle(enemy));
                }
            }).AddTo(this);



        originLensOrthoSize = virtualCamera.m_Lens.OrthographicSize;


        // mi
        playerCanvasGroup.alpha = 0;
        enemyCanvasGroup.alpha = 0;

        playerCanvasGroup.transform.position = playerHpGaugeTrans[0].transform.position;
        enemyCanvasGroup.transform.position = enemyHpGaugeTrans[0].transform.position;

        maxHp = hp;

        Coin.Subscribe(x => txtCoin.DOCounter(x - fromAmountCoin, x, 0.5f)).AddTo(this);
        Food.Subscribe(x =>
        {
            txtFood.DOCounter(x - fromAmountFood, x, 0.5f);
            slider.DOValue(x, 0.5f);
        }).AddTo(this);

        slider.maxValue = maxFood;

        if (GameData.instance.stageNo > 0) {
            CalculateFood(GameData.instance.food);
            Coin.Value = GameData.instance.coin;
            hp = GameData.instance.hp;
            StartCoroutine(UpdatePlayerHpGauge(hp, true));
        } else {
            CalculateFood(startFood);
        }

        if (isCoin) {
            for (int  i = 0; i < Random.Range(3, 6); i++) {
                DropBoxBase coin = Instantiate(coinPrefab, new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
                //coin.SetUpDropBox();
            }
        }

        DropBoxBase goal = Instantiate(goalPrefab, new Vector3(transform.position.x + Random.Range(-5.0f, 5.0f), transform.position.y + Random.Range(-5.0f, 5.0f), 0), Quaternion.identity);
        //goal.SetUpDropBox();

        StartCoroutine(CountDownTimer());
    }
    
    //void Update()
    //{
    //    horizontal = Input.GetAxis("Horizontal");
    //    vertical = Input.GetAxis("Vertical");

    //    if (anim) {
    //        SyncMoveAnimation();
    //    }
    //}

    //void FixedUpdate() {
    //    if (rb) {
    //        Move();
    //    }
    //}

    /// <summary>
    /// 移動
    /// </summary>
    private void Move() {
        if (currentPlayerState == PlayerState.GameUp) {
            rb.velocity = Vector2.zero;
            return;
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
    /// カメラの LensOrthoSize を変更したズーム処理
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private IEnumerator ChangeCameraOrthoSize(float value) {
        DOTween.To(
            () => virtualCamera.m_Lens.OrthographicSize,
            x => virtualCamera.m_Lens.OrthographicSize = x,
            value,
            zoomDuration
            );
        yield return null;
    }


    //mi

    /// <summary>
    /// 自動バトル
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    private IEnumerator AutoBattle(ObstacleBase enemy) {

        Debug.Log("バトル開始");

        if (tweenPlayerGauge != null) {
            //imgPlayerHpGauge.DOKill(true);
            //imgEnemyHpGauge.DOKill(true);
            tweenPlayerGauge.Kill();
            tweenPlayerGauge = null;
        }
        if (tweenEnemyGauge != null) {
            tweenEnemyGauge.Kill();
            tweenEnemyGauge = null;
        }

        enemy.PrapareBattle();
        imgEnemyHpGauge.fillAmount = 1.0f;

        rb.velocity = Vector2.zero;
        currentPlayerState = PlayerState.Battle;

        MoveHpGaugePositions(1.0f, 1);

        // ズームイン
        yield return StartCoroutine(ChangeCameraOrthoSize(zoomLensOrthoSize));

        GameObject[] effect = new GameObject[battleEffectPrefabs.Length];
        for (int i = 0; i < battleEffectPrefabs.Length; i++) {
            effect[i] = Instantiate(battleEffectPrefabs[i], transform.position, battleEffectPrefabs[i].transform.rotation);
        }

        while (currentPlayerState == PlayerState.Battle) {

            // 一時停止
            if (isPause) {
                yield return null;
            }

            // カメラ振動
            impulseSource.GenerateImpulse();

            // 使用するスキル決定
            SkillData useSkillData = dataBase.GetUseRandomSkillData();
            Debug.Log("使用スキル : " + useSkillData.name);

            // アーティファクトによる攻撃速度を加算
            int totalAttackSpeed = useSkillData.attackSpeed;

            // 攻撃回数カウント
            dataBase.currentUseCount++;

            // 指定した回数攻撃を行ったら
            if (dataBase.currentUseCount > dataBase.currentWeaponData.useCount) {
                Debug.Log("武器進化 ボーナス獲得");

                // TODO ボーナスの候補
                // アーティファクト購入用のポイント獲得 => 任意のベース能力値アップ
                // 武器のレアリティ基準のアーティファクトを１つランダムで入手

            }

            // 攻撃順番確認
            if (totalAttackSpeed >= enemy.attackSpeed) {
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

        void AttackPlayer(SkillData skillData) {

            int totalAttackPower = skillData.attackPower;
            bool isCritical = JudgeCriticalHit();
            totalAttackPower *= isCritical ? 3 : 1;

            Debug.Log(totalAttackPower);

            enemy.hp -= totalAttackPower;
            Debug.Log("敵の残り HP : " + enemy.Hp);

            imgEnemyHpGauge.DOFillAmount((float)enemy.hp / enemy.maxHp, 0.25f).SetEase(Ease.InCirc);
            FloatingMessage enemyFloatingMessage = Instantiate(floatingMessagePrefab, enemyHpGaugeTrans[1].transform, false);
            enemyFloatingMessage.ShowMessage(-totalAttackPower, isCritical);

            if (enemy.hp <= 0) {
                currentPlayerState = PlayerState.Result;
            }

            /// <summary>
            /// クリティカル判定
            /// </summary>
            bool JudgeCriticalHit() {
                return skillData.criticalRate > Random.Range(0, 100) ? true : false;
            }
        }

        void AttackEnemy() {
            hp -= enemy.AttackPower;
            Debug.Log("プレイヤーの残り HP : " + hp);

            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = UpdatePlayerHpGauge(-enemy.AttackPower);
            StartCoroutine(coroutine);
            //imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);
            //FloatingMessage playerFloatingMessage = Instantiate(floatingMessagePrefab, playerHpGaugeTrans[1].transform, false);
            //playerFloatingMessage.ShowMessage(-enemy.AttackPower);

            if (hp <= 0) {
                currentPlayerState = PlayerState.GameUp;
            }
        }

        Debug.Log(currentPlayerState == PlayerState.Result ? "勝利" : "敗北");

        // バトルのエフェクトを破棄
        for (int i = 0; i < battleEffectPrefabs.Length; i++) {
            Destroy(effect[i]);
        }

        // ズームアウト
        StartCoroutine(ChangeCameraOrthoSize(originLensOrthoSize));

        // リザルト処理入れる(それまで hp を見せておく)
        if (currentPlayerState == PlayerState.Result) {

            (bool isDropTreasure, Rarity[] rarities) = enemy.JudgeDropTreasure();

            if (isDropTreasure) {

                WeaponData[] weaponDatas = new WeaponData[rarities.Length];

                weaponDatas = dataBase.GetWeaponDataByRarity(rarities);
                //Debug.Log(weaponDatas[0].name);

                Treasure treasure = Instantiate(treasurePrefab, new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
                //treasure.SetUpTreasure(weaponDatas);
            } else {
                // トレジャーが出ない場合には、お金か回復アイテムが出る(50%) 
                GenerateCoinOrPotion();
            }
        }

        enemy.DestroyObstacle();

        // トレジャー選択ウインドウが閉じるまで待機
        
        currentPlayerState = PlayerState.Move;

        yield return new WaitForSeconds(0.5f);
        if (currentPlayerState != PlayerState.Battle) {
            MoveHpGaugePositions(0.0f, 0);
        }
    }

    /// <summary>
    /// フード消費のカウントダウンタイマー
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountDownTimer() {

        float timer = 0;
        while (Food.Value > 0) {

            if (currentPlayerState == PlayerState.Move || currentPlayerState == PlayerState.Battle) {

                timer += Time.deltaTime;

                if (timer >= 1.0f) {
                    timer = 0;
                    CalculateFood(-1);
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Hpゲージの移動
    /// </summary>
    private void MoveHpGaugePositions(float alpha, int index, bool isPlayerOnly = false) {
        Sequence sequencePlayer = DOTween.Sequence();

        tweenPlayerGauge = sequencePlayer.Append(playerCanvasGroup.DOFade(alpha, 0.5f));
        sequencePlayer.Join(playerCanvasGroup.transform.DOMoveX(playerHpGaugeTrans[index].transform.position.x, 0.5f)).OnComplete(() => tweenPlayerGauge.Kill());

        if (!isPlayerOnly) {
            Sequence sequenceEnemy = DOTween.Sequence();
            tweenEnemyGauge = sequenceEnemy.Append(enemyCanvasGroup.DOFade(alpha, 0.5f));
            sequenceEnemy.Join(enemyCanvasGroup.transform.DOMoveX(enemyHpGaugeTrans[index].transform.position.x, 0.5f)).OnComplete(() => tweenEnemyGauge.Kill());
        }
    }

    /// <summary>
    /// 武器選択ウインドウ表示
    /// </summary>
    /// <param name="weaponDatas"></param>
    public void ShowWeaponSelectPopUp(WeaponData[] weaponDatas) {
        Debug.Log("トレジャー取得");

        currentPlayerState = PlayerState.Info;
        rb.velocity = Vector2.zero;

        // 3種類の中から１つを抽出し、スキルリストを作成
        WeaponData newWeaponData = weaponDatas[Random.Range(0, weaponDatas.Length)];
        newWeaponData.skillDatasList = dataBase.GetWeaponSkillDatas(newWeaponData.skillNos);

        // トレジャー選択ウインドウ表示
        dataBase.ShowWeaponSelectPopUp(newWeaponData);
    }

    /// <summary>
    /// Hpゲージ更新
    /// </summary>
    /// <param name="amout"></param>
    /// <returns></returns>
    private IEnumerator UpdatePlayerHpGauge(int amout, bool isGain = false) {
        MoveHpGaugePositions(1.0f, 1, true);
        yield return new WaitForSeconds(0.5f);

        imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);
        FloatingMessage playerFloatingMessage = Instantiate(floatingMessagePrefab, playerHpGaugeTrans[1].transform, false);
        playerFloatingMessage.ShowMessage(amout, false, isGain);

        yield return new WaitForSeconds(1.5f);
        MoveHpGaugePositions(0.0f, 0);
    }

    /// <summary>
    /// コイン計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateCoin(int amount) {

        fromAmountCoin = amount;
        Coin.Value += amount;
        //txtCoin.DOCounter(Coin.Value, Coin.Value += amout, 0.5f).SetEase(Ease.Linear);

        Debug.Log("コイン更新");
    }

    /// <summary>
    /// サーバーセーブ用。SendScoreAndShowRanking メソッドの第2引数に ID 番号としてキャストして使う
    /// RankingBoards スクリプタブル・オブジェクトの配列の Element 番号が ID 番号として識別される
    /// </summary>
    public enum SaveType {
        TotalCoins,
        ClearStageCount
    }

    /// <summary>
    /// フード計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalculateFood(int amount) {
        fromAmountFood = amount;
        Food.Value = Mathf.Clamp(Food.Value += amount, 0, maxFood);

        if (Food.Value <= 0) {
            currentPlayerState = PlayerState.GameUp;
            Debug.Log("ゲームオーバー");

            // サーバーにセーブ
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking(GameData.instance.coin + Coin.Value, (int)SaveType.TotalCoins);
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking(GameData.instance.stageNo, (int)SaveType.ClearStageCount);
        }
        Debug.Log("フード更新");
    }

    /// <summary>
    /// ライフ計算
    /// </summary>
    /// <param name="amout"></param>
    public void GainLife(int amout) {

        hp = Mathf.Clamp(hp += amout, 0, maxHp);

        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = UpdatePlayerHpGauge(amout, true);
        StartCoroutine(coroutine);
        Debug.Log("体力回復");
    }

    /// <summary>
    /// お金かポーションかフードを生成
    /// </summary>
    private void GenerateCoinOrPotion() {
        DropBoxBase itemPrefab = Random.Range(0, 100) > 50 ? coinPrefab : Random.Range(0, 100) > 50 ? potionPrefab : foodPrefab;
        DropBoxBase item = Instantiate(itemPrefab, new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
        //item.SetUpDropBox();
    }

    /// <summary>
    /// Button の EventTrigger(PointerDown) に登録
    /// </summary>
    public void DashOn() {
        isDash = true;
    }

    /// <summary>
    /// Button の EventTrigger(PointerUp) に登録
    /// </summary>
    public void DashOff() {
        isDash = false;
    }

    /// <summary>
    /// 次のステージへ遷移する準備
    /// </summary>
    /// <param name="bonusPoint"></param>

    public void PrepareNextStage(int bonusPoint) {
        Debug.Log("ステージクリア");

        currentPlayerState = PlayerState.GameUp;
        rb.velocity = Vector2.zero;

        GameData.instance.coin = Coin.Value;
        GameData.instance.food = Food.Value + bonusPoint;
        GameData.instance.hp = hp;

        // ズームイン
        StartCoroutine(ChangeCameraOrthoSize(zoomLensOrthoSize));

        dataBase.NextStage();
    }
}
