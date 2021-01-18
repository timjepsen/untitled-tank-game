using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedTurret : MonoBehaviour
{
    [SerializeField] private GameObject[] m_ObjectsToColour;
    private Rigidbody rb;
    public void SetUp(Vector3 localUp, Material tankMaterial)
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = localUp * 10.0f + Vector3.ProjectOnPlane(Random.insideUnitSphere, localUp);
        rb.angularVelocity = Random.insideUnitSphere;

        foreach (GameObject obj in m_ObjectsToColour) {//TODO: refactor. same as changing material in TankManager
            obj.GetComponent<Renderer>().material = tankMaterial;
        }
    }
}
