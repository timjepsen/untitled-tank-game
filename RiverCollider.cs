using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject rootObject = other.transform.root.gameObject;
        if (rootObject.tag == "PlayerVehicle") {
            rootObject.GetComponent<TankManager>().SetInWater(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject rootObject = other.transform.root.gameObject;
        if (rootObject.gameObject.tag == "PlayerVehicle") {
            rootObject.GetComponent<TankManager>().SetInWater(false);
        }
    }//should refactor this eventually
}
