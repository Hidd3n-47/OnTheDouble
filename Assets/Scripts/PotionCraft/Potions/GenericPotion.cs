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
    public Combo Combo { get; set; }

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
    }

    // Sorry Matt, pretend you don't see this and it's all good.
    public GenericPotion(Recipe recipe, Combo combo)
    {
        m_name     = recipe.Name;
        m_icon     = recipe.Icon;
        m_ability  = recipe.ID;
        this.Combo = combo;
    }
}
