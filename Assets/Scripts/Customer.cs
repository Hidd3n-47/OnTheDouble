using UnityEngine;

[RequireComponent(typeof(Effect))]
public class Customer : MonoBehaviour
{
    [SerializeField] private DeliveryPoint _deliveryPoint;

    // Using RequireComponent here, but can expand code to use multiple
    // effects based on situation in more complex cases
    Effect _onDeliverEffect;

    private DeliveryManager _deliveryManager;

    void Start()
    {
        _onDeliverEffect = GetComponent<Effect>();
    }

    public void Activate(DeliveryManager deliveryManager)
    {
        _deliveryManager = deliveryManager;
        _deliveryPoint.gameObject.SetActive(true);
        _deliveryPoint.RequiredPotion = deliveryManager.CurrentRecipe;
    }

    public void Deliver()
    {
        _deliveryManager.DeliverPotion();
        _deliveryPoint.gameObject.SetActive(false);

        _onDeliverEffect.Play();
    }
}
