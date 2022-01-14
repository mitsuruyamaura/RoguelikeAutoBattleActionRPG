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

    [SerializeField]
    private HpGauge_View hpGaugeView;

    [SerializeField]// ReactiveCollection �ɕς���
    private List<ObstacleBase> obstaclesList = new List<ObstacleBase>();  // TOD0 �폜����������


    void Start() {
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
            .Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before || playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_After)
            .Subscribe(_ => {
                // �J�����̃Y�[��
                StartCoroutine(cameraController.ChangeCameraOrthoSize(playerController.CurrentPlayerState.Value));

                // Hp�Q�[�W�̓���m�F�ƈړ�
                hpGaugeView.PrepareCheckHpGaugeState();

                (float alpha, int index) value = playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before ? (1.0f, 1) : (0.0f, 0);
                hpGaugeView.MoveHpGaugePositions(value.alpha, value.index);
                //Debug.Log("�Q�[�W�ړ�");

            }).AddTo(this);

        // �|�[�Y��Ԃ��w�ǂ��A�o�g�������|�[�Y�łȂ��ꍇ�ɂ̓J������U��������
        playerController.IsPause.Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before && !playerController.IsPause.Value)
            .Subscribe(_ => cameraController.ImpulseCamera())
            .AddTo(this);

        UserDataManager.instance.Hp
            .Subscribe(x => {
                // Hp�Q�[�W�X�V
                hpGaugeView.UpdatePlayerHpGauge(x, UserDataManager.instance.currentCharacter.maxHp);
                
                if (UserDataManager.instance.Hp.Value <= 0) {
                    playerController.CurrentPlayerState.Value = PlayerController.PlayerState.GameUp;
                }
            })
            .AddTo(this);

        // ��Q���� HP �̍w��
        for (int i = 0; i < obstaclesList.Count; i++) {
            int index = i;
            obstaclesList[index].Hp.Subscribe(x => hpGaugeView.UpdateObstacleHpGauge(x, obstaclesList[index].maxHp)).AddTo(obstaclesList[index].gameObject);
        }
    }
}
