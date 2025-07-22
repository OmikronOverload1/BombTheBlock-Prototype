using UnityEngine;
using static PlayerHealth;

public class EnemyHealth : MonoBehaviour

{
    [Header("Health Settings")]
    public float Health = 100f;

    [Header("Audio")]
    [SerializeField] private AudioClip hitAudioSource, deathAudioSource;
    [SerializeField] private float PitchMin = 0.8f, PitchMax = 1.2f;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    public void TakeDamage(float amount)
    {

        Health -= amount;
        Debug.Log($"Enemy took damage: {amount}. Current health: {Health}");
        PlayHitSound();

        if (Health <= 0f)
        {
            Die();
            PlayDeathSound();
        }
    }

    private void Die()
    {
        Destroy(gameObject);

        if (OnEnemyKilled != null)
        {
            OnEnemyKilled();
        }
    }

    public void PlayHitSound()
    {
        if (hitAudioSource != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = hitAudioSource;
            audioSource.pitch = Random.Range(PitchMin, PitchMax);
            audioSource.spatialBlend = 1;
            audioSource.Play();
            Object.Destroy(tempAudio, hitAudioSource.length);
        }
    }

    public void PlayDeathSound()
    {
        if (deathAudioSource != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = deathAudioSource;
            audioSource.pitch = Random.Range(PitchMin, PitchMax);
            audioSource.spatialBlend = 1;
            audioSource.Play();
            Object.Destroy(tempAudio, deathAudioSource.length);
        }
    }
}
