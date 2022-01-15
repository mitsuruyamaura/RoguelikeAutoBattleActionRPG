using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField]
    private GameObject[] battleEffectPrefabs;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// エフェクト取得
    /// </summary>
    /// <param name="effectType"></param>
    /// <returns></returns>
    public GameObject[] GetEffect(EffectType effectType) {
        return effectType switch {
            EffectType.Battle => battleEffectPrefabs,
            _ => null
        };
    }
}
