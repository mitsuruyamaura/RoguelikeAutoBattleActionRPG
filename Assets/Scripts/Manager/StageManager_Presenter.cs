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

    [SerializeField]
    private TopUI_View uiManager;

    [SerializeField]
    private WeaponSelectPopUp weaponSelectPopUpPrefab;
    [SerializeField]
    private Transform canvasTran;

    private WeaponSelectPopUp weaponSelectPopUp;


    void Start() {
        // ���[�U�[����������Ă��Ȃ��ꍇ�ɂ́A���[�U�[���쐬
        if (UserDataManager.instance.User == null) {
            UserDataManager.instance.User = User.CreateUser(60, 0, 1);
        }

        // �R�C���ƃt�[�h�̍w��
        UserDataManager.instance.User.Coin
            .Zip(UserDataManager.instance.User.Coin.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => uiManager.UpdateDisplayCoin(x.oldValue, x.newValue)).AddTo(this);

        UserDataManager.instance.User.Food
            .Zip(UserDataManager.instance.User.Food.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => {
                uiManager.UpdateDisplayFood(x.oldValue, x.newValue);

                if (UserDataManager.instance.User.Food.Value <= 0) {
                    playerController.CurrentPlayerState.Value = PlayerController.PlayerState.GameUp;
                }
            }).AddTo(this);

        UserDataManager.instance.User.Coin.SetValueAndForceNotify(UserDataManager.instance.User.Coin.Value);
        UserDataManager.instance.User.Food.SetValueAndForceNotify(UserDataManager.instance.User.Food.Value);

        // �L�����N�^�[�̐ݒ�B��������Ă��Ȃ��ꍇ�ɂ́A�L�����N�^�[���쐬
        if (UserDataManager.instance.CurrentCharacter == null) {
            UserDataManager.instance.CurrentCharacter = Character.CreateChara();

            // HP �ݒ�
            UserDataManager.instance.Hp.Value = UserDataManager.instance.CurrentCharacter.maxHp;
        }

        // ��������̐ݒ�B�ݒ肳��Ă��Ȃ��̂ݐݒ�
        if (UserDataManager.instance.CurrentWeapon == null) {
            UserDataManager.instance.SetUpWeapon();
        }

        hpGaugeView.UpdatePlayerHpGauge(UserDataManager.instance.Hp.Value, UserDataManager.instance.CurrentCharacter.maxHp, UserDataManager.instance.Hp.Value);

        // �v���C���[�̃X�e�[�g���w�ǂ��A�o�g�������U���g���ɃJ�����̃Y�[�����s��(ReactiveProperty�̏ꍇ)
        playerController.CurrentPlayerState
            .Where(x => x == PlayerController.PlayerState.Battle_Before || x == PlayerController.PlayerState.Battle_After || x == PlayerController.PlayerState.GameUp)
            .Subscribe(state => {
                // �J�����̃Y�[��
                StartCoroutine(cameraController.ChangeCameraOrthoSize(state));

                if (state != PlayerController.PlayerState.GameUp) {
                    // Hp�Q�[�W�̓���m�F�ƈړ�
                    hpGaugeView.PrepareCheckHpGaugeState();

                    (float alpha, int index) value = state == PlayerController.PlayerState.Battle_Before ? (1.0f, 1) : (0.0f, 0);
                    hpGaugeView.MoveHpGaugePositions(value.alpha, value.index);
                    //Debug.Log("�Q�[�W�ړ�");
                }
            }).AddTo(playerController.gameObject);

        // �v���C���[�̃X�e�[�g���w�ǂ��A�o�g�������U���g���ɃJ�����̃Y�[�����s��(ObserveEveryValueChanged�̏ꍇ)
        // �ʏ�̃N���X�̕ϐ��ɑ΂��ė��p�ł��镔�����|�C���g
        // �������A�t���[���Ԃ̕����̒l�̕ϓ��͎�ꂸ�AReactiveProperty�����d���̂ŁA��ʂ��l���Ďg��
        //playerController.ObserveEveryValueChanged(playerController => playerController.currentPlayerState)
        //    .Where(x => x == PlayerController.PlayerState.Battle_Before || x == PlayerController.PlayerState.Battle_After)
        //    .Subscribe(state => {
        //        // �J�����̃Y�[��
        //        StartCoroutine(cameraController.ChangeCameraOrthoSize(state));

        //        // Hp�Q�[�W�̓���m�F�ƈړ�
        //        hpGaugeView.PrepareCheckHpGaugeState();

        //        (float alpha, int index) value = state == PlayerController.PlayerState.Battle_Before ? (1.0f, 1) : (0.0f, 0);
        //        hpGaugeView.MoveHpGaugePositions(value.alpha, value.index);
        //        //Debug.Log("�Q�[�W�ړ�");

        //    }).AddTo(playerController.gameObject);

        // �v���C���[�̃|�[�Y��Ԃ��w�ǂ��A�o�g�������|�[�Y�łȂ��ꍇ�ɂ̓J������U��������
        playerController.IsPause
            .CombineLatest(playerController.CurrentPlayerState, (pause, state) => (pause, state)) // ���������� ReactiveProperty ������
            .Where(x => x.state == PlayerController.PlayerState.Battle_Before && !x.pause)
            .Subscribe(_ => cameraController.ImpulseCamera())
            .AddTo(playerController.gameObject);

        // �v���C���[�� HP �̍w��
        UserDataManager.instance.Hp
            .Zip(UserDataManager.instance.Hp.Skip(1), (oldValue, newValue) => new { oldValue, newValue })
            .Subscribe(x => {
                // Hp�Q�[�W�X�V
                hpGaugeView.UpdatePlayerHpGauge(x.newValue, UserDataManager.instance.CurrentCharacter.maxHp, x.newValue - x.oldValue);

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
                    if (x.newValue <= 0 || x.newValue <= 0) {
                        obstaclesList[index].DestroyObstacle();
                        obstaclesList.Remove(obstaclesList[index]);
                    }
                }).AddTo(obstaclesList[index].gameObject);
        }

        // �h���b�v�A�C�e���̎擾
        playerController.OnTriggerEnter2DAsObservable()
            .Subscribe(col => {
                if (col.TryGetComponent(out DropBoxBase dropItem)) {
                    dropItem.TriggerDropBoxEffect(this);
                }
            }).AddTo(playerController.gameObject);

        // ����擾���̃|�b�v�A�b�v�̐���
        weaponSelectPopUp = Instantiate(weaponSelectPopUpPrefab, canvasTran, false);
        weaponSelectPopUp.SetUpPopUp(UserDataManager.instance.CurrentWeapon, this);
    }

    /// <summary>
    /// �X�e�[�W�N���A�����ƁA���̃X�e�[�W�֑J�ڂ��鏀��
    /// </summary>
    /// <param name="bonusPoint"></param>
    public void ClearStage(int bonusPoint) {
        Debug.Log("�X�e�[�W�N���A");

        playerController.currentPlayerState = PlayerController.PlayerState.GameUp;

        // �t�[�h�̏��� UserDataManager �ɕێ�
        UserDataManager.instance.CalculateFood(bonusPoint);

        SceneStateManager.instance.PrepareNextScene(SceneName.Main);
    }

    /// <summary>
    /// ����I���E�C���h�E�\��
    /// </summary>
    /// <param name="weaponDatas"></param>
    public void ShowWeaponSelectPopUp(WeaponData[] weaponDatas) {
        Debug.Log("�g���W���[�擾");

        playerController.CurrentPlayerState.Value = PlayerController.PlayerState.Info;
        playerController.currentPlayerState = PlayerController.PlayerState.Info;
        playerController.StopMove();

        // 3��ނ̒�����P�𒊏o���A�X�L�����X�g���쐬
        WeaponData newWeaponData = weaponDatas[Random.Range(0, weaponDatas.Length)];
        newWeaponData.skillDatasList = DataBaseManager.instance.GetWeaponSkillDatas(newWeaponData.skillNos);

        // �g���W���[�I���E�C���h�E�\��
        weaponSelectPopUp.ShowPopUp(newWeaponData, UserDataManager.instance.CurrentWeapon, UserDataManager.instance.currentUseCount);

        // ��Q���S�̂̈ړ����ꎞ��~
        if (obstaclesList.Count > 0) {
            PauseObstacles();
        }
    }

    /// <summary>
    /// �|�[�Y��Ԃ̐؂�ւ�
    /// </summary>
    public void ResumeGame() {
        // �v���[���[�̑�����͂��ĊJ
        playerController.CurrentPlayerState.Value = PlayerController.PlayerState.Move;
        playerController.currentPlayerState = PlayerController.PlayerState.Move;

        // ��Q���S�̂̈ړ����ĊJ
        if (obstaclesList.Count > 0) {
            ResumeObstacles();
        }
    }

    /// <summary>
    /// ��Q���S�̂̈ړ����ꎞ��~
    /// </summary>
    public void PauseObstacles() {
        for (int i = 0; i < obstaclesList.Count; i++) {
            if (obstaclesList[i].gameObject.TryGetComponent(out EnemyController enemyController)) {
                enemyController.PauseMove();
            }
        }
    }

    /// <summary>
    /// ��Q���S�̂̈ړ����ĊJ
    /// </summary>
    public void ResumeObstacles() {
        for (int i = 0; i < obstaclesList.Count; i++) {
            if (obstaclesList[i].gameObject.TryGetComponent(out EnemyController enemyController)) {
                enemyController.ResumeMove();
            }
        }
    }
}
