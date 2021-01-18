using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthpack : AbstractPickup
{
    [SerializeField] private float m_Health;
    public override void PickedUp(TankManager tankManager) {
        tankManager.SetHealth(tankManager.GetHealth() + m_Health);
    }
}
