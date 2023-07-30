using System.Collections.Generic;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;

[DefaultExecutionOrder(-1)]
public class PlayerSkin : MonoBehaviour
{
    public List<GameObject> skins;

    private List<SkinBonus> skinDataValueList;
    private SkinData curSkinData;
    private float bonusStat;
    private float bonusGold;
    private void Awake()
    {
        Init();
        InitEvent();
    }

    private void Init()
    {
        EquipSkin();
        SetCurrentSkinActive();
    }

    private void InitEvent()
    {
        Observer.Instance.AddObserver(EventID.ChangeSkin, SetCurrentSkinActive);
        Observer.Instance.AddObserver(EventID.EndGameLevelData, GetSkinBonusGold);
    }

    private void GetSkinBonusGold()
    {
        if (curSkinData != GameDB.skinDatas[4]) return;
        bonusGold = CollectibleCollector.CollectedGoldThisLevel * GetSkinBonusStat(SkinBonusType.Gold, 1);
        PlayerSave.Gold += (int)bonusGold;
    }
    public void EquipSkin()
    {
        curSkinData = GameDB.skinDatas[PlayerSave.GetSkinIsUsing()];
        Observer.Instance.Notify(EventID.ChangeSkin);
    }
    private void SetCurrentSkinActive()
    {
        for (var i = 0; i < skins.Count; i++)
        {
            skins[i].gameObject.SetActive(i == PlayerSave.GetSkinIsUsing());
        }
    }
    
    public float GetSkinBonusStat(SkinBonusType skinBonusType, float defaultValue)
    {
        skinDataValueList = curSkinData.skinBonusValues;
        bonusStat = defaultValue;
        if (skinDataValueList == null || skinDataValueList.Count <= 0) return bonusStat;
        foreach (var bonus in skinDataValueList)
        {
            if (bonus.skinBonusType == skinBonusType)
            {
                bonusStat = bonus.skinBonusValue;
            }
        }
        return bonusStat;
    }
}



