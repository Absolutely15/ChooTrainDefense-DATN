using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public TMP_InputField tmpInputField;
    public void SetLevel()
    {
        PlayerSave.CurrentGameLevel = Mathf.Clamp(int.Parse(tmpInputField.text) - 1, 0, 999);
        if (GPRainbowDefense.CheckPoint.Contains(PlayerSave.CurrentGameLevel))
        {
            PlayerSave.AlreadyInitMapData = false;
            Replay();
        }
        else
        {
            Replay();
        }
    }

    [Button]
    public void HackMoney()
    {
        PlayerSave.Gold += 999999;
        PlayerSave.Diamond += 999999;
    }

    [Button]
    public void Replay()
    {
        GPExecutor.ReloadScene();
    }
    
}