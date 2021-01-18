using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        GameObject collider_root = collider.transform.root.gameObject;
        if (collider_root.tag == "PlayerVehicle" && collider_root.GetComponent<TankManager>().IsAlive()) {
            PickedUp(collider_root.GetComponent<TankManager>());
            Destroy(gameObject);
        }
    }

    public abstract void PickedUp(TankManager tankManager);
    void FixedUpdate() {
        transform.Rotate(Vector3.up);
    }
}
