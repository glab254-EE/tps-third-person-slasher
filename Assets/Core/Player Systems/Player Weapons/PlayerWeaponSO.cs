using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponSO", menuName = "Scriptable Objects/PlayerWeaponSO")]
public class PlayerWeaponSO : ScriptableObject
{
    [field:SerializeField]
    public GameObject model;
    [field:SerializeField]
    public RuntimeAnimatorController animator;
    [field:SerializeField]
    public float ComboDuration = 4f;
    [field:SerializeField]
    public List<AttackPattern> AttackCombos;
}
