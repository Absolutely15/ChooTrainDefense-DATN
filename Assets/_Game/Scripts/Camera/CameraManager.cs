using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using NVTT;
using UnityBase.DesignPattern;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private List<CinemachineVirtualCamera> vCamera;
    [SerializeField] private int defaultCamera;

    private void OnValidate()
    {
        defaultCamera = Mathf.Clamp(defaultCamera, 0, vCamera.Count - 1);
    }

    private void Start()
    {
        Observer.Instance.AddObserver(EventID.EndGameLevel, EndGameCamFocus);
    }

    public void SwitchToCamera(VCam vCam) => SwitchToCamera((int)vCam);
    public void SwitchToCamera(int camID)
    {
        for (var i = 0; i < vCamera.Count; i++)
            vCamera[i].Priority = i == camID ? 1 : 0;
    }

    private void EndGameCamFocus()
    {
        vCamera[(int)VCam.EndGame].Follow = Utilities.MapManager.zombieSpawner.lastZombieKilled.transform;
        SwitchToCamera(VCam.EndGame);
        DOVirtual.DelayedCall(2, () => SwitchToCamera(VCam.Player));
    }
}

public enum VCam
{
    Player,
    EndGame
}
