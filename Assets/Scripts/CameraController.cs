using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// �J��������p
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    private float originLensOrthoSize;
    private float zoomLensOrthoSize = 3.0f;
    private float zoomDuration = 0.5f;

    private bool isZoom;  // true �Ȃ� ZoomIn ��

    void Start()
    {
        originLensOrthoSize = virtualCamera.m_Lens.OrthographicSize;

        // �f�o�b�O�p
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ => {
                isZoom = !isZoom;
                StartCoroutine(ChangeCameraOrthoSize(isZoom ? zoomLensOrthoSize : originLensOrthoSize));
            });

        // �f�o�b�O�p
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.I))
            .Subscribe(_ => ImpulseCamera());
    }

    /// <summary>
    /// �J������ LensOrthoSize ��ύX�����Y�[������
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IEnumerator ChangeCameraOrthoSize(float value) {
        DOTween.To(
            () => virtualCamera.m_Lens.OrthographicSize,
            x => virtualCamera.m_Lens.OrthographicSize = x,
            value,
            zoomDuration
            );
        yield return null;
    }

    /// <summary>
    /// �J�����̐U��
    /// </summary>
    public void ImpulseCamera() {
        // �J�����U��
        impulseSource.GenerateImpulse();
    }
}
