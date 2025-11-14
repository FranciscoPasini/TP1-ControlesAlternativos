using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemType itemType;
    [HideInInspector] public bool IsPickedUp = false;

    public int points = 1;
    public int penalty = 1;
    public float timeBonus = 2f;
    public float timePenalty = 2f;

    private Transform playerTransform;

    public void PickUp(Transform player)
    {
        if (IsPickedUp) return;

        IsPickedUp = true;
        playerTransform = player;

        // Parentarlo para que siga al jugador sin Update
        transform.SetParent(playerTransform);

        // Ajustar posición donde vos quieras que el objeto se vea
        transform.localPosition = new Vector3(0, -0.8f, 0);
    }

    private void Update()
    {
        // Ya no hace falta moverlo manualmente, el parent lo hace
    }

    public void Deliver(bool correctZone)
    {
        if (correctZone)
        {
            GameController.Instance.AddPoints(points, timeBonus);
        }
        else
        {
            GameController.Instance.SubtractPoints(penalty, timePenalty);
        }

        Destroy(gameObject);
    }
}
