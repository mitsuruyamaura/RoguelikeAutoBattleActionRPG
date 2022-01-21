using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Create WeaponDataSO")]
public class WeaponDataSO : ScriptableObject {
    public List<WeaponData> weaponDatasList = new List<WeaponData>();
}
