using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static NVTT.Utilities;

public class UIPreviewSkin : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> skins;
    [SerializeField] private List<Material> mat;
    [SerializeField] private ParticleSystem sparkle;
    [SerializeField] private TextMeshProUGUI textDesc;
    
    public void DisplaySkin(SkinData sData, SkinState state)
    {
        if (state == SkinState.Unlocked)
        {
            skins[sData.id].material = mat[0];
            sparkle.Play();
        }
        else
        {
            skins[sData.id].material = mat[1];
            sparkle.Stop(default, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        for (var i = 0; i < skins.Count; i++)
        {
            skins[i].gameObject.SetActive(i == sData.id);
        }

        textDesc.text = $"{GameDB.skinDatas[sData.id].description}";
    }
}
