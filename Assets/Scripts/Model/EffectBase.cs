using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour {
    public virtual void TriggerEffect(int amount, bool isCritical = false, bool isGain = false) { }
}
