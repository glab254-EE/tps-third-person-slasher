using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHealthHandler : MonoBehaviour, IDamagable
{
    [field:SerializeField]
    private double MaxHealth = 10;
    [field:SerializeField]
    private int GoddedTimerMilisec = 100;
    internal bool Godded {get;private set;}
    internal double Health {get;private set;}
    internal event Action<double> OnDamaged;
    void Start()
    {
        Health = MaxHealth;
    }
    public bool TryDamage(double damage)
    {
        if (Godded || Health <= 0)
        {
            return false;
        }
        Health -= damage;
        Godded = true;
        Task.Run(GoddedTask);
        OnDamaged.Invoke(Health);
        return true;
    }
    private async Task GoddedTask()
    {
        Godded = true;
        await Task.Delay(GoddedTimerMilisec);
        Godded = false;
    }
}
