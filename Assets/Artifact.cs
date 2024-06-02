using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Artifact
{
    public int no;
    public string name;
    public Rarity rarity;
    public int dropWeight;
    public Sprite artifactSprite;

    // 補正値
    public int hp;
    public int food;
    public int attackPower;
    public int critacalRate;
    public int doubleStrikeRate;
    public int attackSpeed;

    // 特性・耐性・特殊効果の類(今の所、アーティファクトは捨てられない予定なので、永続的な効果)
    public PotentialBase[] potentials;

    // PotentialBase
    // 配列にしておいて、この効果を耐性や特性としてインスタンスして適用する

    // 耐性(毒(指定時間の間、移動ごとにダメージ)・スタン(指定ターン攻撃不可)・暗闇(指定時間の間、画面の一部しか見えなくなる)・暴食(指定時間の間、食料の減りが早くなる))

    // 特殊効果・特性
    // ここにあるクラスをインスタンスしてアタッチさせて、効果を適用する

    // 障害物(木・岩など)にダメージアップ
    // ダメージゾーンからダメージ受けない
    // コイン獲得量アップ
    // 食料獲得量アップ
    // HP回復量アップ
    // 移動速度アップ
    // トレジャーの罠回避
    // ダメージゾーンを破壊できるようになる
    // 敵の拠点を破壊できるようになる
    // ショップの出現確率アップ
    // 食料最大値アップ
    // HP最大値アップ
    // アイテムのドロップ率アップ


    // ConditonBase
    // 毒などの状態変化系の一時的な効果を付与する
    // 一時的な攻撃力アップなどを作る場合にも利用できる

}
