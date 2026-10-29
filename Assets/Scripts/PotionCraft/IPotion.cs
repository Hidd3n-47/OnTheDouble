using UnityEngine;

public interface IPotion
{
    public abstract string Name { get; }
    public abstract Sprite Icon { get; }
    public abstract void Drink();
    public abstract void SetupPrefab(GameObject parent);
}
