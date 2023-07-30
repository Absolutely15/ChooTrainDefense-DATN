using UnityEngine;
using static NVTT.Utilities;

public class WeaponSkin : MonoBehaviour
{
    [SerializeField] private MeshRenderer weaponMeshRend;
    [SerializeField] private ParticleSystem goldenStarParticle;

    private WeaponState curWeaponState;

    private void OnValidate()
    {
        if (weaponMeshRend == null)
            weaponMeshRend = GetComponentInChildren<MeshRenderer>();
    }

    public void GetCurrentMat(Weapon weapon)
    {
        curWeaponState = weapon.weaponLocalData.GetWeaponState();
        weaponMeshRend.material = curWeaponState == WeaponState.Golden ? GameDB.weaponUpgradeData.weaponMat[2] : GameDB.weaponUpgradeData.weaponMat[0];
        if (curWeaponState == WeaponState.Golden)
            goldenStarParticle.gameObject.SetActive(true);
    }
}
