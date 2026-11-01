using System.Collections;
using UnityEngine;

public class GenericPotion : IPotion
{
    protected string m_name;
    protected Sprite m_icon;
    protected PotionAbility m_ability;

    public string Name => m_name;
    public PotionAbility ID => m_ability;
    public Sprite Icon => m_icon;

    // Sorry Matt, pretend you don't see this and it's all good.
    // I saw, I fixed. - M@
    public Combo Combo { get; private set; }

    public virtual void Drink(GameObject target)
    {
        // Do any effects and whatnot. 
        // example implementation:
        // target.GetComponent<PotionFX>().DefaultDrink();
    }

    public GenericPotion(Recipe recipe)
    {
        m_name    = recipe.Name;
        m_icon    = recipe.Icon;
        m_ability = recipe.ID;

        // @ Hidd3n FIFY (All you needed is to read a little more ;)
        Combo = new Combo(recipe.Elements);
    }
}

// The way I did potions changed part-way though, so without visuals or anything like that, these can all just become one class
// However I'm leaving it in as the clases are added, and in case there is bespoke behaviour we decide to add, it'll be good to have 'em separate... -M@

public class SpeedModPotion : GenericPotion
{
    public SpeedModPotion(Recipe recipe) : base(recipe) { }

    public override void Drink(GameObject target)
    {
        if(m_ability == PotionAbility.Speed)
        {
            // Speed Up Player
            target.GetComponent<PlayerController>()?.AddSpeedMod(0.5f);
        }
        else // Slow
        {
            // Slow Player
            target.GetComponent<PlayerController>()?.AddSpeedMod(-0.65f);
        }
    }
}

public class MagnetismPotion : GenericPotion
{
    public MagnetismPotion(Recipe recipe) : base(recipe) { }

    public override void Drink(GameObject target)
    {
        if(m_ability == PotionAbility.Magnetism)
        {
            // 50% Boost on normal.
            target.GetComponent<IModdable>()?.AddMod(m_ability, 1.5f);
        }
        else // Reduce Range - Not yet used. 
        {
            // Reduce by 25% on normal
            target.GetComponent<IModdable>()?.AddMod(m_ability,-1.25f);
        }
    }
}

public class VisionModPotion : GenericPotion
{
    public VisionModPotion(Recipe recipe) : base(recipe) { }

    public override void Drink(GameObject target)
    {
        if(m_ability == PotionAbility.VisionBoost)
        {
            // Increase FOV by 10
            target.GetComponent<IModdable>()?.AddMod(m_ability,10f);
        }
        else // Blinkers
        {
            // Reduce FOV instead.
            target.GetComponent<IModdable>()?.AddMod(m_ability,-10f);
        }
    }
}

public class ButterfingersPotion : GenericPotion
{
    public ButterfingersPotion(Recipe recipe) : base(recipe) { }

    public override void Drink(GameObject target)
    {
        target.GetComponent<IModdable>()?.AddMod(m_ability,5f);
    }
}

public class ConfusePotion : GenericPotion
{
    public ConfusePotion(Recipe recipe) : base(recipe) { }

    public override void Drink(GameObject target)
    {
        target.GetComponent<IModdable>()?.AddMod(m_ability,1f);
    }
}