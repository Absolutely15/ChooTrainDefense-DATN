using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.AVT.FetchGoogleSheet;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObject/Map Data")]
public class MapData : ScriptableObject
{
    public List<MapSaveData> defaultMapSaveData;
    public List<MapManager> map;
    public List<ZombieSpawnAmount> zombieSpawnAmount;
    [Space]
    public string URL;
    public string zombieRangeFetch;
    
#if UNITY_EDITOR
    [Button]
    public void FetchZombieAmount()
    {
        zombieSpawnAmount.Clear();
        var block = zombieRangeFetch.ToSheetBlock().ToValidBlock();
        this.GetRawTextFromUrl(URL, (success, text) =>
        {
            if (!success)
                return;

            var data = text.ToSheetMatrix().TrimSheetMatrix(block);
            foreach (var row in data)
            {
                var rowData = new ZombieSpawnAmount();
                foreach (var cell in row.Where(cell => cell != ""))
                {
                    rowData.zombieType.Add(int.Parse(cell));
                }
                zombieSpawnAmount.Add(rowData);
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        });
    }
#endif
}

[Serializable]
public class ZombieSpawnAmount
{
    public List<int> zombieType = new List<int>();
}
