using System.Collections.Generic;
using System.Linq;
using Plugins.AVT.FetchGoogleSheet;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObject/Currency Bonus Data")]
public class CurrencyBonusData : ScriptableObject
{
    public string URL;
    public string diamondRangeFetch;
    [Space]
    public List<int> diamondBonusEachLevel;
#if UNITY_EDITOR
    [Button]
    public void FetchDiamondAmount()
    {
        diamondBonusEachLevel.Clear();
        var block = diamondRangeFetch.ToSheetBlock().ToValidBlock();
        this.GetRawTextFromUrl(URL, (success, text) =>
        {
            if (!success)
                return;

            var data = text.ToSheetMatrix().TrimSheetMatrix(block);
            foreach (var cell in data.SelectMany(row => row.Where(cell => cell != "")))
            {
                diamondBonusEachLevel.Add(int.Parse(cell));
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        });
    }
#endif
}
