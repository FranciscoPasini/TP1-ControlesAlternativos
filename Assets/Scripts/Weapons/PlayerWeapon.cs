using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("🎯 El arma golpeó a: " + other.name);

        ZombieHealth zombie = other.GetComponent<ZombieHealth>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage); // Solo daño, sin dirección
            Debug.Log($"💥 {other.name} entró en Stun");
        }
        else
        {
            Debug.Log("❌ " + other.name + " no tiene ZombieHealth");
        }
    }
}