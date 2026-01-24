using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class OnTriggerEnterTrigger : MonoBehaviour
{
    [field:SerializeField]
    private UnityEvent events;
    void OnTriggerEnter(Collider other)
    {
        events?.Invoke();
    }
}
