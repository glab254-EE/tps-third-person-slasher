using Unity.Mathematics;
using UnityEngine;

public class TargetDamagable : MonoBehaviour, IDamagable
{
    // THE SCRIPT MAY BE MOVED, OR DELETED.
    // USED FOR TESTING PLAYER COMBAT.
    [field:SerializeField]
    private double health;
    public bool TryDamage(double damage)
    {
        health = math.clamp(health-damage,0,health);
        Debug.Log(health);
        return health > 0;
    }
}

