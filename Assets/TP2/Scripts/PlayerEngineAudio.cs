using UnityEngine;

public class PlayerEngineAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip forwardEngineClip;
    [SerializeField] private AudioClip reverseEngineClip;

    // Para detectar dirección real de movimiento
    private Vector3 lastPosition;
    private float movementSpeed;

    private enum MovementDirection { Idle, Forward, Reverse }
    private MovementDirection currentDirection = MovementDirection.Idle;

    private void Start()
    {
        lastPosition = transform.position;
        audioSource.loop = true;
    }

    private void Update()
    {
        DetectRealMovement();
        HandleEngineSound();
    }

    private void DetectRealMovement()
    {
        // Velocidad = cuanto se movió el jugador desde el frame anterior
        Vector3 delta = transform.position - lastPosition;
        movementSpeed = delta.magnitude;

        // Calculamos si avanza hacia adelante o hacia atrás
        float dot = Vector3.Dot(transform.up, delta.normalized);

        if (movementSpeed < 0.01f)
        {
            currentDirection = MovementDirection.Idle;
        }
        else if (dot > 0.1f)
        {
            currentDirection = MovementDirection.Forward;
        }
        else if (dot < -0.1f)
        {
            currentDirection = MovementDirection.Reverse;
        }

        lastPosition = transform.position;
    }

    private void HandleEngineSound()
    {
        switch (currentDirection)
        {
            case MovementDirection.Forward:
                PlayClip(forwardEngineClip);
                break;

            case MovementDirection.Reverse:
                PlayClip(reverseEngineClip);
                break;

            case MovementDirection.Idle:
                audioSource.Stop();
                break;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
