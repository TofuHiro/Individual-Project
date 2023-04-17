using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableVoxel : VoxelClump, IHarvestableVoxel
{
    [Header("Harvesting")]
    [Tooltip("The minimum tool tier required to be harvested")]
    [SerializeField] int minTier;
    [Tooltip("The tool type required to be harvested")]
    [SerializeField] HarvestTypes harvestType;
    [Tooltip("Array of item to drop with specified spawn rates")]
    [SerializeField] ObjectChance[] drops;
    [Tooltip("Visual effects to play on harvest")]
    [SerializeField] string[] harvestEffects;
    [Tooltip("Sound effects to play on harvest. To use a random varient, input just the tag without the number")]
    [SerializeField] string[] harvestSounds;

    EffectsManager effectsManager;
    AudioManager audioManager;

    protected override void Start()
    {
        base.Start();
        effectsManager = EffectsManager.Instance;
        audioManager = AudioManager.Instance;
    }

    public HarvestTypes GetHarvestType()
    {
        return harvestType;
    }

    public int GetMinTier()
    {
        return minTier;
    }

    /// <summary>
    /// Harvests voxels within a radius around a point
    /// </summary>
    /// <param name="_worldSpacePos">The point to harvest around</param>
    /// <param name="_radius">The radius to harvest from. If set to 0, only harvest single target at point</param>
    /// <param name="_toolType">The tool type used to harvest</param>
    /// <param name="_toolTier">The tier of the tool used to harvest</param>
    public void Harvest(Vector3 _worldSpacePos, float _radius, HarvestTypes _toolType, int _toolTier)
    {
        if (_toolTier < minTier || _toolType != harvestType)
            return;

        //Harvest effects
        foreach (string _effect in harvestEffects) {
            effectsManager.PlayEffect(_effect, _worldSpacePos, transform.rotation);
        }
        //Sound
        foreach (string _audio in harvestSounds) {
            audioManager.PlayClip(_audio, _worldSpacePos);
        }

        if (_radius > 0) {
            List<Vector3> _voxels = GetVoxelsInRadius(_worldSpacePos, _radius);
            RemoveVoxels(_voxels);
            foreach (Vector3 _voxel in _voxels) {
                SpawnResource(_voxel + transform.position);
            }
        }
        else {
            Vector3 _voxelMapCoord = GetVoxel(_worldSpacePos);
            RemoveVoxel(_voxelMapCoord);
            SpawnResource(_voxelMapCoord + transform.position);
        }
    }

    /// <summary>
    /// Spawns the set resource
    /// </summary>
    /// <param name="_pos">The position to spawn at</param>
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
            Quaternion _randRot = Quaternion.Euler(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f)));
            ObjectPooler.SpawnObject(_newObject.nameTag, _newObject.resource, _pos, _randRot);
        }
    }
}
