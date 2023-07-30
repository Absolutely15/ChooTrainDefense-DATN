using Sirenix.OdinInspector;
using UnityEngine;

public class ItemData : ScriptableObject
{
    [Title("Information")]
    public int id;
    public string itemName;
    public string description;
    [PreviewField(100, ObjectFieldAlignment.Center)] public Sprite icon;
}
