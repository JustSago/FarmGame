using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{   
    public string Name;
    public Sprite Icon;
    public int maxCount;    
}
