using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// BGM �֘A�̏����Ǘ�����N���X
/// </summary>
[Serializable]
public class BgmData {
    public int no;          // BGM �̒ʂ��ԍ�
    public BgmType bgmType;      // BGM �̎��
    public float volume = 0.05f;   // BGM �̃{�����[��
    public AudioClip bgmAudioClip;�@�@// BGM �Ƃ��Ė炷�I�[�f�B�I�t�@�C��
    public bool isLoop;
}
