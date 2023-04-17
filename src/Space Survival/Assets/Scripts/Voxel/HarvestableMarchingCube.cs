using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableMarchingCube : MarchingCube, IHarvestableVoxel
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
    [Tooltip("Sound effects to play on harvest")]
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
            List<Vector3> _verts = GetVertsInRadius(_worldSpacePos, _radius);
            RemoveVerts(_verts);
            foreach (Vector3 _vert in _verts) {
                SpawnResource(_vert + transform.position);
            }
        }
        else {
            Vector3 _vertMapCoord = GetVert(_worldSpacePos);
            RemoveVert(_vertMapCoord);
            SpawnResource(_vertMapCoord + transform.position);
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
            Quaternion _randRot = Quaternion.Euler(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f)));
            ObjectPooler.SpawnObject(_newObject.nameTag, _newObject.resource, _pos, _randRot, Vector3.one / voxelPerUnit);
        }
    }
}
