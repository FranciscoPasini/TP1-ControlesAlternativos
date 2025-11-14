using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform carryPoint;  // arrastrá acá tu hijo "UP" en el inspector
    private PickupItem carriedItem = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (carriedItem == null && collision.CompareTag("Pickup"))
        {
            PickupItem item = collision.GetComponent<PickupItem>();
            if (item != null && !item.IsPickedUp)
            {
                carriedItem = item;
                item.PickUp(carryPoint); //  ahora se adjunta al hijo exacto
            }
        }
    }

    public void DropItem(bool correctZone)
    {
        if (carriedItem != null)
        {
            carriedItem.Deliver(correctZone);
            carriedItem = null;
        }
    }
}
