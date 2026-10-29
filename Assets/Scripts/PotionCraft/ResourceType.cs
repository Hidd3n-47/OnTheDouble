using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class ResourceType : ScriptableObject
{
    [SerializeField]
    string m_name;

    [SerializeField]
    Element[] m_elements = new Element[2];

    [SerializeField]
    Mesh m_mesh; 

    public Mesh mesh => m_mesh;
    public string Name => m_name;
}
