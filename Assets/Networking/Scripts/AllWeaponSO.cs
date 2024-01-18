using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Weapon Details", menuName = "Scriptable Objects/SiwDev/All Weapon Details")]
public class AllWeaponSO : ScriptableObject
{
    public List<WeaponDetailsSO> weapons;
}
