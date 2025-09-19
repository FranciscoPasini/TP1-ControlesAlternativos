using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    public ItemType acceptedType;  // Tipo de objeto que acepta esta zona

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PickupItem item = collision.GetComponent<PickupItem>();
        if (item != null && item.IsPickedUp)
        {
            bool correct = (item.itemType == acceptedType);
            item.Deliver(correct);
        }
    }
}
