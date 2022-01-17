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
    /// コイン表示更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateDisplayCoin(int oldValue, int newValue) {
        txtCoin.DOCounter(oldValue, newValue, 0.5f);
    }

    /// <summary>
    /// フード表示更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateDisplayFood(int oldValue, int newValue) {
        txtFood.DOCounter(oldValue, newValue, 0.5f);

        // TODO あとでスライダーも追加

    }
}
