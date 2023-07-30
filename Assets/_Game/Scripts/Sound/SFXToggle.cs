using UnityEngine;
using UnityEngine.UI;

public class SFXToggle : MonoBehaviour
{
    public AudioType audioType = AudioType.Click;
    public Toggle toggle;

    private void OnValidate()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        
    }

    private void Awake()
    {
        if (toggle != null)
            toggle.onValueChanged.AddListener(a => AudioManager.Instance.PlayAudio(audioType, true));
    }
}