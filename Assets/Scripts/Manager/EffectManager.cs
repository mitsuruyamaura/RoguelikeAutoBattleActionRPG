using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField]
    private EffectBase[] battleEffectPrefabs;

    [SerializeField]
    private EffectBase floatingMessagePrefab;

    [SerializeField]
    private EffectBase trasureEffectPrefab;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 複数のエフェクト取得
    /// </summary>
    /// <param name="effectType"></param>
    /// <returns></returns>
    public EffectBase[] GetEffects(EffectType effectType) {
        return effectType switch {
            EffectType.Battle => battleEffectPrefabs,
            _ => null
        };
    }

    /// <summary>
    /// 単一のエフェクト取得
    /// </summary>
    /// <param name="effectType"></param>
    /// <returns></returns>
    public EffectBase GetEffect(EffectType effectType) {
        return effectType switch {
            EffectType.FloatingMessage => floatingMessagePrefab,
            EffectType.TresureDrop => trasureEffectPrefab,
            _ => null
        };
    }
}
