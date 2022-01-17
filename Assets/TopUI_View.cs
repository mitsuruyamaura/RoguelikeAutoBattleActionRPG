using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TopUI_View : MonoBehaviour
{
    [SerializeField]
    private Text txtCoin;

    [SerializeField]
    private Text txtFood;

    /// <summary>
    /// �R�C���\���X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateDisplayCoin(int oldValue, int newValue) {
        txtCoin.DOCounter(oldValue, newValue, 0.5f);
    }

    /// <summary>
    /// �t�[�h�\���X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateDisplayFood(int oldValue, int newValue) {
        txtFood.DOCounter(oldValue, newValue, 0.5f);

        // TODO ���ƂŃX���C�_�[���ǉ�

    }
}
