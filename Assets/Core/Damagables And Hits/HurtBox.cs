using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [field:SerializeField]
    private DamagableContainer damagable;
    public bool OnHit(double damage)
    {
        return damagable.damagable.TryDamage(damage);
    }
}
[System.Serializable]
struct DamagableContainer
{
    [field:SerializeField]
    private UnityEngine.Object Object;
    public IDamagable damagable
    {
        get
        {
            return (IDamagable) Object;
        }
    }
}