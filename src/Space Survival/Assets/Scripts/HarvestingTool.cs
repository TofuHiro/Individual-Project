using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingTool : MeleeWeapon
{
    [SerializeField] HarvestTiers tier;
    [SerializeField] HarvestTypes harvestType;

    IHarvestable harvestable;

    protected override void Attack()
    {
        base.Attack();
        if (hit.transform != null) {
            harvestable = hit.transform.GetComponent<IHarvestable>();
            if (harvestable != null) {
                harvestable.TakeDamage(damage, harvestType, tier);
            }
        }
    }
}
