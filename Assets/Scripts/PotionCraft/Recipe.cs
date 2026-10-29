using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{

    [SerializeField]
    string m_name;

    [SerializeField]
    Element[] m_elements = new Element[3];

    [SerializeField]
    IPotion m_outputPotion;

    public string Name => m_name;
    public Element[] Elements => m_elements;


}
