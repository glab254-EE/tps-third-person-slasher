using UnityEngine;
using System.Collections.Generic;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private List<EnemyAI> enemiesInRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var enemy in enemiesInRoom)
            {
                enemy.Activate(other.transform);

                /*if (!enemy.gameObject.activeInHierarchy)      //Edited
                {                                               //Edited
                    enemy.gameObject.SetActive(true);           //Edited
                }*/                                             //Edited
            }
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        foreach (var enemy in enemiesInRoom)
        {
            enemy.DeActivate(other.transform);
        }
    }*/
}