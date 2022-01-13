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

    public ReactiveProperty<PlayerState> CurrentPlayerState = new ReactiveProperty<PlayerState>(PlayerState.Move);
    public ReactiveProperty<bool> IsPause = new ReactiveProperty<bool>(false);


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

        this.OnTriggerEnter2DAsObservable()
            .Subscribe(col => {
            if (col.TryGetComponent(out ObstacleBase enemy)) {
                StartCoroutine(AutoBattle(enemy));
            }
        }).AddTo(this);
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    private void Move() {
        Vector2 dir = new Vector3(horizontal, vertical).normalized;
        float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * 2.0f : moveSpeed;
        speed = isDash ? moveSpeed * 2.0f : moveSpeed;

        rb.velocity = dir * speed;
    }

    /// <summary>
    /// �ړ���������ƈړ��A�j���̓���
    /// </summary>
    private void SyncMoveAnimation() {
        if (!Mathf.Approximately(horizontal, 0.0f) || !Mathf.Approximately(vertical, 0.0f)) {
            lookDirection.Set(horizontal, vertical);
            lookDirection.Normalize();

            anim.SetFloat("Look X", lookDirection.x);
            anim.SetFloat("Look Y", lookDirection.y);

            // �_�b�V���L���ɉ����ăA�j���̍Đ����x�𒲐�
            anim.SetFloat("Speed", isDash ? 2 : lookDirection.sqrMagnitude);
        } else {
            anim.SetFloat("Speed", 0);
        }
    }

    /// <summary>
    /// �����o�g��
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    private IEnumerator AutoBattle(ObstacleBase enemy) {

        Debug.Log("�o�g���J�n");

        // �o���� Hp �Q�[�W���\�������m�F�@�A�j�����Ȃ��~


        enemy.PrapareBattle();

        // �G��Hp �Q�[�W���ő�l�ɂ���

        // �v���C���[�̈ړ����~
        rb.velocity = Vector2.zero;

        // �X�e�[�g��ύX���āA�ړ��̓��͂��󂯕t���Ȃ��悤�ɂ�����
        currentPlayerState = PlayerState.Battle;

        // �o���� Hp �Q�[�W����ʂɈړ����ĕ\��

        // �Y�[���C��(�����ōw�ǂ�����̂ŁA�s�v)

        // �o�g�����̃G�t�F�N�g�\��

        // �o�g���Ď�
        while (currentPlayerState == PlayerState.Battle) {

            // �ꎞ��~
            if (IsPause.Value) {
                yield return null;
            }

            // �J�����U��(�����ōw�ǂ�����̂ŁA�s�v)

            // �g�p����X�L������

            // TODO �A�[�e�B�t�@�N�g�ɂ��U�����x�����Z
            int totalAttackSpeed = Random.Range(3, 10);

            // �U���񐔃J�E���g


            // �w�肵���񐔍U�����s������


            // TODO �{�[�i�X�̌��
            // �A�[�e�B�t�@�N�g�w���p�̃|�C���g�l�� => �C�ӂ̃x�[�X�\�͒l�A�b�v
            // ����̃��A���e�B��̃A�[�e�B�t�@�N�g���P�����_���œ���



            // �U�����Ԋm�F
            if (totalAttackSpeed >= enemy.attackSpeed) {
                AttackPlayer(); // TODO SkillData �n��
                yield return new WaitForSeconds(0.25f);
                AttackEnemy();
            } else {
                AttackEnemy();
                yield return new WaitForSeconds(0.25f);
                AttackPlayer(); // TODO SkillData �n��
            }
            yield return new WaitForSeconds(0.25f);
        }

        void AttackPlayer(SkillData skillData = null) {

            // �X�L���f�[�^�����邩�Ȃ����ŏ�����ύX
            int totalAttackPower = skillData != null ? skillData.attackPower : Random.Range(1, 4);
            bool isCritical = skillData != null ? JudgeCriticalHit() : Random.Range(0, 2) == 0 ? true : false;
            totalAttackPower *= isCritical ? 3 : 1;

            Debug.Log(totalAttackPower);

            enemy.Hp -= totalAttackPower;
            Debug.Log("�G�̎c�� HP : " + enemy.Hp);

            // Hp �Q�[�W�̓���
            
            // �t���[�e�B���O���b�Z�[�W�̐���

            if (enemy.Hp <= 0) {
                currentPlayerState = PlayerState.Result;
            }

            /// <summary>
            /// �N���e�B�J������
            /// </summary>
            bool JudgeCriticalHit() {
                return skillData.criticalRate > Random.Range(0, 100) ? true : false;
            }
        }

        void AttackEnemy() {
            UserDataManager.instance.Hp.Value -= enemy.AttackPower;
            Debug.Log("�v���C���[�̎c�� HP : " + UserDataManager.instance.Hp.Value);

            // HP �Q�[�W���\������Ă���ꍇ�ɂ́A�Q�[�W�̈ړ��������~�߂�


            // �Q�[�W�̈ړ��Ɠ���

            //imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);
            //FloatingMessage playerFloatingMessage = Instantiate(floatingMessagePrefab, playerHpGaugeTrans[1].transform, false);
            //playerFloatingMessage.ShowMessage(-enemy.AttackPower);

            // Hp �� 0 �ȉ����ǂ������肵�ăX�e�[�g�ύX(�w�ǂ��Ă���̂ŏ����s�v)
        }

        Debug.Log(currentPlayerState == PlayerState.Result ? "����" : "�s�k");

        // �o�g���̃G�t�F�N�g��j��

        // �Y�[���A�E�g(�����ōw�ǂ�����̂ŁA�s�v)

        // ���U���g���������(����܂� hp �������Ă���)
        if (currentPlayerState == PlayerState.Result) {

           // �h���b�v�A�C�e�������邩����

        }

        enemy.DestroyObstacle();

        // �g���W���[�I���E�C���h�E������܂őҋ@

        currentPlayerState = PlayerState.Move;

        yield return new WaitForSeconds(0.5f);

        // HP �Q�[�W����ʊO�ֈړ�
        
    }

    /***** UniRX ���g��Ȃ��ꍇ  *****/

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
