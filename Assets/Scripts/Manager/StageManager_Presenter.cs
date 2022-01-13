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
        // ���[�U�[����������Ă��Ȃ��ꍇ�ɂ́A���[�U�[���쐬
        //if(UserDataManager.instance.user != null) {
        //    UserDataManager.instance.user = User.CreateUser(60, 0, 1);
        //}

        // �L�����N�^�[�̐ݒ�B��������Ă��Ȃ��ꍇ�ɂ́A�L�����N�^�[���쐬
        if (UserDataManager.instance.currentCharacter != null) {
            UserDataManager.instance.currentCharacter = Character.CreateChara();
        }

        // HP �ݒ�
        UserDataManager.instance.Hp.Value = UserDataManager.instance.currentCharacter.maxHp;

        // �v���C���[�̃X�e�[�g���w�ǂ��A�o�g�������U���g���ɃJ�����̃Y�[�����s��
        playerController.CurrentPlayerState
            .Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle || playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Result)
            .Subscribe(_ => {
                StartCoroutine(cameraController.ChangeCameraOrthoSize(PlayerController.PlayerState.Battle));
            }).AddTo(this);

        // �|�[�Y��Ԃ��w�ǂ��A�o�g�������|�[�Y�łȂ��ꍇ�ɂ̓J������U��������
        playerController.IsPause.Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle && !playerController.IsPause.Value)
            .Subscribe(_ => cameraController.ImpulseCamera())
            .AddTo(this);

        UserDataManager.instance.Hp.Where(_ => UserDataManager.instance.Hp.Value <= 0)
            .Subscribe(_ => playerController.CurrentPlayerState.Value = PlayerController.PlayerState.GameUp)
            .AddTo(this);
    }
}
