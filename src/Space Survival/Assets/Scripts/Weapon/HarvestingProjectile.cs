using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingProjectile : Projectile
{
    [Tooltip("The level tier of resource this tool can harvest")]
    [SerializeField] int tier;
    [Tooltip("The type of resource this tool can harvest")]
    [SerializeField] HarvestTypes harvestType;
    [Tooltip("If this tool can harvest voxels")]
    [SerializeField] bool harvestVoxel;

    protected override void Explode()
    {
        //Get nearby objects
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider _collider in _colliders) {
            IHarvestable _harvestable = _collider.transform.GetComponent<IHarvestable>();
            if (_harvestable != null) {
                _harvestable.TakeDamage(damage, harvestType, tier);
            }
        }

        //Harvest all voxels in radius
        if (harvestVoxel) {
            foreach (Collider _collder in _colliders) {
                IHarvestableVoxel _voxelChunk = _collder.GetComponent<IHarvestableVoxel>();
                if (_voxelChunk != null) {
                    _voxelChunk.Harvest(transform.position, explosionRadius, harvestType, tier);
                }
            }
        }
       
        base.Explode();
    }
}
