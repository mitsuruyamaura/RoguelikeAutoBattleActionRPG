using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// カメラ制御用
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

    private bool isZoom;  // true なら ZoomIn 中

    void Start()
    {
        originLensOrthoSize = virtualCamera.m_Lens.OrthographicSize;

        // デバッグ用
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ => {
                isZoom = !isZoom;
                StartCoroutine(ChangeCameraOrthoSize(isZoom ? zoomLensOrthoSize : originLensOrthoSize));
            });

        // デバッグ用
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.I))
            .Subscribe(_ => ImpulseCamera());
    }

    /// <summary>
    /// カメラの LensOrthoSize を変更したズーム処理
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
    /// カメラの LensOrthoSize を変更したズーム処理
    /// </summary>
    /// <param name="playerState"></param>
    /// <returns></returns>
    public IEnumerator ChangeCameraOrthoSize(PlayerController.PlayerState playerState) {

        // シーン遷移とバトル開始時はズームイン、バトル終了時はズームアウト
        float value = playerState == PlayerController.PlayerState.Battle_After ? originLensOrthoSize : zoomLensOrthoSize;

        DOTween.To(
            () => virtualCamera.m_Lens.OrthographicSize,
            x => virtualCamera.m_Lens.OrthographicSize = x,
            value,
            zoomDuration
            );
        yield return null;
    }

    /// <summary>
    /// カメラの振動
    /// </summary>
    public void ImpulseCamera() {
        // カメラ振動
        impulseSource.GenerateImpulse();
        //Debug.Log("カメラ振動");
    }
}
