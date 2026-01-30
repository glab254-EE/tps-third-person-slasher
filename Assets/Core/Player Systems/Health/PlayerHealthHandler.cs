using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHandler : MonoBehaviour, IDamagable
{
    [SerializeField] private Image HealthImage; //Edited

    [field:SerializeField]
    private double MaxHealth = 10;
    [field:SerializeField]
    private int GoddedTimerMilisec = 100;
    internal bool Godded {get;private set;}
    internal double Health {get;private set;}
    internal event Action<double> OnDamaged;
    void Awake()
    {
        Health = MaxHealth;

        HealthImage.fillAmount = (float)(Health / MaxHealth); //Edited
    }
    public bool TryDamage(double damage)
    {
        if (Godded || Health <= 0)
        {
            return false;
        }
        Health -= damage;

        HealthImage.fillAmount = (float)(Health / MaxHealth); //Edited

        Godded = true;
        Task.Run(GoddedTask);
        OnDamaged.Invoke(Health);
        #if UNITY_EDITOR
        Debug.Log(Health);
        #endif
        return true;
    }
    private async Task GoddedTask()
    {
        Godded = true;
        await Task.Delay(GoddedTimerMilisec);
        Godded = false;
    }
}
