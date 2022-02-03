using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgWeaponTypeIcon;

    [SerializeField]
    private Text txtParameters;

    
    public void SetUpSkillDetail(SkillData skillData, int totalWeight) {

        imgWeaponTypeIcon.sprite = skillData.sprite;

        txtParameters.text = skillData.name + " : " + skillData.rarity.ToString() + " : " + skillData.weight + " / " + totalWeight + "\n";
        txtParameters.text += "Attack : " + skillData.attackPower + " / Critical : " + skillData.criticalRate + " / DoubleAttack : " + skillData.doubleStrikeRate
            + " / Speed : " + skillData.attackSpeed + "\n";
        txtParameters.text += "SP : -"; 
    }
}
