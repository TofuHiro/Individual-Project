using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour, IOperable
{
    [SerializeField] WeaponType weaponType;

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public virtual void Use()
    {
        //
    }
}
