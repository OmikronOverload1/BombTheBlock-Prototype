using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 0)]
public class WeaponScript : ScriptableObject
{
    public WeaponType WeaponType;
    public string WeaponName;
    private float LastShootTime;
    public GameObject WeaponPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public ShootConfigScript ShootConfig;
    public ProjectileTrailConfigScript ProjectileTrailConfig;
    public GameObject HitParticlePrefab;
    public Vector3 SprintRotation;

    private MonoBehaviour WeaponMonoBehaviour;
    private GameObject WeaponModel;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;
    private Transform MuzzlePoint;
    private GameObject WeaponHolder;
    
    PlayerController PlayerController => WeaponMonoBehaviour.GetComponent<PlayerController>();
    InputHelper inputHelper => WeaponMonoBehaviour.GetComponent<InputHelper>();

    public void Spawn(Transform Parent, MonoBehaviour WeaponMonoBehaviour, Transform muzzlePoint = null)
    {
        this.WeaponMonoBehaviour = WeaponMonoBehaviour;
        LastShootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        MuzzlePoint = muzzlePoint;

        WeaponModel = GameObject.Instantiate(WeaponPrefab, Parent);
        WeaponModel.transform.localPosition = SpawnPoint;
        WeaponModel.transform.localRotation = Quaternion.Euler(SpawnRotation);

        if (muzzlePoint != null && muzzlePoint.GetComponentInChildren<ParticleSystem>() != null)
        {
            ShootSystem = muzzlePoint.GetComponentInChildren<ParticleSystem>();
        }
        else
        {
            ShootSystem = WeaponModel.GetComponentInChildren<ParticleSystem>();
        }
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.transform.position = StartPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0f)
        {
            instance.transform.position = Vector3.Lerp(StartPoint, EndPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= ProjectileTrailConfig.simSpeed * Time.deltaTime;

            yield return null;
        }
        instance.transform.position = EndPoint;

        yield return new WaitForSeconds(ProjectileTrailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Clear();
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Projectile Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = ProjectileTrailConfig.color;
        trail.material = ProjectileTrailConfig.material;
        trail.widthCurve = ProjectileTrailConfig.widthCurve;
        trail.time = ProjectileTrailConfig.duration;
        trail.minVertexDistance = ProjectileTrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public void Shoot()
    {
        if (Time.time > ShootConfig.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;
            ShootSystem.Play();
            for (int i = 0; i < ShootConfig.BulletsPerShot; i++)
            {
                Vector3 shootDirection = ShootSystem.transform.forward
                    + new Vector3(
                        Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x),
                        Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y),
                        Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z)
                    );
                shootDirection.Normalize();

                if (Physics.Raycast(
                    ShootSystem.transform.position,
                    shootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                ))
                {
                    var enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                    if (enemyHealth != null && hit.collider.gameObject)
                    {
                        enemyHealth.TakeDamage(ShootConfig.Damage);
                    }

                    if (HitParticlePrefab != null)
                    {
                        GameObject hitParticle = GameObject.Instantiate(
                            HitParticlePrefab,
                            hit.point,
                            Quaternion.LookRotation(hit.normal)
                        );
                        ParticleSystem ps = hitParticle.GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Play();
                            GameObject.Destroy(hitParticle, ps.main.duration);
                        }
                        else
                        {
                            GameObject.Destroy(hitParticle, 1f);
                        }
                    }

                    WeaponMonoBehaviour.StartCoroutine(
                        PlayTrail(ShootSystem.transform.position, hit.point, hit)
                    );
                    PlayerController.Recoil();
                    Debug.Log("Hit");
                }
                else
                {
                    WeaponMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            ShootSystem.transform.position,
                            ShootSystem.transform.position + (shootDirection * ProjectileTrailConfig.missDistance),
                            new RaycastHit()
                        )
                    );
                    PlayerController.Recoil();
                    Debug.Log("Miss");
                }
            }

            PlayShootSound();
        }
    }

    public void PlayShootSound()
    {
        if (ShootConfig.ShootSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = ShootSystem.transform.position;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = ShootConfig.ShootSound;
            audioSource.pitch = Random.Range(ShootConfig.PitchMin, ShootConfig.PitchMax);
            audioSource.spatialBlend = 0;
            audioSource.Play();
            Object.Destroy(tempAudio, ShootConfig.ShootSound.length);
            audioSource.clip = GetRandomSoundVariation();
        }
    }

    private AudioClip GetRandomSoundVariation()
    {
        if (ShootConfig.UseRandomVariations && ShootConfig.SoundVariations != null && ShootConfig.SoundVariations.Length > 0)
        {
            return ShootConfig.SoundVariations[Random.Range(0, ShootConfig.SoundVariations.Length)];
        }
        return ShootConfig.ShootSound;
    }
}