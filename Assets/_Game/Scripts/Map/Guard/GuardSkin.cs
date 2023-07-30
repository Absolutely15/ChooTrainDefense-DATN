using UnityEngine;
using static NVTT.Utilities;

public class GuardSkin : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    
    public void GetMaterialData(int lev)
    {
        skinnedMesh.material = GameDB.guardUpgradeData.upgradeMaterials[lev - 1];
    }
}
