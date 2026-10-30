using UnityEngine;

public class Combo
{
    public Element[] m_elements = new Element[3];

    public Combo(Element[] elements)
    {
        for (int i = 0; i < Mathf.Min(elements.Length, m_elements.Length); i++)
        {
            m_elements[i] = elements[i];
        }
    }

    // Legacy for ResourceTest - use the other ctor!
    public Combo() { }

    public static bool operator ==(Combo a, Combo b)
    {
        if ((object)a == null || (object)b == null) return false;

        for (int i = 0; i < 3; i++)
        {
            bool found = false;
            for (int j = 0; j < 3; j++)
            {
                if (a.m_elements[i] == b.m_elements[j])
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }
    public static bool operator !=(Combo a, Combo b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        Combo c = obj as Combo;

        if (c != null)
        {
            return this == c;
        }
        return false;
    }
    public override string ToString()
    {
        return ElementUtils.ElementToName(m_elements[0]) + ":" + ElementUtils.ElementToName(m_elements[1]) + ":" + ElementUtils.ElementToName(m_elements[2]);
    }

    public override int GetHashCode()
    {
        string s = "";
        for (int i = 0; i < (int)Element.Max; i++)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (m_elements[j] == (Element)i)
                {
                    s += ElementUtils.ElementToName(m_elements[j]);
                }
            }
        }

        return s.GetHashCode();
    }
}