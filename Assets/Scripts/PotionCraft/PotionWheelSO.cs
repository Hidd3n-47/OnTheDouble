using UnityEngine;

[CreateAssetMenu(fileName = "PotionWheelOptions", menuName = "Scriptable Objects/PotionWheelOptions")]
public class PotionWheelSO : ScriptableObject
{
    [SerializeField] public ResourceType[] resources;
}
