using UnityEngine;

[System.Serializable]
public class Character
{
    public int no;
    public string name;
    public int maxHp;
    public int maxFood;
    public int moveSpeed;
    public int attackPower;
    public int critacalRate;
    public int doubleStrikeRate;
    public int attackSpeed;
    public Rarity rarity;
    public Sprite charaSprite;


    public static Character CreateChara() {
        return new Character() {
            maxHp = 10
        };
    }
}
