using UnityEngine;

public class GenericPotion : IPotion
{
    protected string m_name;
    protected Sprite m_icon;
    protected PotionAbility m_ability;

    public string Name => m_name;
    public PotionAbility ID => m_ability;
    public Sprite Icon => m_icon;

    public virtual void Drink(GameObject target)
    {
        // Do any effects and whatnot. 
        // example implementation:
        // target.GetComponent<PotionFX>().DefaultDrink();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GenericPotion(Recipe recipe)
    {
        m_name = recipe.Name;
        m_icon = recipe.Icon;
        m_ability = recipe.ID;
    }
}
