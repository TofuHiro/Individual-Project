public interface IHarvestable 
{
    HarvestTypes GetHarvestType();
    int GetMinTier();
    void TakeDamage(float _value, HarvestTypes _harvestType, int _tier);
    void Die();
}
