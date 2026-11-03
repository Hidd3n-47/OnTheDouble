using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is effectively a factory class. 
public class Cauldron : MonoBehaviour
{
    const int k_ingredientQuantity = 4;

    [SerializeField] 
    ParticleSystem m_cauldronSmoke;

    [SerializeField]
    Recipe[] m_recipes = new Recipe[0];

    [SerializeField]
    Inventory m_inventory;

    [SerializeField]
    MeshRenderer[] m_elementOrbs = new MeshRenderer[k_ingredientQuantity];

    [SerializeField]
    Effect m_potionCreatedEffect;

    readonly Dictionary<PotionAbility, IPotion> m_potions = new Dictionary<PotionAbility, IPotion>();
    readonly Element[] m_addedElements = new Element[k_ingredientQuantity] { Element.Max, Element.Max, Element.Max, Element.Max };
    readonly Queue<IPotion> m_potionBacklog = new Queue<IPotion>();
    bool m_brewing = false;

    void Start()
    {
        // TODO: Add to Dictionary every Potion Created
        for (int i = 0; i < m_recipes.Length; ++i)
        {
            IPotion potion = null;
            Recipe recipe = m_recipes[i];
            switch (recipe.ID)
            {
                case PotionAbility.Speed:
                case PotionAbility.Slow:
                    potion = new SpeedModPotion(recipe);
                    break;
                case PotionAbility.Magnetism:
                    potion = new MagnetismPotion(recipe);
                    break;
                case PotionAbility.Butterfingers:
                    potion = new ButterfingersPotion(recipe);
                    break;
                case PotionAbility.VisionBoost:
                case PotionAbility.Blinkers:
                    potion = new VisionModPotion(recipe);
                    break;
                case PotionAbility.Confuse:
                    potion = new ConfusePotion(recipe);
                    break;
                default:
                    break;
            }
            if(potion != null && !m_potions.ContainsKey(recipe.ID)) m_potions.Add(m_recipes[i].ID, potion);
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
        foreach (Recipe recipe in m_recipes)
        {
            Combo currentBrew = new Combo(m_addedElements);
            if ( recipe == currentBrew )
            {
                if (m_potions.ContainsKey(recipe.ID))
                {
                    Enqueue(m_potions[recipe.ID]);
                    return;
                }

                Enqueue(new GenericPotion(recipe));
                return;
            }
        }

        SpawnSmoke(Color.darkGray, 15);
        Log.Msg("Brewing Failed: " + new Combo(m_addedElements).ToString());
        ResetIngredients();
    }

    void Enqueue(IPotion potion)
    {
        Log.Msg("Queued: " + potion.Name);
        m_potionBacklog.Enqueue(potion);
        if(!m_brewing)
        {
            StartCoroutine(BrewPotion());
        }
    }

    IEnumerator BrewPotion()
    {
        const float brewTime = 1f;
        const float smokeTime = 0.1f;
        const float newPotionDelay = 1f;

        m_brewing = true;
        while(m_potionBacklog.Count > 0)
        {            
            IPotion potion = m_potionBacklog.Dequeue();
            Combo recipe = potion.Combo;
            ResetIngredients();
            Color[] cols = new Color[3];
            for (int i = 0; i < 3; ++i)
            {
                cols[i] = ElementUtils.ElementToColour(recipe[i]);
            }
            Color smokeCol = (cols[0] + cols[1] + cols[2])/3;

            float timer = brewTime;
            while ((timer -= smokeTime) > 0)
            {
                SpawnSmoke(smokeCol, 2);
                yield return new WaitForSeconds(smokeTime);
            }

            m_potionCreatedEffect.Play();
            m_inventory.AddPotion(potion);

            yield return new WaitForSeconds(newPotionDelay);
        }
        m_brewing = false;
    }

    void SpawnSmoke(Color col, int count)
    {
        ParticleSystem.MainModule param = m_cauldronSmoke.main;
        param.startColor = col;

        m_cauldronSmoke.Emit(count);
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
