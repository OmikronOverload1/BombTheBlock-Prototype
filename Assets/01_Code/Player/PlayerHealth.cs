using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 100f;
    public float currentHealth;
    public Slider healthSlider;

    [Header("Audio")]
    [SerializeField] private AudioClip hitAudioSource, deathAudioSource;
    [SerializeField] private float pitchMin = 0.8f, pitchMax = 1.2f;

    

    private void Start()
    {
        currentHealth = health;
    }

    void Update()
    {
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took damage: {amount}. Current health: {currentHealth}");
        PlayHitSound();

        if (currentHealth <= 0f)
        {
            Die();
            PlayDeathSound();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

    }

    private void Die()
    {

        Destroy(gameObject);

    }

    public void PlayHitSound()
    {
        if (hitAudioSource != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = hitAudioSource;
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.spatialBlend = 1;
            audioSource.Play();
            Destroy(tempAudio, hitAudioSource.length);
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
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.spatialBlend = 1;
            audioSource.Play();
            Destroy(tempAudio, deathAudioSource.length);
        }
    }
}