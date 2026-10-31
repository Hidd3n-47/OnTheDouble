using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private DeliveryPoint _deliveryPoint;

    private DeliveryManager _deliveryManager;

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
    }
}
