using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    const int k_resourceCap = 20;

    [SerializeField] 
    Sprite m_emptyPotionSlot;


    Dictionary<ResourceType, int> m_storedResources = new Dictionary<ResourceType, int>();
    IPotion[] m_storedPotions = new IPotion[4];

    int m_numPotions = 0;

    [SerializeField]
    Image[] m_potionIcons = new Image[4];

    [SerializeField]
    Cauldron m_cauldron;

    public int GetResourceCount(ResourceType resourceType)
    {
        if (!m_storedResources.ContainsKey(resourceType)) return 0;

        return m_storedResources[resourceType];
    }

    public void AddPotion(IPotion newPot)
    {
        if(m_numPotions < 4)
        {
            Log.Msg("Brewed: " + newPot.Name);

            m_storedPotions[m_numPotions] = newPot;
            ++m_numPotions;
        }
        else
        {
            Log.Msg("No Space - Drinking: " + newPot.Name);
        }
    }

    public void DrinkPotion(int potionLocation, GameObject user)
    {
        IPotion pot = PopPotion(potionLocation);

        if (pot != null)
        {
            Log.Msg("Drinks: " + pot.Name);
            pot.Drink(user);
        }
    }

    public IPotion DeliverPotion(int potionLocation)
    {
        Log.Msg("Delivers: " + m_storedPotions[potionLocation].Name);

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
                //m_potionIcons[i - 1].sprite = m_storedPotions[i - 1].Icon;
            }
            --m_numPotions;
            //m_potionIcons[m_numPotions].sprite = m_emptyPotionSlot;
        }

        return returnPot;
    }

    public bool AddResource(ResourceType resource)
    {
        if(m_storedResources.ContainsKey(resource))
        {
            if (m_storedResources[resource] >= k_resourceCap)
            {
                Log.Msg("Cannot Add: " + resource + ":" + m_storedResources[resource]);

                return false;
            }

            ++m_storedResources[resource];

            Log.Msg("Adds: " + resource + ":" + m_storedResources[resource]);
        }
        else
        {
            m_storedResources.Add(resource, 1);

            Log.Msg("Creates: " + resource + ":" + m_storedResources[resource]);
        }
        return true;
    }

    public void UseIngredient(ResourceType ingredient)
    {
        if(!m_storedResources.ContainsKey(ingredient)) { return; }

        if (m_storedResources[ingredient] > 0)
        {
            m_cauldron.AddIngredient(ingredient); 
            --m_storedResources[ingredient];
        }
    }

}
