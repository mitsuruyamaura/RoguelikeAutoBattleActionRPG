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
        playerCanvasGroup.alpha = 0;
        enemyCanvasGroup.alpha = 0;

        playerCanvasGroup.transform.position = playerHpGaugeTrans[0].transform.position;
        enemyCanvasGroup.transform.position = enemyHpGaugeTrans[0].transform.position;


    }


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

        imgEnemyHpGauge.fillAmount = 1.0f;
    }


    /// <summary>
    /// Hpゲージの移動
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
    /// 障害物のHpゲージ更新
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxHp"></param>
    public void UpdateObstacleHpGauge(float hp, float maxHp) {
        imgEnemyHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.InCirc);
    }

    /// <summary>
    /// プレイヤーのHpゲージ更新
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxHp"></param>
    public void UpdatePlayerHpGauge(float hp, float maxHp) {
        imgPlayerHpGauge.DOFillAmount((float)hp / maxHp, 0.25f).SetEase(Ease.Linear);
    }
}
