using System.Collections.Generic;
using UnityEngine;

public class PotionTest : MonoBehaviour
{
    const float k_timeBetweenPotions = 10f;

    [SerializeField] Inventory inv;
    [SerializeField] Recipe[] m_recipes = new Recipe[0];
    readonly List<IPotion> m_potions = new List<IPotion>();
    float m_timer = k_timeBetweenPotions;

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
            if(potion != null && !m_potions.Contains(potion)) m_potions.Add(potion);
        }
    }

    void Update()
    {
        if(m_timer < Time.time)
        {
            int index = Random.Range(0,m_potions.Count);
            inv.AddPotion(m_potions[index]);
            inv.DrinkPotion(0, inv.gameObject);

            Log.Msg(m_timer.ToString() + ": Drank a" + m_potions[index].Name + " Effect: " + m_potions[index].ID.ToString());

            m_timer = Time.time + k_timeBetweenPotions;
        }
    
    }
}