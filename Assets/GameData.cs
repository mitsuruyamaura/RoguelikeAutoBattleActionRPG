using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public WeaponData currentWeaponData;
    public int currentUseCount;
    public int stageNo;

    public int coin;
    public int food;
    public int hp;

    public Character currentChara;


    // 最大HP
    // アーティファクト


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }   
    }


    public void CalculateCharacterParametersByArtifacts(Artifact artifact) {

    }

}
