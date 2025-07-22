using UnityEngine;


[CreateAssetMenu(fileName = "ShootConfig", menuName = "Weapons/ShootConfiguration", order = 2)]
public class ShootConfigScript : ScriptableObject
{
    [Header("Weapon Settings")]
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;
    public float Damage = 3f;
    public float Recoil = 5f;
    public float RecoilRecovery = 1.4f;
    public int BulletsPerShot = 1;

    [Header("Audio Settings")]
    public AudioClip ShootSound;
    public AudioClip[] SoundVariations;

    [Header("Audio Effects")]
    [Range(0.5f, 2.0f)]
    public float PitchMin = 0.9f;
    [Range(0.5f, 2.0f)]
    public float PitchMax = 1.1f;

    [Range(0.1f, 1.0f)]
    public float VolumeMin = 0.8f;
    [Range(0.1f, 1.0f)]
    public float VolumeMax = 1.0f;

    [Header("Advanced Audio Effects")]
    public bool UseLowPassFilter = false;
    public bool UseReverbFilter = false;
    public bool UseRandomVariations = false;
}
