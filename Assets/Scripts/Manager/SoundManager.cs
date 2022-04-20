using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Linq;

/// <summary>
/// �����Ǘ��N���X
/// </summary>
public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    public SoundDataSO soundDataSO;

    public const float CROSS_FADE_TIME = 1.0f;  // �N���X�t�F�[�h����

    // �{�����[���֘A
    public float bgmVolume = 0.1f;
    public float seVolume = 0.2f;
    public float voiceVolume = 0.2f;
    public bool isMute = false;

    // �e�I�[�f�B�I�t�@�C���Đ��p�� AudioSource
    private AudioSource[] bgmSources = new AudioSource[2];
    // SE �p�� AudioSource �R���|�[�l���g�������邽�߂̕ϐ��̐錾�B�����p�ӂ��Ă���̂́A�d�����Ė邱�Ƃ�z��
    private AudioSource[] seSources = new AudioSource[10];

    private bool isCrossFading;�@�@�@�@�@�@�@// �N���X�t�F�[�h���������ǂ�������p


    void Awake() {
        // �V���O���g�����A�V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // BGM�p AudioSource�ǉ�
        bgmSources[0] = gameObject.AddComponent<AudioSource>();
        bgmSources[1] = gameObject.AddComponent<AudioSource>();
        bgmSources[1].volume = 0;

        // SE�p�� AudioSource�ǉ�
        for (int i = 0; i < seSources.Length; i++) {
            seSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// BGM�Đ�
    /// </summary>
    /// <param name="bgmType"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(BgmType newBgmType, bool loopFlg = true) {

        // BGM�� Silence �̏�Ԃɂ���ꍇ            
        if ((int)newBgmType == 999) {
            StopBGM();
            return;
        }

        // �Đ����� BGM �p�� BgmData ���擾
        BgmData newBgmData = null;
        foreach (BgmData bgmData in soundDataSO.bgmDataList.Where(x => x.bgmType == newBgmType)) {
            newBgmData = bgmData;
            break;
        }

        // �ΏۂƂȂ�f�[�^���Ȃ���Ώ������Ȃ�
        if (newBgmData == null) {
            return;
        }

        // ����BGM�̏ꍇ�͉������Ȃ�
        if (bgmSources[0].clip != null && bgmSources[0].clip == newBgmData.bgmAudioClip) {
            return;
        } else if (bgmSources[1].clip != null && bgmSources[1].clip == newBgmData.bgmAudioClip) {
            return;
        }

        // �t�F�[�h��BGM�J�n
        if (bgmSources[0].clip == null && bgmSources[1].clip == null) {
            bgmSources[0].loop = loopFlg;
            bgmSources[0].clip = newBgmData.bgmAudioClip;
            bgmSources[0].volume = newBgmData.volume;
            bgmSources[0].Play();
            Debug.Log("Play");
        } else {
            // �N���X�t�F�[�h�����𗘗p���� BGM ��؂�ւ�
            StartCoroutine(CrossFadeChangeBMG(newBgmData, loopFlg));
        }
    }

    /// <summary>
    /// BGM�̃N���X�t�F�[�h����
    /// </summary>
    /// <param name="bgmData"></param>
    /// <param name="loopFlg">���[�v�ݒ�B���[�v���Ȃ��ꍇ����false�w��</param>
    /// <returns></returns>
    private IEnumerator CrossFadeChangeBMG(BgmData bgmData, bool loopFlg) {
        isCrossFading = true;

        if (bgmSources[0].clip != null) {
            // [0]���Đ�����Ă���ꍇ�A[0]�̉��ʂ����X�ɉ����āA[1]��V�����ȂƂ��čĐ�
            bgmSources[1].DOFade(bgmData.volume, CROSS_FADE_TIME).SetEase(Ease.Linear);
            bgmSources[1].clip = bgmData.bgmAudioClip;
            bgmSources[1].loop = loopFlg;
            bgmSources[1].Play();
            bgmSources[0].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);

            yield return new WaitForSeconds(CROSS_FADE_TIME);
            bgmSources[0].Stop();
            bgmSources[0].clip = null;
        } else {
            // [1]���Đ�����Ă���ꍇ�A[1]�̉��ʂ����X�ɉ����āA[0]��V�����ȂƂ��čĐ�
            bgmSources[0].DOFade(bgmData.volume, CROSS_FADE_TIME).SetEase(Ease.Linear);
            bgmSources[0].clip = bgmData.bgmAudioClip;
            bgmSources[0].loop = loopFlg;
            bgmSources[0].Play();
            bgmSources[1].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);

            yield return new WaitForSeconds(CROSS_FADE_TIME);
            bgmSources[1].Stop();
            bgmSources[1].clip = null;
        }
        isCrossFading = false;
    }

    /// <summary>
    /// BGM���S��~
    /// </summary>
    public void StopBGM() {
        bgmSources[0].Stop();
        bgmSources[1].Stop();
        bgmSources[0].clip = null;
        bgmSources[1].clip = null;
    }

    /// <summary>
    /// BGM�ꎞ��~
    /// </summary>
    public void MuteBGM() {
        bgmSources[0].Stop();
        bgmSources[1].Stop();
    }

    /// <summary>
    /// �ꎞ��~��������BGM���Đ�(�ĊJ)
    /// </summary>
    public void ResumeBGM() {
        bgmSources[0].Play();
        bgmSources[1].Play();
    }

    /// <summary>
    /// SE�Đ�
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SeType newSeType) {

        // �Đ����� SE �p�� SeData ���擾
        SeData newSeData = null;
        foreach (SeData seData in soundDataSO.seDataList.Where(x => x.seType == newSeType)) {
            newSeData = seData;
            break;
        }

        // �Đ����ł͂Ȃ�AudioSouce��������SE��炷
        foreach (AudioSource source in seSources) {
            if (source.isPlaying == false) {
                source.clip = newSeData.seAudioClip;
                source.volume = newSeData.volume;
                source.Play();
                return;
            }
        }
    }

    /// <summary>
    /// SE��~
    /// </summary>
    public void StopSE() {
        // �S�Ă�SE�p��AudioSouce���~����
        foreach (AudioSource source in seSources) {
            source.Stop();
            source.clip = null;
        }
    }
}