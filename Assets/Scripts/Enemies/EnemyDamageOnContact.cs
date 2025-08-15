// EnemyDamageOnContact.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldownPerTarget = 0.5f;
    [SerializeField] private string playerTag = "Player";

    private readonly Dictionary<PlayerHealth, float> _nextHit = new();

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        float now = Time.time;
        if (!_nextHit.TryGetValue(health, out float allowedAt)) allowedAt = 0f;

        if (now >= allowedAt)
        {
            health.TakeDamage(damage);
            _nextHit[health] = now + cooldownPerTarget;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        var health = other.GetComponent<PlayerHealth>();
        if (health != null && _nextHit.ContainsKey(health))
            _nextHit.Remove(health);
    }
}
