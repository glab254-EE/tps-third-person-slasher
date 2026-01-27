using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
   
    [field:SerializeField]
    private double MaxHealth = 10;
    internal double Health {get;private set;}
    internal event Action<double> OnDamaged;
    void Start()
    {
        Health = MaxHealth;
    }
    public bool TryDamage(double damage)
    {
        if (Health <= 0)
        {
            return false;
        }
        Health -= damage;
        OnDamaged.Invoke(Health);
        #if UNITY_EDITOR
        Debug.Log(Health);
        #endif
        return true;
    }
}
