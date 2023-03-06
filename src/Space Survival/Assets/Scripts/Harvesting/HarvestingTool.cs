using UnityEngine;

public class HarvestingTool : MeleeWeapon
{
    [Tooltip("The level tier of resource this tool can harvest")]
    [SerializeField] int tier;
    [Tooltip("The type of resource this tool can harvest")]
    [SerializeField] HarvestTypes harvestType;

    [SerializeField] bool harvestVoxel;

    HarvestableVoxel voxelChunk;
    IHarvestable harvestable;

    protected override void Attack()
    {
        base.Attack();
        //Apply damage to resource
        if (hit.collider != null) {
            //All object in radius
            if (hitRadius > 0) {
                //Harvest all surrounding harvestables
                Collider[] _colliders = Physics.OverlapSphere(hit.transform.position, hitRadius, ~0, QueryTriggerInteraction.Ignore);
                foreach (Collider _collider in _colliders) {
                    harvestable = _collider.transform.GetComponent<IHarvestable>();
                    if (harvestable != null) {
                        harvestable.TakeDamage(damage, harvestType, tier);
                    }
                }
            }

            //One object through raycast
            else {
                //Apply damage to resource
                harvestable = hit.collider.transform.GetComponent<IHarvestable>();
                if (harvestable != null) {
                    harvestable.TakeDamage(damage, harvestType, tier);
                }
            }

            if (harvestVoxel) {
                //Harvest all voxels in radius
                if (harvestVoxel) {
                    //Get chunk reference
                    voxelChunk = hit.transform.GetComponent<HarvestableVoxel>();
                    if (voxelChunk != null) {
                        voxelChunk.TakeDamage(hit.point - (hit.normal * .01f), hitRadius, harvestType, tier);
                    }
                }
            }
        }
    }
}
