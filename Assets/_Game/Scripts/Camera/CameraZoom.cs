using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float defaultDist = 4;
    [SerializeField] private float zoomOutBonusDist;
    
    private CinemachineFramingTransposer transposer;
    private Tween zoomTween;
    private void Start()
    {
        transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    
    [Button]
    public void ZoomToggle()
    {
        if (zoomOutBonusDist == 0)
            ZoomOut();
        else
            ZoomIn();
    }

    public void ZoomIn()
    {
        zoomOutBonusDist = 0;
        SmoothZoom(defaultDist);
    }
    
    public void ZoomOut()
    {
        zoomOutBonusDist = 2;
        SmoothZoom(defaultDist + zoomOutBonusDist);
    }

    private void SmoothZoom(float target)
    {
        zoomTween?.Kill();
        zoomTween = DOVirtual.Float(transposer.m_CameraDistance, target, 0.5f, ChangeZoom);
    }

    private void ChangeZoom(float distance) => transposer.m_CameraDistance = distance;
}
