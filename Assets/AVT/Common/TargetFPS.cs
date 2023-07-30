using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    [SerializeField]
    private int fps = 60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;
    }
}
