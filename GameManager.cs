using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int m_NumberOfPlayers = 2;
    [SerializeField] private float m_MapXMin, m_MapZMin, m_MapXMAx, m_MapZMax;
    [SerializeField] private PickupsManager m_PickupsManager;
    private CameraManager m_CameraManager;
    public TeamManager[] m_TeamManagers;
    private UIManager m_UIManager;
    private int m_ActivePlayer = 0;
    private bool m_GameActive = true;
    private bool m_IsInBetweenTurns;
    
    void Start() {
        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_UIManager.ShowPlayerTurn(m_ActivePlayer + 1);
        m_CameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        m_IsInBetweenTurns = true;
    }
    void Update() {
        if (Input.GetButtonDown("NextTurn")) {
            if (m_GameActive) {
                if (!m_IsInBetweenTurns) {//arguably this could be made neater
                    EndTurn();
                }
                else {
                    StartNextTurn();
                }
            }
            else {
                SceneManager.LoadScene(0);
            }
        }
    }
    public void EndTurn() {
        m_TeamManagers[m_ActivePlayer].EndTurn();   
        m_CameraManager.SetMainCamera();         
            m_ActivePlayer++;
            if (m_ActivePlayer >= m_NumberOfPlayers) {
                m_ActivePlayer = 0;
            }
            m_UIManager.ShowPlayerTurn(m_ActivePlayer + 1);
            m_IsInBetweenTurns = true;
    }
    public void StartNextTurn() {
            m_UIManager.HidePlayerTurn();
            m_TeamManagers[m_ActivePlayer].StartNextTankTurn();
            m_PickupsManager.CountDown();
            m_IsInBetweenTurns = false;
    }
    
    public void GameEnd(int winningPlayerNumber) {
        m_TeamManagers[m_ActivePlayer].EndTurn();
        m_CameraManager.SetMainCamera();
        m_UIManager.DisplayWinner(winningPlayerNumber);
        m_GameActive = false;
    }
    public (float, float, float, float)  GetMapDimensions() {
        return (m_MapXMin, m_MapXMAx, m_MapZMin, m_MapZMax);
    }
    public int GetTotalAliveTanks() {
        return m_TeamManagers[0].GetNumberOfAliveTanks() + m_TeamManagers[1].GetNumberOfAliveTanks();//TODO: generalise for more teams
    }
    public int GetActivePlayer() {
        return m_ActivePlayer;
    }
    public bool GetIsBetweenTurns() {
        return m_IsInBetweenTurns;
    }
}