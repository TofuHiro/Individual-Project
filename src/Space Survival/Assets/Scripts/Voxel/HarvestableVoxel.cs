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
                SpawnResource(_voxel + transform.position);
            }
        }
        else {
            Vector3 _voxelMapCoord = GetVoxel(_worldSpacePos);
            RemoveVoxel(_voxelMapCoord);
            SpawnResource(_voxelMapCoord + transform.position);
        }
    }

    void SpawnResource(Vector3 _pos)
    {
        float _chance = Random.Range(.01f, 100f);
        ObjectChance _newObject = new ObjectChance
        {
            spawnChance = 100f
        };
        foreach (ObjectChance _resource in drops) {
            if (_chance <= _resource.spawnChance && _resource.spawnChance <= _newObject.spawnChance) {
                _newObject = _resource;
            }
        }
        if (_newObject.resource != null) {
            ObjectPooler.SpawnObject(_newObject.nameTag, _newObject.resource, _pos, Quaternion.identity, Vector3.one / voxelPerUnit);
        }
    }
}
