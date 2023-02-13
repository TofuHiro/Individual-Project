public interface IDamagable
{
    float Health { get; set; }
    void TakeDamage(float _value);
    void Die();
}
