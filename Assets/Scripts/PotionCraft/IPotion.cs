using UnityEngine;

public enum PotionAbility
{
    None,
    Speed,
    Slow,
    Magnetism,
    Butterfingers,
    VisionBoost,
    Blinkers,
    Confuse,
    Max
}

public interface IPotion
{
    public abstract string Name { get; }
    public abstract PotionAbility ID { get; }
    public abstract Sprite Icon { get; }
    // Sorry Matt, pretend you don't see this and it's all good.
    // I saw, I fixed - M@
    public abstract Combo Combo { get; }

    // Each potion could affect a different component, so this way we pass any target
    // and the concrete potion finds the relevant component.
    public abstract void Drink(GameObject target);
}

public static class PotionUtils
{
    public static string AbilityToName(PotionAbility ability)
    {
        // TODO: Need to update with proper potion names!
        return ability.ToString();
    }
}
