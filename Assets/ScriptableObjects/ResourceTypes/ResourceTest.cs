using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ResourceTest : MonoBehaviour
{
    public ResourceType[] r;
    Dictionary<Combo, int> m_combos = new Dictionary<Combo, int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    Inventory inv;

    void Start()
    {
        TestInventorySystem();
        //TestCombosFromResources();
    }

    void TestInventorySystem()
    {
        Log.Msg("Testing Inventory!");

        for (int i = 0; i < 100; ++i)
        {
            inv.AddResource(r[Random.Range(0, r.Length)]);
        }

        for (int i = 0; i < 90; ++i)
        {
            inv.UseIngredient(r[Random.Range(0, r.Length)]);
        }
    }

    // Update is called once per frame
    void TestCombosFromResources()
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
                c.m_elements[0] = r[i].Elements[0];
                c.m_elements[1] = r[i].Elements[1];
                c.m_elements[2] = r[j].Elements[0];

                bool nonUnique = false;
                for (int a = 0; a < 3; a++)
                {
                    for (int b = a + 1; b < 3; b++)
                    {
                        if (c.m_elements[a] == c.m_elements[b])
                        {
                            nonUnique = true;
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
        foreach (KeyValuePair<Combo, int> c in m_combos)
        {
            write += c.Key.ToString() + " occurs " + c.Value + " times.\n";
            Debug.Log(c.Key.ToString() + " occurs " + c.Value + " times.");
        }
        output.Write(Encoding.Unicode.GetBytes(write), 0, Encoding.Unicode.GetByteCount(write));
        output.Close();
    }
}
