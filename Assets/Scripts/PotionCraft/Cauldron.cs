using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is effectively a factory class. 
public class Cauldron : MonoBehaviour
{
    const int k_ingredientQuantity = 4;

    [SerializeField] ParticleSystem m_cauldromSmoke;

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

                    Color c = ElementUtils.ElementToColour(ingredient.Elements[j]);
                    m_elementOrbs[i+j].material.SetColor("_EmissionColor", c * 255);
                    m_elementOrbs[i+j].gameObject.SetActive(true);
                }
                SpawnSmoke(Color.Lerp(ElementUtils.ElementToColour(ingredient.Elements[0]), ElementUtils.ElementToColour(ingredient.Elements[1]), 0.5f), 10);

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
                    StartCoroutine(BrewPotion(m_potions[recipe.ID]));
                    break;
                }

                StartCoroutine(BrewPotion(new GenericPotion(recipe)));
                // Need to add new ones for each generic to avoid renaming potions!
                
                break;
            }
        }

        if(!brewed)
        {
            SpawnSmoke(Color.darkGray, 15);
            ResetIngredients();
            Log.Msg("Brewing Failed: " + new Combo(m_addedElements).ToString());
        }
    }

    IEnumerator BrewPotion(IPotion potion)
    {

        Color[] cols = new Color[3];
        for (int i = 0; i < 3; ++i)
        {
            cols[i] = ElementUtils.ElementToColour(m_addedElements[i]);
        }
        Color smokeCol = (cols[0] + cols[1] + cols[2])/3;

        ResetIngredients();

        float timer = 1;
        while ((timer -= Time.deltaTime) > 0)
        {
            SpawnSmoke(smokeCol, 2);
            yield return new WaitForSeconds(0.1f);
        }

        SpawnSmoke(smokeCol, 10);
        m_inventory.AddPotion(potion);


    }

    void SpawnSmoke(Color col, int count)
    {
        var param = m_cauldromSmoke.main;
        param.startColor = col;


        m_cauldromSmoke.Emit(count);
    }

    void ResetIngredients()
    {
        for (int i = 0; i < k_ingredientQuantity; ++i)
        {
            m_addedElements[i] = Element.Max;
            m_elementOrbs[i].gameObject.SetActive(false);
        }
    }
}
