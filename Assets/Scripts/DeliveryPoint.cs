using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    Customer _parent;

    public Recipe RequiredPotion { get; set; }

    void Start()
    {
        _parent = GetComponentInParent<Customer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponentInParent<Inventory>();

        if (inventory.DeliverPotion(RequiredPotion))
        {
            _parent.Deliver();
        }
    }
}
