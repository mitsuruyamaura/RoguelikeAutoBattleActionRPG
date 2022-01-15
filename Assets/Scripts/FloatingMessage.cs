using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingMessage : EffectBase
{
    [SerializeField]
    private Text txtMessage;


    public override void TriggerEffect(int amount, bool isCritical = false, bool isGain = false) {
        ShowMessage(amount, isCritical, isGain);
    }

    /// <summary>
    /// フロート表示の生成と設定
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="isCritical"></param>
    /// <param name="isGain"></param>
    public void ShowMessage(int amount, bool isCritical = false, bool isGain = false) {

        transform.position = new Vector3(transform.position.x + Random.Range(-10.0f, 10.0f), transform.position.y + Random.Range(0, 5.0f), 0);

        transform.localScale = isCritical ? Vector3.one * Random.Range(2.0f, 2.5f) : Vector3.one * Random.Range(1.0f, 1.3f);

        if (isGain) {
            txtMessage.color = Color.blue;
            txtMessage.text = "+";
        }
        txtMessage.text += amount.ToString();

        transform.DOMoveY(transform.position.y + 10, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { Destroy(gameObject); }
            );       
    }
}
