using UnityEngine;

[CreateAssetMenu(fileName = "ResourceWheelOptions", menuName = "Scriptable Objects/ResourceWheelOptions")]
public class ResourceWheelSO : ScriptableObject
{
    [SerializeField] public ResourceType[] resources;
}
