using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{

    [SerializeField]
    string m_name;

    [Header ("Mostly to use for reference unless we find a place for it.")]
    [SerializeField]
    string m_description;

    [SerializeField]
    Element[] m_elements = new Element[3];

    [SerializeField]
    Sprite m_potionIcon;

    [SerializeField]
    PotionAbility m_potionAbility = 0;

    public string Name => m_name;
    public PotionAbility ID => m_potionAbility;
    public Sprite Icon => m_potionIcon;
    public Element[] Elements => m_elements;

    // Look. I didn't wanna refactor it ok...
    public static bool operator ==(Recipe lhs, Combo rhs)
    {
        Combo c = new Combo(lhs.Elements);
        return c == rhs;
    }
    public static bool operator !=(Recipe lhs, Combo rhs)
    {
        return !(lhs == rhs);
    }
}
