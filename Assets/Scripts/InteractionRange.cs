using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractionRange : MonoBehaviour
{
    public UnityEvent OnInteractionRangeEnter;

    private void OnTriggerEnter(Collider other)
    {
        OnInteractionRangeEnter?.Invoke();
    }
}
