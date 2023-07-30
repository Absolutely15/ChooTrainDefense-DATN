using System.Collections.Generic;
using System.Linq;
using Plugins.AVT.FetchGoogleSheet;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObject/Zombie Data")]
public class ZombieData : ScriptableObject
{
    public string zombieName;
    [Title("Stats")]
    public List<int> health;
    public List<float> moveSpeed;
    public float attackRange = 0.3f;
    [Space]
    public List<int> damageDealtToDoor;
    public List<float> atkDoorCD;
    public int damageDealtToPlayer = 1;
    [Title("Collectible")]
    public float randomSpreadRange = 0.2f;
    public List<int> goldDropPerUnit;
    public int diamondDropPerUnit;
    [Title("Explosive")] 
    public List<int> explosiveDamage;

    [Title("Data Range Fetch")]
    public string URL;

    public string healthRF;
    public string damageDealtToDoorRF;
    public string atkDoorCDRF;
    public string moveSpeedRF;
    public string explosiveDamageRF;
    
#if UNITY_EDITOR
    [Button]
    public void FetchAllBasicData()
    {
        FetchBasicIntData(health, healthRF);
        FetchBasicIntData(damageDealtToDoor, damageDealtToDoorRF);
        FetchBasicFloatData(atkDoorCD, atkDoorCDRF);
        FetchBasicFloatData(moveSpeed, moveSpeedRF);

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    [Button]
    public void FetchExplosiveDamageData()
    {
        FetchBasicIntData(explosiveDamage, explosiveDamageRF);
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    private void FetchBasicIntData(List<int> dataList, string rangeFetch)
    {
        dataList.Clear();
        var block = rangeFetch.ToSheetBlock().ToValidBlock();
        this.GetRawTextFromUrl(URL, (success, text) =>
        {
            if (!success)
                return;

            var data = text.ToSheetMatrix().TrimSheetMatrix(block);
            dataList.AddRange(data.SelectMany(row => row.Where(cell => cell != "")).Select(int.Parse));
        });
    }
    private void FetchBasicFloatData(List<float> dataList, string rangeFetch)
    {
        dataList.Clear();
        var block = rangeFetch.ToSheetBlock().ToValidBlock();
        this.GetRawTextFromUrl(URL, (success, text) =>
        {
            if (!success)
                return;

            var data = text.ToSheetMatrix().TrimSheetMatrix(block);
            dataList.AddRange(data.SelectMany(row => row.Where(cell => cell != "")).Select(float.Parse));
        });
    }
#endif
}

public enum ZombieState
{
    Smashing,
    Seeking,
    Die,
    PlayerDeath
}