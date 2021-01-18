using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] public int m_PlayerNumber;
    private int m_StartingNumberTanks= 3;
    [SerializeField] private GameObject m_StandardTankPrefab;
    [SerializeField] private GameObject m_QuickTankPrefab;
    public GameManager m_GameManager;
    private CameraManager m_CameraManager;
    [SerializeField] private int m_RecentTank = -1;//The current/most recently active tank on this team
    public List<TankManager> m_Tanks;
    [SerializeField] private GameObject[] m_SpawnPoints;
    [SerializeField] private Material m_TankMaterial;
    [SerializeField] private Color m_TeamColor;
  
    public void SpawnTank(GameObject spawnPoint, GameObject tankToSpawn) { 
        GameObject newTank = Instantiate(tankToSpawn, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        m_Tanks.Insert(m_RecentTank, newTank.GetComponent<TankManager>());
        m_Tanks[m_RecentTank].SetUp(gameObject.GetComponent<TeamManager>(), m_RecentTank, m_TankMaterial);
        m_RecentTank++;
    }
    
    void Awake() {
        m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_CameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();

        m_RecentTank = 0;
        for (int i = 0; i < m_StartingNumberTanks -1; i++) {
            SpawnTank(m_SpawnPoints[i], m_StandardTankPrefab);
        }
        SpawnTank(m_SpawnPoints[m_StartingNumberTanks -1], m_QuickTankPrefab);
        m_RecentTank = m_Tanks.Count -1;
        Debug.Log("Team " + m_PlayerNumber.ToString() + " finished setup.");
    }

    private void IncrementRecentTank() {
        m_RecentTank++;
        if (m_RecentTank >= m_Tanks.Count) {
            m_RecentTank = 0;
        }
    }

    public void EndTurn() {
        m_Tanks[m_RecentTank].MakeActive(false);
    }

    public void StartNextTankTurn() {
        IncrementRecentTank();
        if (m_Tanks[m_RecentTank].IsAlive()) {
            m_CameraManager.SetMainCamera();
            m_Tanks[m_RecentTank].MakeActive(true);
        }
        else {
            StartNextTankTurn();
        }      
    }

    public bool CheckIfTeamDestroyed() {
        bool teamDestroyed = m_Tanks.TrueForAll(tank => !(tank.IsAlive() && tank.m_TankMovement.GetAbleToMove()));
        if (teamDestroyed) {
            Debug.Log("Team " + m_PlayerNumber.ToString() + " has been destroyed and loses");
            m_GameManager.GameEnd(1 - m_PlayerNumber);
        }
        return teamDestroyed;
    }

    public void SetTurretCamera(bool active) {
        m_Tanks[m_RecentTank].SetTurretCamera(active);
    }
    public void SetFollowerCamera(bool active) {
        m_Tanks[m_RecentTank].SetFollowerCamera(active);
    }//consider moving these two into just one function maybe
    public void SetTopDownCamera(bool active) {
        m_Tanks[m_RecentTank].SetTopDownCamera(active);
    }

    public int GetPlayerNumber() {
        return m_PlayerNumber;
    }
    public int GetNumberOfAliveTanks() {
        List<TankManager> AliveTanks = m_Tanks.FindAll(tank => tank.IsAlive());
        return AliveTanks.Count;
    }
    public Color GetTeamColour() {
        return m_TeamColor;
    }
    public Material GetTeamMaterial() {
        return m_TankMaterial;
    }
}
