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

    // �␳�l
    public int hp;
    public int food;
    public int attackPower;
    public int critacalRate;
    public int doubleStrikeRate;
    public int attackSpeed;

    // �����E�ϐ��E������ʂ̗�(���̏��A�A�[�e�B�t�@�N�g�͎̂Ă��Ȃ��\��Ȃ̂ŁA�i���I�Ȍ���)
    public PotentialBase[] potentials;

    // PotentialBase
    // �z��ɂ��Ă����āA���̌��ʂ�ϐ�������Ƃ��ăC���X�^���X���ēK�p����

    // �ϐ�(��(�w�莞�Ԃ̊ԁA�ړ����ƂɃ_���[�W)�E�X�^��(�w��^�[���U���s��)�E�È�(�w�莞�Ԃ̊ԁA��ʂ̈ꕔ���������Ȃ��Ȃ�)�E�\�H(�w�莞�Ԃ̊ԁA�H���̌��肪�����Ȃ�))

    // ������ʁE����
    // �����ɂ���N���X���C���X�^���X���ăA�^�b�`�����āA���ʂ�K�p����

    // ��Q��(�؁E��Ȃ�)�Ƀ_���[�W�A�b�v
    // �_���[�W�]�[������_���[�W�󂯂Ȃ�
    // �R�C���l���ʃA�b�v
    // �H���l���ʃA�b�v
    // HP�񕜗ʃA�b�v
    // �ړ����x�A�b�v
    // �g���W���[��㩉��
    // �_���[�W�]�[����j��ł���悤�ɂȂ�
    // �G�̋��_��j��ł���悤�ɂȂ�
    // �V���b�v�̏o���m���A�b�v
    // �H���ő�l�A�b�v
    // HP�ő�l�A�b�v
    // �A�C�e���̃h���b�v���A�b�v


    // ConditonBase
    // �łȂǂ̏�ԕω��n�̈ꎞ�I�Ȍ��ʂ�t�^����
    // �ꎞ�I�ȍU���̓A�b�v�Ȃǂ����ꍇ�ɂ����p�ł���

}
