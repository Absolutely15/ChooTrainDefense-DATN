using UnityEngine;
using UnityEngine.UI;

public class SFXButton : MonoBehaviour
{
    public AudioType audioType = AudioType.Click;
    public Button btn;

    private void OnValidate()
    {
        if (btn == null)
        {
            btn = GetComponent<Button>();
        }
    }

    private void Awake()
    {
        btn.onClick.AddListener(() => AudioManager.Instance.PlayAudio(audioType, true));
    }
}
