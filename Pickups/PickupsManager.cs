using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupsManager : MonoBehaviour
{
    [SerializeField] private int TurnsPerPickup;
    private int TurnsUntilNextPickup;
    [SerializeField] private float m_MapXMin, m_MapZMin, m_MapXMax, m_MapZMax;
    [SerializeField] private float m_height;
    [SerializeField] private float m_MaxTankDistance;
    [SerializeField] private float m_MaxEnvironmentDistance; //this isn't actually the distance since i'm using an OverlapBox with this number. TODO: fix
    [SerializeField] private TeamManager[] TeamManagers;
    [SerializeField] private GameObject m_HealthPack;
    [SerializeField] private GameObject m_WeaponUpgradePack;
    [SerializeField] private int m_StartingPickupsNumber;
    [SerializeField] private Vector3 m_Center;
    [SerializeField] private float m_MaximumRadius;
    private GameManager m_GameManager;
    
    void Start() {
        m_GameManager = gameObject.GetComponent<GameManager>();

        TurnsUntilNextPickup = TurnsPerPickup;

         for (int i = 0; i < m_StartingPickupsNumber; i++) {
            CreatePickup();
        }
    }
    public void CountDown() {//rename
        TurnsUntilNextPickup -= 1;
        if (TurnsUntilNextPickup == 0) {
            CreatePickup();
            TurnsUntilNextPickup = TurnsPerPickup;
        }
        Debug.Log(TurnsUntilNextPickup.ToString());
    }
    public void CreatePickup() {
        Vector3 positionAttempt = GenerateRandomPosition();
        if (CheckIsSufficientlyFarFromPlayerVehicle(positionAttempt) && CheckIfNotTouchingObjects(positionAttempt)) {
                Instantiate(ChoosePickupToCreate(), positionAttempt, Quaternion.identity);
        } else { //try again
                CreatePickup();
        }
    }
        
    private Vector3 GenerateRandomPosition() {//Generates a random vector3 within m_MaximumRadius of m_Centre. Points are more frequently generated near the centre, as the probability
    //distribution of what radius points are at is constant and closer radii are more dense due to being small //TODO: rephrase maybe.
        float angle = Random.Range(0, 2* Mathf.PI);
        float length = Random.Range(0, m_MaximumRadius);

        Vector3 relativePosition = new Vector3(length * Mathf.Cos(angle), m_height, length * Mathf.Sin(angle));
        return m_Center + relativePosition;

    }

    private bool CheckIsSufficientlyFarFromPlayerVehicle(Vector3 position) {//This function and that below are v similar, so worth writing into just one if i expand this
        Collider[] ObjectsNearPosition = Physics.OverlapSphere(position,  m_MaxTankDistance);

        foreach(Collider ObjectNearPosition in ObjectsNearPosition) {
            if (ObjectNearPosition.transform.root.tag == "PlayerVehicle") {
                return false;
            }
        }
        return true;
    }

    private bool CheckIfNotTouchingObjects(Vector3 position) {
        Collider[] ObjectsNearPosition = Physics.OverlapBox(position,  new Vector3(m_MaxEnvironmentDistance,1.0f,m_MaxEnvironmentDistance));
        return (ObjectsNearPosition == null || ObjectsNearPosition.Length == 0);
    }
    private GameObject ChoosePickupToCreate() {
        if (m_GameManager.GetTotalAliveTanks() > 3) {
            return m_HealthPack;
        }
        else {
            return m_WeaponUpgradePack;
        }
    }
}