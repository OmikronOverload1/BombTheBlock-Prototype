using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponSelector : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField]
    private WeaponType Weapon;
    [SerializeField]
    private Transform WeaponParent;
    [SerializeField]
    private List<WeaponScript> WeaponList;
    [SerializeField]
    private Transform MuzzlePoint;

    [Space]
    [Header("Runtime Filled")]
    public WeaponScript ActiveWeapon;



    private void Start()
    {
        WeaponScript weapon = WeaponList.Find(weapon => weapon.WeaponType == Weapon);

        if (weapon == null)
        {
            Debug.LogError($"Weapon of type {Weapon} was not found in the list.");
            return;
        }

        ActiveWeapon = weapon;
        ActiveWeapon.Spawn(WeaponParent, this, MuzzlePoint);
    }
}
