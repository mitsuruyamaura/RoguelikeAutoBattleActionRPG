using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleData
{
    public int no;
    public string name;
    public int hp;
    public int moveSpeed;
    public int attackPower;
    public int attackSpeed;
    public Rarity rarity;
    public ObstacleType obstacleType;
    public WeaponData weaponData;
    public int treasureDropRate;
    public Sprite sprite;
    public Vector2 colliderSize;

}
