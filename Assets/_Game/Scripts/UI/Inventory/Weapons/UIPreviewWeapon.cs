using System.Collections.Generic;
using UnityEngine;
using static NVTT.Utilities;

public class UIPreviewWeapon : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> weapons;
    [SerializeField] private List<ParticleSystem> sparkle;
    [SerializeField] private GameObject pointLight;
    
    public void DisplayGun(WeaponLocalData wData)
    {
        var state = wData.GetWeaponState();
        if (state == WeaponState.Unlocked || wData.isPremium)
        {
            weapons[wData.id].material = GameDB.weaponUpgradeData.weaponMat[0];
            ParticleResponse(0);
            pointLight.SetActive(false);
        }
        else
        {
            weapons[wData.id].material = GameDB.weaponUpgradeData.weaponMat[1];
            ParticleResponse(2);
            pointLight.SetActive(false);
        }

        if (state == WeaponState.Golden)
        {
            weapons[wData.id].material = GameDB.weaponUpgradeData.weaponMat[2];
            ParticleResponse(1);
            pointLight.SetActive(true);
        }

        for (var i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(i == wData.id);
        }
    }

    private void ParticleResponse(int j)
    {
        for (var i = 0; i < sparkle.Count; i++)
        {
            if (j == i)
                sparkle[i].Play();
            else
            {
                sparkle[i].Stop(default, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}
