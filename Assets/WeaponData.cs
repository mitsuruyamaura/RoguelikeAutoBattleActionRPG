using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData {
    public int no;
    public string name;
    public int useCount;
    public Rarity rarity;
    public int weight;
    public string skillNos;
    public Sprite sprite;
    public List<SkillData> skillDatasList = new List<SkillData>();
}
