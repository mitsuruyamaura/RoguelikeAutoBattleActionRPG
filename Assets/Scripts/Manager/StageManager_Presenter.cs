using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StageManager_Presenter : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private CameraController cameraController;


    void Start()
    {
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
            .Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle || playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Result)
            .Subscribe(_ => {
                StartCoroutine(cameraController.ChangeCameraOrthoSize(PlayerController.PlayerState.Battle));
            }).AddTo(this);

        // ポーズ状態を購読し、バトル時かつポーズでない場合にはカメラを振動させる
        playerController.IsPause.Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle && !playerController.IsPause.Value)
            .Subscribe(_ => cameraController.ImpulseCamera())
            .AddTo(this);

        UserDataManager.instance.Hp.Where(_ => UserDataManager.instance.Hp.Value <= 0)
            .Subscribe(_ => playerController.CurrentPlayerState.Value = PlayerController.PlayerState.GameUp)
            .AddTo(this);
    }
}
