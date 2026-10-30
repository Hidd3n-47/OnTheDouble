using UnityEngine;

[CreateAssetMenu(fileName = "ResourceType", menuName = "Scriptable Objects/ResourceType")]
public class ResourceType : ScriptableObject
{
    int spawnCount = 0;

    [SerializeField]
    string m_name;

    [SerializeField]
    Element[] m_elements = new Element[2];

    [SerializeField]
    Mesh m_mesh; 

    public Mesh mesh => m_mesh;
    public string Name => m_name;

    public Element[] Elements => m_elements;

    public static bool operator ==(ResourceType a, ResourceType b)
    {
        return (a.m_elements[0] == b.m_elements[0] || a.m_elements[0] == b.m_elements[1]) && (a.m_elements[1] == b.m_elements[0] || a.m_elements[1] == b.m_elements[1]);
    }
    public static bool operator !=(ResourceType a, ResourceType b)
    {
        return !(a == b);
    }

    public int Count => spawnCount;
    public static ResourceType operator --(ResourceType id) { id.spawnCount--; return id; }
    public static ResourceType operator ++(ResourceType id) { id.spawnCount++; return id; }

    public override string ToString()
    {
        return m_name;
    }
}
