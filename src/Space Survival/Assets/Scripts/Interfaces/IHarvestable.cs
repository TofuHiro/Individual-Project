public interface IHarvestable 
{
    void TakeDamage(float _value, HarvestTypes _harvestType, int _tier);

    void Die();
}
