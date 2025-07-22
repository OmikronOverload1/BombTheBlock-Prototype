using UnityEngine;

[CreateAssetMenu(fileName = "BulletTrailConfig", menuName = "Weapons/BulletTrailConfiguration", order = 4)]
public class ProjectileTrailConfigScript : ScriptableObject
{
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient color;

    public float missDistance = 100f;
    public float simSpeed = 100f;
}
