using System.Collections;
using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    [Header("Audio Layers")]
    public AudioSource layer1;
    public AudioSource layer2;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    [Header("Enemy Detection")]
    public float detectionRadius = 20f;
    public bool enemyDetected = false;

    void Start()
    {
        if (layer1 != null)
        {
            layer1.volume = 0.5f;
            layer1.Play();
        }
        if (layer2 != null)
        {
            layer2.volume = 0.0f;
            layer2.Play();
        }
    }

    void Update()
    {
        EnemyInRange();
    }

    /// <summary>
    /// Blendet einen Layer ein.
    /// </summary>
    /// <param name="layerIndex">Index des Layers (1 bis 4)</param>
    public void ActivateLayer(int layerIndex)
    {
        AudioSource source = GetLayer(layerIndex);
        if (source != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeVolume(source, 0.5f, fadeDuration));
        }
    }

    /// <summary>
    /// Blendet einen Layer aus.
    /// </summary>
    /// <param name="layerIndex">Index des Layers (1 bis 4)</param>
    public void DeactivateLayer(int layerIndex)
    {
        AudioSource source = GetLayer(layerIndex);
        if (source != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeVolume(source, 0.0f, fadeDuration));
        }
    }

    private AudioSource GetLayer(int index)
    {
        return index switch
        {
            1 => layer1,
            2 => layer2,
            _ => null
        };
    }

    private IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;
        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        source.volume = targetVolume;
    }

    void EnemyInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool enemyFound = false;

        Debug.Log($"Found {hitColliders.Length} colliders in detection sphere");

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Checking collider: {hitCollider.name} with tag: {hitCollider.tag}");

            if (hitCollider.CompareTag("Enemy"))
            {
                enemyFound = true;
                Debug.Log("Enemy detected!");
                break;
            }
        }

        if (enemyFound != enemyDetected)
        {
            if (enemyFound)
            {
                enemyDetected = true;
                ActivateLayer(2);
                Debug.Log("Switching to combat music");
            }
            else
            {
                enemyDetected = false;
                DeactivateLayer(2);
                Debug.Log("Switching to ambient music");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = enemyDetected ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}