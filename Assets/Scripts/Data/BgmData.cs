using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// BGM 関連の情報を管理するクラス
/// </summary>
[Serializable]
public class BgmData {
    public int no;          // BGM の通し番号
    public BgmType bgmType;      // BGM の種類
    public float volume = 0.05f;   // BGM のボリューム
    public AudioClip bgmAudioClip;　　// BGM として鳴らすオーディオファイル
    public bool isLoop;
}
