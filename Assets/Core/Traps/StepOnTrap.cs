using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class StepOnTrap : MonoBehaviour
{
    [field:SerializeField]
    private Vector3 OffsetPosition;
    [field:SerializeField]
    private Vector3 HitboxSize;
    [field:SerializeField]
    private LayerMask layerMask;
    [field:SerializeField]
    private double damage;
    [field:SerializeField]
    private int CooldownMiliseconds = 4000;
    [field:SerializeField]
    private GameObject TrapObject;
    [field:SerializeField]
    private int TrapVisibleDurationMilisec = 1000;
    private bool Active = false;
    public void Trigger()
    {
        if (Active) return;
        Active = true;
        StatcHitboxCreator.TryHitWithBoxHitbox(transform.position+OffsetPosition,HitboxSize,layerMask,damage,false,transform.rotation);
        if (TrapObject != null)
        {
            StartCoroutine(TrapVisible());
        }
        Task.Run(CooldownTask);
    }
    private async Task CooldownTask()
    {
        await Task.Delay(CooldownMiliseconds);
        Active = false;
    }
    private IEnumerator TrapVisible()
    {
        TrapObject.SetActive(true);
        yield return new WaitForSeconds(TrapVisibleDurationMilisec / 1000f);
        TrapObject.SetActive(false);
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position+OffsetPosition,HitboxSize/2);
    }
#endif
}
