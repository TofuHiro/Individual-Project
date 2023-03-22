using UnityEngine;

public interface IHarvestableVoxel
{
    void Harvest(Vector3 _worldSpacePos, float _radius, HarvestTypes _toolType, int _toolTier);
}
