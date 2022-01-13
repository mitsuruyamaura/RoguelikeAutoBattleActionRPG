using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Create SkillDataSO")]
public class SkillDataSO : ScriptableObject {
    public List<SkillData> skillDatasList = new List<SkillData>();
}
