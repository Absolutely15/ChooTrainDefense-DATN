#if UNITY_EDITOR
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEditor;
using UnityEngine;

namespace Plugins.AVT.FetchGoogleSheet
{
    [CreateAssetMenu]
    public class TestDataSO : ScriptableObject
    {
        [Multiline]
        public string sheetUrl;
        public string range;
        public List<UnitData> units;

        #if UNITY_EDITOR
        [MenuItem("CONTEXT/TestDataSO/Fetch")]
        public static void Fetch(MenuCommand command)
        {
            var _ = (TestDataSO)command.context;
            //_.SheetToList(_.sheetUrl, _.units);

            var block = _.range.ToSheetBlock().ToValidBlock();
            _.GetRawTextFromUrl(_.sheetUrl, (success, text) =>
            {
                if (!success)
                    return;

                var data = text.ToSheetMatrix().TrimSheetMatrix(block);
                
            });
        }
        #endif

#if ODIN_INSPECTOR

        [Button]
        public void TestRegex() => Debug.Log(range.ToSheetBlock().ToValidBlock());

#endif

    }

    [System.Serializable]
    public struct UnitData : IGoogleSheetDataSetter
    {
        public string name;
        public int health;
        public int damage;

        public void SetDataFromSheet(Dictionary<string, string> source)
        {
            name = source["name"];
            health = int.Parse(source["health"]);
            damage = int.Parse(source["damage"]);
        }
    }
}
#endif

