using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableVoxel : VoxelClump
{
    [Tooltip("The minimum tool tier required to be harvested")]
    [SerializeField] int minTier;
    [Tooltip("The tool type required to be harvested")]
    [SerializeField] HarvestTypes harvestType;
    [Tooltip("Array of item to drop with specified spawn rates")]
    [SerializeField] ObjectChance[] drops;

    public void TakeDamage(Vector3 _worldSpacePos, float _radius, HarvestTypes _toolType, int _toolTier)
    {
        if (_toolTier < minTier || _toolType != harvestType)
            return;

        if (_radius > 0) {
            List<Vector3> _voxels = GetVoxelsInRadius(_worldSpacePos, _radius);
            foreach (Vector3 _voxel in _voxels) {
                RemoveVoxel(_voxel);
                //SpawnResource(_voxel);
            }
        }
        else {
            Vector3 _voxelMapCoord = GetVoxel(_worldSpacePos);
            RemoveVoxel(_voxelMapCoord);
            SpawnResource(_voxelMapCoord);
        }
    }

    void SpawnResource(Vector3 _pos)
    {
        Debug.Log("Spawning");
        float _chance = Random.Range(.01f, 100f);
        GameObject _resourceObject = null;
        float lowestSpawn = 101f;
        foreach (ObjectChance _resource in drops) {
            if (_chance < _resource.spawnChance && _resource.spawnChance < lowestSpawn) {
                _resourceObject = _resource.resource;
                lowestSpawn = _resource.spawnChance;
            }
        }
        if (_resourceObject != null) {
            Instantiate(_resourceObject, transform.position, Quaternion.identity);
        }
    }
}
