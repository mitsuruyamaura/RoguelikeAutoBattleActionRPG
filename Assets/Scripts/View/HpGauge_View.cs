using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HpGauge_View : MonoBehaviour
{
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


    void Start() {
        // �f�o�b�O�p
        SetUpHpGauge();
    }

    /// <summary>
    /// HP �Q�[�W���\���ɂ��Ĉʒu����ʊO�ɐݒ�
    /// </summary>
    public void SetUpHpGauge() {
        playerCanvasGroup.alpha = 0;
        enemyCanvasGroup.alpha = 0;

        playerCanvasGroup.transform.position = playerHpGaugeTrans[0].transform.position;
        enemyCanvasGroup.transform.position = enemyHpGaugeTrans[0].transform.position;
    }

    /// <summary>
    /// �o����HP�Q�[�W�̈ʒu�̊m�F�B��Q����HP�Q�[�W���ő�l�ɂ���(�����\�ɂȂ���������ŏC������)
    /// </summary>
    public void PrepareCheckHpGaugeState() {

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

        // ����́A��ɏ�Q���� HP�Q�[�W�͍ő�l�ɂ���
        imgEnemyHpGauge.fillAmount = 1.0f;
    }

    /// <summary>
    /// Hp�Q�[�W�̈ړ�
    /// </summary>
    public void MoveHpGaugePositions(float alpha, int index, bool isPlayerOnly = false) {
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
    /// ��Q����Hp�Q�[�W�X�V
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxHp"></param>
    public void UpdateObstacleHpGauge(int hp, int maxHp, int amount) {
        Debug.Log("hp : " + hp + " / maxHp : " + maxHp);
        imgEnemyHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.InCirc);

        if (maxHp != 0) {
            EffectBase enemyFloatingMessage = Instantiate(EffectManager.instance.GetEffect(EffectType.FloatingMessage), enemyHpGaugeTrans[1].transform, false);
            enemyFloatingMessage.TriggerEffect(amount);
        }
    }

    /// <summary>
    /// �v���C���[��Hp�Q�[�W�X�V
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxHp"></param>
    public void UpdatePlayerHpGauge(float hp, float maxHp, int amount, bool isGain = false) {
        imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);

        EffectBase playerFloatingMessage = Instantiate(EffectManager.instance.GetEffect(EffectType.FloatingMessage), playerHpGaugeTrans[1].transform, false);

        if (amount > 0) {
            isGain = true;
        }
        playerFloatingMessage.TriggerEffect(amount, false, isGain);
    }
}
