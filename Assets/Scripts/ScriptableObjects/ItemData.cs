using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public Color32 itemColor;
    public float effectDuration;
    public string effectName;

    public abstract void ItemEffect();
}
