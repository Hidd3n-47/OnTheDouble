using System.Collections;
using UnityEngine;


public class SpeedModPotion : GenericPotion
{

    public SpeedModPotion(Recipe recipe) : base(recipe)
    {

    }

    public override void Drink(GameObject target)
    {
        if(m_ability == PotionAbility.Speed)
        {
            // Speed Up Player
        }
        else // Slow
        {
            // Slow Player
        }
    }
}
