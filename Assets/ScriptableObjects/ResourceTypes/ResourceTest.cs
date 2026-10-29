using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Combo
{
    public Element[] elements = new Element[3];


    public static bool operator == (Combo a, Combo b)
    {
        if ((object)a == null || (object)b == null) return false;

        for (int i = 0; i < 3; i++)
        {
            bool found = false;
            for (int j = 0; j < 3; j++)
            {
                if (a.elements[i] == b.elements[j])
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

        if(c != null)
        {
            return this == c;
        }
        return false;
    }
    public override string ToString()
    {
        return elements[0].ToString() + ":" + elements[1].ToString() + ":" + elements[2].ToString(); 
    }

    public override int GetHashCode()
    {
        string s = "";
        for (int i = 0; i < (int)Element.Max; i++ )
        {
            for (int j = 0; j < 3; ++j)
            {
                if (elements[j] == (Element)i)
                {
                    s += elements[j].ToString();
                }
            }
        }

        return s.GetHashCode();
    }
}

public class ResourceTest : MonoBehaviour
{
    public ResourceType[] r;
    Dictionary<Combo, int> m_combos = new Dictionary<Combo, int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < r.Length; i++)
        {
            for (int j = 0; j < r.Length; j++)
            {
                if (i == j) continue;          

                if (r[i] == r[j])
                {
                    continue;
                }

                Combo c = new Combo();
                c.elements[0] = r[i].Elements[0];
                c.elements[1] = r[i].Elements[1];
                c.elements[2] = r[j].Elements[0];

                bool nonUnique = false;
                for (int a = 0; a < 3; a++)
                {
                    for (int b = a+1; b < 3; b++)
                    {
                        if (c.elements[a] == c.elements[b])
                        {
                            nonUnique=true;
                            break;
                        }
                    }
                }
                if (nonUnique) continue;

                if (m_combos.ContainsKey(c))
                {
                    m_combos[c]++;
                }
                else
                { 
                    m_combos.Add(c, 1);
                }
            }
        }

        FileStream output = File.Open("combos.txt", FileMode.OpenOrCreate);

        string write = "There are " + m_combos.Count + " combos.\n";
        Debug.Log(write);
        foreach (KeyValuePair<Combo,int> c in  m_combos)
        {
            write += c.Key.ToString() + " occurs " + c.Value + " times.\n";
            Debug.Log(c.Key.ToString() + " occurs " + c.Value + " times.");
        }
        output.Write(Encoding.Unicode.GetBytes(write), 0, Encoding.Unicode.GetByteCount(write));
        output.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
