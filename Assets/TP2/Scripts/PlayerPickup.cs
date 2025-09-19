using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private PickupItem carriedItem = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (carriedItem == null && collision.CompareTag("Pickup"))
        {
            PickupItem item = collision.GetComponent<PickupItem>();
            if (item != null && !item.IsPickedUp)
            {
                carriedItem = item;
                item.PickUp(transform);
            }
        }
    }
}
