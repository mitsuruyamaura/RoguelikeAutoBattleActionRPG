using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUpPopUp : MonoBehaviour
{
    [SerializeField]
    private Button btnSubmit;

    [SerializeField]
    private Image imgGameUp;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Sprite spriteGameOver;

    [SerializeField]
    private Sprite spriteGameClear;

    /// <summary>
    /// �|�b�v�A�b�v�\���Ɛݒ�
    /// </summary>
    /// <param name="isGameOver"></param>
    public void ShowGameUpPopUp(bool isGameOver) {
        canvasGroup.alpha = 0;
        btnSubmit.interactable = false;
        btnSubmit.onClick.AddListener(OnClickSubmit);

        imgGameUp.rectTransform.sizeDelta = Vector2.zero;
        imgGameUp.color = new Color(1, 1, 1, 0);
        imgGameUp.sprite = isGameOver ? spriteGameOver : spriteGameClear;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1, 0.5f).SetEase(Ease.InQuart));
        // �Q�[����Ԃɂ��A�摜�̃T�C�Y��ύX
        if (isGameOver) {
            sequence.Append(imgGameUp.rectTransform.DOSizeDelta(new Vector2(2000, 450), 1.5f).SetEase(Ease.InQuart));
        } else {
            sequence.Append(imgGameUp.rectTransform.DOSizeDelta(new Vector2(1200, 450), 1.5f).SetEase(Ease.InQuart));
        }
        sequence.Join(imgGameUp.DOColor(new Color(1, 1, 1, 1), 1.75f).SetEase(Ease.InQuart)).OnComplete(() => btnSubmit.interactable = true);
    }

    /// <summary>
    /// ��ʃ^�b�v���̏���
    /// </summary>
    private void OnClickSubmit() {
        canvasGroup.DOFade(0, 1.5f).SetEase(Ease.InQuart).OnComplete(() => Debug.Log("���X�^�[�g"));
    }
}
