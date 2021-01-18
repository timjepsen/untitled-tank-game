using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnhancement : AbstractPickup
{
    [SerializeField] private float m_FirepowerIncrease;
    public override void PickedUp(TankManager tankManager) {
        tankManager.m_TankShooting.SetFirepower(tankManager.m_TankShooting.GetFirepower() + m_FirepowerIncrease);
    }
}
