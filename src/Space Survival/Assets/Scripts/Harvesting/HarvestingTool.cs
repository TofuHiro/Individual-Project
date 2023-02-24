using UnityEngine;

public class HarvestingTool : MeleeWeapon
{
    [Tooltip("The level tier of resource this tool can harvest")]
    [SerializeField] int tier;
    [Tooltip("The type of resource this tool can harvest")]
    [SerializeField] HarvestTypes harvestType;

    IHarvestable harvestable;

    protected override void Attack()
    {
        base.Attack();
        //Apply damage to resource
        if (hit.collider.transform != null) {
            harvestable = hit.collider.transform.GetComponent<IHarvestable>();
            if (harvestable != null) {
                harvestable.TakeDamage(damage, harvestType, tier);
            }
        }
    }
}
