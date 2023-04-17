using UnityEngine;

public interface IHarvestableVoxel
{
    HarvestTypes GetHarvestType();
    int GetMinTier();
    void Harvest(Vector3 _worldSpacePos, float _radius, HarvestTypes _toolType, int _toolTier);
}
