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

    [SerializeField]                              // Model ReactiveCollection �ɕς���
    private List<ObstacleBase> obstaclesList = new List<ObstacleBase>();  // TOD0 �폜����������

    [SerializeField, Header("��Q���̏d�ݕt��")]
    private int[] obstacleWeights;   // TODO �X�e�[�W�̃f�[�^������炤�悤�ɂ���

    [SerializeField]
    private int generateCount;


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

            }).AddTo(playerController.gameObject);

        // �v���C���[�̃|�[�Y��Ԃ��w�ǂ��A�o�g�������|�[�Y�łȂ��ꍇ�ɂ̓J������U��������
        playerController.IsPause.Where(_ => playerController.CurrentPlayerState.Value == PlayerController.PlayerState.Battle_Before && !playerController.IsPause.Value)
            .Subscribe(_ => cameraController.ImpulseCamera())
            .AddTo(playerController.gameObject);

        // �v���C���[�� HP �̍w��
        UserDataManager.instance.Hp
            .Zip(UserDataManager.instance.Hp.Skip(1), (oldValue, newValue) => new { oldValue, newValue })
            .Subscribe(x => {
                // Hp�Q�[�W�X�V
                hpGaugeView.UpdatePlayerHpGauge(x.newValue, UserDataManager.instance.currentCharacter.maxHp, x.newValue - x.oldValue);

                if (UserDataManager.instance.Hp.Value <= 0) {
                    playerController.CurrentPlayerState.Value = PlayerController.PlayerState.GameUp;
                }
            })
            .AddTo(this);

        // TODO ��Q���̐����� List �ւ̓o�^
        obstaclesList = obstacleGenerator.GenerateRandomObstacles(obstacleWeights, generateCount);

        // ��Q���� HP �̍w��
        for (int i = 0; i < obstaclesList.Count; i++) {
            int index = i;

            // ��Q�����j�󂳂ꂽ���_�ōw�ǂ��~�߂�
            obstaclesList[index].Hp
                .Zip(obstaclesList[index].Hp.Skip(1), (oldValue, newValue) => new { oldValue, newValue })
                .Subscribe(x => {
                    hpGaugeView.UpdateObstacleHpGauge(x.newValue, obstaclesList[index].maxHp, x.newValue - x.oldValue);

                    // TODO List ���甲���������ASkip �����邽�߁A��肭����ł��Ȃ��B�ʂ̕��@����������
                    // ���P�Bindex ���Y����B������Ă��Ă��Y����
                    // ���Q�BSkip �̉e���ŁA�P�񕪑ҋ@������AHP 0 �̃^�C�~���O�ŌĂ΂�Ȃ����Ƃ�����
                    //if (x.newValue <= 0) {
                    //    obstaclesList[index].DestroyObstacle();
                    //    obstaclesList.Remove(obstaclesList[index]);                        
                    //}
                }).AddTo(obstaclesList[index].gameObject);
        }
    }
}
