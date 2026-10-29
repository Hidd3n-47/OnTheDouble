using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    const int k_resourceCap = 20;

    [SerializeField] Sprite m_emptyPotionSlot;


    Dictionary<ResourceType, int> m_storedResources = new Dictionary<ResourceType, int>();
    IPotion[] m_storedPotions = new IPotion[4];

    int m_numPotions = 0;
    Image[] m_potionIcons = new Image[4];


    public void AddPotion(IPotion newPot)
    {
        if(m_numPotions < 4)
        {
            m_storedPotions[m_numPotions] = newPot;
            m_potionIcons[m_numPotions].sprite = newPot.Icon;
            ++m_numPotions;
        }
    }

    public void DrinkPotion(int potionLocation)
    {
        IPotion pot = PopPotion(potionLocation);

        if (pot != null)
        {
            pot.Drink();
        }
    }

    public IPotion DeliverPotion(int potionLocation)
    {
        return PopPotion(potionLocation);
    }

    IPotion PopPotion(int potionLocation)
    {
        IPotion returnPot = null;
        if(potionLocation < m_numPotions)
        {
            returnPot = m_storedPotions[potionLocation];
            
            for(int i = potionLocation+1; i < m_numPotions; ++potionLocation)
            {
                m_storedPotions[i - 1] = m_storedPotions[i];
                m_potionIcons[i - 1].sprite = m_storedPotions[i - 1].Icon;
            }
            --m_numPotions;
            m_potionIcons[m_numPotions].sprite = m_emptyPotionSlot;
        }

        return returnPot;
    }

    public bool AddResource(Resource resource)
    {
        if(m_storedResources.ContainsKey(resource.Type))
        {
            if (m_storedResources[resource.Type] >= k_resourceCap)
            {
                return false;
            }

            ++m_storedResources[resource.Type];
        }
        else
        {
            m_storedResources.Add(resource.Type, 1);
        }
        return true;
    }

}
