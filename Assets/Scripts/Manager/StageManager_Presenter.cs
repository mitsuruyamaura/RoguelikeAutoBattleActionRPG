using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StageManager_Presenter : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;    // Model

    [SerializeField]
    private CameraController cameraController;    // View

    [SerializeField]
    private HpGauge_View hpGaugeView;

    [SerializeField]
    private ObstacleGenerator obstacleGenerator;

    [SerializeField]                              // Model ReactiveCollection に変える
    private List<ObstacleBase> obstaclesList = new List<ObstacleBase>();  // TOD0 削除処理を入れる

    [SerializeField, Header("障害物の重み付け")]
    private int[] obstacleWeights;   // TODO ステージのデータからもらうようにする

    [SerializeField]
    private int generateCount;


    void Start() {
        // ユーザーが生成されていない場合には、ユーザーを作成
        //if(UserDataManager.instance.user != null) {
        //    UserDataManager.instance.user = User.CreateUser(60, 0, 1);
        //}

        // キャラクターの設定。生成されていない場合には、キャラクターを作成
        if (UserDataManager.instance.currentCharacter != null) {
            UserDataManager.instance.currentCharacter = Character.CreateChara();
        }

        // HP 設定
        UserDataManager.instance.Hp.Value = UserDataManager.instance.currentCharacter.maxHp;

        // プレイヤーのステートを購読し、バトルかリザルト時にカメラのズームを行う
        playerController.CurrentPlayerState
            .Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before || playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_After)
            .Subscribe(_ => {
                // カメラのズーム
                StartCoroutine(cameraController.ChangeCameraOrthoSize(playerController.CurrentPlayerState.Value));

                // Hpゲージの動作確認と移動
                hpGaugeView.PrepareCheckHpGaugeState();

                (float alpha, int index) value = playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before ? (1.0f, 1) : (0.0f, 0);
                hpGaugeView.MoveHpGaugePositions(value.alpha, value.index);
                //Debug.Log("ゲージ移動");

            }).AddTo(playerController.gameObject);

        // プレイヤーのポーズ状態を購読し、バトル時かつポーズでない場合にはカメラを振動させる
        playerController.IsPause.Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before && !playerController.IsPause.Value)
            .Subscribe(_ => cameraController.ImpulseCamera())
            .AddTo(playerController.gameObject);

        // プレイヤーの HP の購読
        UserDataManager.instance.Hp
            .Zip(UserDataManager.instance.Hp.Skip(1), (oldValue, newValue) => new { oldValue, newValue })
            .Subscribe(x => {
                // Hpゲージ更新
                hpGaugeView.UpdatePlayerHpGauge(x.newValue, UserDataManager.instance.currentCharacter.maxHp, x.newValue - x.oldValue);

                if (UserDataManager.instance.Hp.Value <= 0) {
                    playerController.CurrentPlayerState.Value = PlayerController.PlayerState.GameUp;
                }
            })
            .AddTo(this);

        // TODO 障害物の生成と List への登録
        obstaclesList = obstacleGenerator.GenerateRandomObstacles(obstacleWeights, generateCount);

        // 障害物の HP の購読
        for (int i = 0; i < obstaclesList.Count; i++) {
            int index = i;

            // 障害物が破壊された時点で購読を止める
            obstaclesList[index].Hp
                .Zip(obstaclesList[index].Hp.Skip(1), (oldValue, newValue) => new { oldValue, newValue })
                .Subscribe(x => {
                    hpGaugeView.UpdateObstacleHpGauge(x.newValue, obstaclesList[index].maxHp, x.newValue - x.oldValue);

                    // TODO List から抜きたいが、Skip があるため、上手く制御できない。別の方法を検討する
                    // 問題１。index がズレる。代入していてもズレる
                    // 問題２。Skip の影響で、１回分待機が入り、HP 0 のタイミングで呼ばれないことがある
                    //if (x.newValue <= 0) {
                    //    obstaclesList[index].DestroyObstacle();
                    //    obstaclesList.Remove(obstaclesList[index]);                        
                    //}
                }).AddTo(obstaclesList[index].gameObject);
        }
    }
}
