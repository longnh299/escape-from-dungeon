using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector });
    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection; // direcction of aim
    public float aimAngle; // angle between mouse cursor and pivot point of player
    public float weaponAimAngle; //angle between mouse cursor and pivot point of weapon on player prefab
    public Vector3 weaponAimDirectionVector; // use to caculate weaponAimAngle
}
