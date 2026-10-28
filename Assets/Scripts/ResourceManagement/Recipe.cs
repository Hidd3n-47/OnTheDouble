using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField]
    string Name;

    [SerializeField]
    Base[] m_bases = new Base[3];

    [SerializeField]
    IPotion OutputPotion;    
}
