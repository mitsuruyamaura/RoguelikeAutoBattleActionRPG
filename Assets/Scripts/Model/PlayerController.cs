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
    /// �ړ�
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
    /// <param name="obstacle"></param>
    /// <returns></returns>
    private IEnumerator AutoBattle(ObstacleBase obstacle) {

        Debug.Log("�o�g���J�n");

        // �o���� Hp �Q�[�W���\�������m�F�@�A�j�����Ȃ��~(�����ōw�ǂ�����̂ŁA�s�v)

        obstacle.PrapareBattle();

        // ��Q����Hp �Q�[�W���ő�l�ɂ���(�����ōw�ǂ�����̂ŁA�s�v)

        // �v���C���[�̈ړ����~
        rb.velocity = Vector2.zero;

        // �X�e�[�g��ύX���āA�ړ��̓��͂��󂯕t���Ȃ��悤�ɂ���
        currentPlayerState = PlayerState.Battle_Before;
        CurrentPlayerState.Value = PlayerState.Battle_Before;

        // �o���� Hp �Q�[�W����ʂɈړ����ĕ\��(�����ōw�ǂ�����̂ŁA�s�v)

        // �Y�[���C��(�����ōw�ǂ�����̂ŁA�s�v)

        // �o�g�����̃G�t�F�N�g�\��
        EffectBase[] effect = new EffectBase[EffectManager.instance.GetEffects(EffectType.Battle).Length];
        for (int i = 0; i < effect.Length; i++) {
            effect[i] = Instantiate(EffectManager.instance.GetEffects(EffectType.Battle)[i], transform.position, EffectManager.instance.GetEffects(EffectType.Battle)[i].transform.rotation);
        }

        // �o�g���Ď�
        while (currentPlayerState == PlayerState.Battle_Before || CurrentPlayerState.Value == PlayerState.Battle_Before) {

            // �ꎞ��~
            if (IsPause.Value) {
                yield return null;
            }

            // �����l��ʒm
            IsPause.SetValueAndForceNotify(false);

            // �J�����U��(�����ōw�ǂ�����̂ŁA�s�v)

            // �g�p����X�L������(�X�L���̔ԍ����w�肷��ƁA���̃X�L���𗘗p�B����ȊO�̓����_��)
            SkillData useSkillData = UserDataManager.instance.GetUseSkillData();
            Debug.Log("�g�p�X�L�� : " + useSkillData.name);

            // TODO �A�[�e�B�t�@�N�g�ɂ��U�����x�����Z
            int totalAttackSpeed = Random.Range(3, 10);

            // �U���񐔃J�E���g
            UserDataManager.instance.currentUseCount++;

            // TODO �w�肵���񐔍U�����s������(�w�ǂ���`�ɕς���)
            
            // TODO �{�[�i�X�̌��
            // �A�[�e�B�t�@�N�g�w���p�̃|�C���g�l�� => �C�ӂ̃x�[�X�\�͒l�A�b�v
            // ����̃��A���e�B��̃A�[�e�B�t�@�N�g���P�����_���œ���


            // �U�����Ԋm�F
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

            // �X�L���f�[�^�����邩�Ȃ����ŏ�����ύX
            int totalAttackPower = skillData != null ? skillData.attackPower : Random.Range(1, 4);
            bool isCritical = skillData != null ? JudgeCriticalHit() : Random.Range(0, 2) == 0 ? true : false;
            totalAttackPower *= isCritical ? 3 : 1;

            Debug.Log(totalAttackPower);

            obstacle.Hp.Value -= totalAttackPower;
            Debug.Log("��Q���̎c�� HP : " + obstacle.Hp);

            // Hp �Q�[�W�̓���(�����ōw�ǂ�����̂ŁA�s�v)

            // �t���[�e�B���O���b�Z�[�W�̐���(�����ōw�ǂ�����̂ŁA�s�v)

            if (obstacle.Hp.Value <= 0) {
                currentPlayerState = PlayerState.Result;
                CurrentPlayerState.Value = PlayerState.Result;
            }

            /// <summary>
            /// �N���e�B�J������
            /// </summary>
            bool JudgeCriticalHit() {
                return skillData.criticalRate > Random.Range(0, 100) ? true : false;
            }
        }

        void AttackEnemy() {
            UserDataManager.instance.Hp.Value -= obstacle.AttackPower;
            Debug.Log("�v���C���[�̎c�� HP : " + UserDataManager.instance.Hp.Value);

            // HP �Q�[�W���\������Ă���ꍇ�ɂ́A�Q�[�W�̈ړ��������~�߂�

            // �Q�[�W�̈ړ��Ɠ���(�����ōw�ǂ�����̂ŁA�s�v)

            //imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);
            //FloatingMessage playerFloatingMessage = Instantiate(floatingMessagePrefab, playerHpGaugeTrans[1].transform, false);
            //playerFloatingMessage.ShowMessage(-enemy.AttackPower);

            // Hp �� 0 �ȉ����ǂ������肵�ăX�e�[�g�ύX(�w�ǂ��Ă���̂ŏ����s�v)
        }

        //Debug.Log(currentPlayerState == PlayerState.Result ? "����" : "�s�k");
        Debug.Log(CurrentPlayerState.Value == PlayerState.Result ? "����" : "�s�k");

        // �o�g���̃G�t�F�N�g��j��
        for (int i = 0; i < effect.Length; i++) {
            Destroy(effect[i].gameObject);
        }

        // �Y�[���A�E�g(�����ōw�ǂ�����̂ŁA�s�v)

        // ���U���g���������(����܂� hp �������Ă���)
        if (currentPlayerState == PlayerState.Result || CurrentPlayerState.Value == PlayerState.Result) {

            // �S�[���n�_�̐ݒ肪����ꍇ�A�S�[���𐶐�����
            if (obstacle.isGoal) {
                DropItemManager.instance.GenerateDropItem(ItemType.Goal, new Vector2(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f)));
                //DropBoxBase goal = Instantiate(DropItemManager.instance.GetDropItemPrefab(ItemType.Goal),
                //    new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
            }

            // �h���b�v�A�C�e�������邩����
            (bool isDropTreasure, Rarity[] rarities) = obstacle.JudgeDropTreasure();

            if (isDropTreasure) {

                WeaponData[] weaponDatas = new WeaponData[rarities.Length];

                weaponDatas = DataBaseManager.instance.GetWeaponDataByRarity(rarities);
                DropBoxBase treasure = DropItemManager.instance.GenerateDropItem(ItemType.Weapon, new Vector2(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + Random.Range(-2.0f, 2.0f)));
                //treasure.GetComponent<Treasure>().SetUpTreasure(weaponDatas);
                treasure.SetUpDropBox(weaponDatas);
            } else {

                // �R�C�����t�[�h�̐���
                GenerateNormalItem();
            }
        }
        // ��Q���̍폜
        obstacle.DestroyObstacle();

        yield return new WaitForSeconds(0.5f);

        currentPlayerState = PlayerState.Battle_After;
        CurrentPlayerState.Value = PlayerState.Battle_After;

        yield return new WaitForSeconds(0.25f);

        // �����ɕ�����擾���Ă���ꍇ�ɂ́A�ړ���Ԃɂ͂��Ȃ�
        if (currentPlayerState == PlayerState.Info || CurrentPlayerState.Value == PlayerState.Info) {
            yield break;
        }

        currentPlayerState = PlayerState.Move;
        CurrentPlayerState.Value = PlayerState.Move;

        // HP �Q�[�W����ʊO�ֈړ�(�����ōw�ǂ�����̂ŁA�s�v)
    }

    /// <summary>
    /// �������t�[�h�𐶐�
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
