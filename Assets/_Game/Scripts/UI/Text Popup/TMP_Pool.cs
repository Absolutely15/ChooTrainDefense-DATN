using Sirenix.OdinInspector;
using TMPro;

public class TMP_Pool : SimplePool<TextMeshProUGUI>
{
    [Button]
    public void Popup() => Popup("");
    public void Popup(string newTxt)
    {
        var pText = Get();
        if (newTxt != "")
            pText.text = newTxt;
        pText.gameObject.SetActive(true);
    }
}
