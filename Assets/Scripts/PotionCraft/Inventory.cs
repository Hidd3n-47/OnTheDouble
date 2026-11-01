using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    const int k_resourceCap = 20;

    [SerializeField] 
    Sprite m_emptyPotionSlot;

    readonly Dictionary<ResourceType, int> m_storedResources = new Dictionary<ResourceType, int>();
    readonly IPotion[] m_storedPotions = new IPotion[4];

    int m_numPotions = 0;

    [SerializeField]
    Image[] m_potionIcons = new Image[4];

    [SerializeField]
    Cauldron m_cauldron = null;

    public System.Action IngredientCountChanged {get; set;}

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
            newPot.Drink(gameObject);
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

    public bool DeliverPotion(Recipe desiredRecipe)
    {
        Combo combo = new Combo(desiredRecipe.Elements);

        for (int i = 0; i < m_numPotions; ++i)
        {
            if (m_storedPotions[i].Combo == combo)
            {
                PopPotion(i);
                Debug.Log("Delivered.");
                return true;
            }
        }

        Debug.Log("Not delivered as don't have the correct pot :(");
        return false;
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
                SetPotionIcon(i-1, m_storedPotions[i - 1].Icon);
            }
            --m_numPotions;
            SetPotionIcon(m_numPotions, m_emptyPotionSlot);
        }

        return returnPot;
    }

    void SetPotionIcon(int index, Sprite sprite)
    {
        if(m_potionIcons[index] != null)
        {
            m_potionIcons[index].sprite = sprite;
        }
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

        IngredientCountChanged();
        return true;
    }

    public void UseIngredient(ResourceType ingredient)
    {
        if(!m_storedResources.ContainsKey(ingredient)) { return; }

        if (m_storedResources[ingredient] > 0)
        {
            m_cauldron.AddIngredient(ingredient); 
            --m_storedResources[ingredient];
            IngredientCountChanged();
        }
    }

    public void DropRandomIngredients(int quantity)
    {
        List<ResourceType> ingredients = m_storedResources.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key).ToList();
        while (quantity > 0 && ingredients.Count > 0)
        {
            int idx = Random.Range(0, ingredients.Count);
            --m_storedResources[ingredients[idx]];
            --quantity;

            if(m_storedResources[ingredients[idx]] <= 0)
            {
                ingredients.Remove(ingredients[idx]);
            }
            
            // TODO: Some spill effect!
        }
        IngredientCountChanged();
    }
}
