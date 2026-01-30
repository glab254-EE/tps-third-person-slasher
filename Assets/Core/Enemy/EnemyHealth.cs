using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    [field:SerializeField] private Image HealthImage; //Edited

    [field:SerializeField]
    private double MaxHealth;
    internal double Health {get;private set;}
    internal event Action<double> OnDamaged;
    void Start()
    {
        Health = MaxHealth;

        HealthImage.fillAmount = (float)(Health / MaxHealth); //Edited
    }
    public bool TryDamage(double damage)
    {
        if (Health <= 0)
        {
            return false;
        }
        Health -= damage;

        HealthImage.fillAmount = (float)(Health / MaxHealth); //Edited

        OnDamaged?.Invoke(Health);
        #if UNITY_EDITOR
        Debug.Log(Health);
        #endif
        return true;
    }
}
