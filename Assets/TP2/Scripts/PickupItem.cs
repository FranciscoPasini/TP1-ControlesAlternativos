using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemType itemType;           // Tipo de este objeto
    [HideInInspector] public bool IsPickedUp = false;

    public int points = 1;              // Puntos al entregar correctamente
    public int penalty = 1;             // Puntos al entregar en zona incorrecta
    public float timeBonus = 2f;        // Segundos al entregar correctamente
    public float timePenalty = 2f;      // Segundos al entregar en zona incorrecta

    private Transform playerTransform;  // Para seguir al jugador mientras lo lleva

    public void PickUp(Transform player)
    {
        if (IsPickedUp) return;
        IsPickedUp = true;
        playerTransform = player;
        transform.SetParent(playerTransform);
        transform.localPosition = Vector3.up; // ajusta según donde quieras que esté
    }

    private void Update()
    {
        // Si está recogido, sigue la posición del jugador
        if (IsPickedUp && playerTransform != null)
        {
            transform.position = playerTransform.position + Vector3.up; // offset visual
        }
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
