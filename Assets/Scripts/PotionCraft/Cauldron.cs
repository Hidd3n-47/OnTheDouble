using System.Collections.Generic;
using UnityEngine;

// This is effectively a factory class. 
public class Cauldron : MonoBehaviour
{
    const int k_ingredientQuantity = 4;

    [SerializeField]
    Recipe[] m_recipes = new Recipe[0];
    Dictionary<PotionAbility, IPotion> m_potions = new Dictionary<PotionAbility, IPotion>();

    [SerializeField]
    Inventory m_inventory;

    Element[] m_addedElements = new Element[k_ingredientQuantity] { Element.Max, Element.Max, Element.Max, Element.Max };

    [SerializeField]
    MeshRenderer[] m_elementOrbs = new MeshRenderer[k_ingredientQuantity];

    void Start()
    {
        // TODO: Add to Dictionary every Potion Created
        for (int i = 0; i < m_recipes.Length; ++i)
        {
            switch (m_recipes[i].ID)
            {
                case PotionAbility.Speed:
                case PotionAbility.Slow:
                    m_potions.Add(m_recipes[i].ID, new SpeedModPotion(m_recipes[i]));
                    break;
                case PotionAbility.Magnetism:
                    break;
                case PotionAbility.Butterfingers:
                    break;
                case PotionAbility.VisionBoost:
                case PotionAbility.Blinkers:
                    break;
                case PotionAbility.Stun:
                    break;
                default:
                    break;
            }

        }
    }


    public bool AddIngredient(ResourceType ingredient)
    {
        int i = 0;
        for ( ; i < k_ingredientQuantity; i+=2)
        {
            if (m_addedElements[i] == Element.Max)
            {
                Log.Msg("Simmers: " + ingredient.Name);
                for (int j = 0; j < 2; ++j)
                {
                    m_addedElements[i+j] = ingredient.Elements[j];
                    m_elementOrbs[i+j].material.SetColor("_BaseColor", ElementUtils.ElementToColour(ingredient.Elements[j]));
                    m_elementOrbs[i+j].gameObject.SetActive(true);
                }
                i += 2;
                break;
            }
            else
            {
                bool dupe = true;
                for (int j = 0; j < 2; ++j)
                {
                    if (m_addedElements[i + j] == ingredient.Elements[j])
                    {
                        continue;
                    }
                    dupe = false;
                }
                if( dupe )
                { 
                    Log.Msg("Cannot add Duplicate: " + ingredient.Name);
                    return false;
                }
            }
        }
        if(i == k_ingredientQuantity)
        {
            Brew();
        }
        return true;
    }

    void Brew()
    {
        // TODO: Coroutine or call out to any visual/audio behaviours 

        bool brewed = false;
        foreach (Recipe recipe in m_recipes)
        {
            Combo currentBrew = new Combo(m_addedElements);
            if ( recipe == currentBrew )
            {
                brewed = true;
                Log.Msg("Brews: " + recipe.Name);
                if (m_potions.ContainsKey(recipe.ID))
                {
                    m_inventory.AddPotion(m_potions[recipe.ID]);
                    break;
                }
                // Need to add new ones for each generic to avoid renaming potions!
                m_inventory.AddPotion(new GenericPotion(recipe));
                break;
            }
        }

        if(!brewed)
        {
            Log.Msg("Brewing Failed: " + new Combo(m_addedElements).ToString());
        }

        for (int i = 0; i < k_ingredientQuantity; ++i)
        {
            m_addedElements[i] = Element.Max;
            m_elementOrbs[i].gameObject.SetActive(false);
        }
    }
}
