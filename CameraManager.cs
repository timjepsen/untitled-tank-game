using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this page has a couple of magic numbers in and could benefit with a cheeky refactor, but i don't think i'll be changing this code much so for now i doubt there's much actual benefit to doing so
public class CameraManager : MonoBehaviour
{
    private GameManager m_GameManager;
    private TeamManager[] m_TeamManagers;
    private Camera m_MainCamera;
    private int m_CurrentCameraIndex = 0;//in particular it's m_CurrentCameraIndex that in principle could do with a refactor
    
    void Start() {
        m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_TeamManagers = m_GameManager.m_TeamManagers;
        m_MainCamera = Camera.main;
    }
    void Update() {
        if (!m_GameManager.GetIsBetweenTurns()) {
            if (Input.GetButtonDown("View")) {
                m_CurrentCameraIndex++;
                if (m_CurrentCameraIndex >= 4) {
                    m_CurrentCameraIndex = 0;
                }
                switch (m_CurrentCameraIndex) {
                    case 0:
                        SetMainCamera();
                        break;
                    case 1:
                        SetTopDownCamera();
                        break;
                    case 2:
                        SetFollowerCamera();
                        
                        break;
                    case 3:
                        SetTurretCamera();
                        break;
                }
            }   //TODO: eventually refactor this into using an inputmanager to directly handle inputs
            if  (Input.GetAxis ("DPadVertical") < -0.1f || Input.GetKeyDown(KeyCode.Alpha1)) {
                SetMainCamera();
            }
            else if (Input.GetAxis("DPadHorizontal") < -0.1f || Input.GetKeyDown(KeyCode.Alpha2)) {
                SetTopDownCamera();
            }
            else if (Input.GetAxis ("DPadHorizontal") > 0.1f || Input.GetKeyDown(KeyCode.Alpha3)) {
                SetFollowerCamera();
            }
            else if  (Input.GetAxis ("DPadVertical") > 0.1f || Input.GetKeyDown(KeyCode.Alpha4)) {
                SetTurretCamera();
            }
        }
    }
    public void SetMainCamera() {
        TurnOffAllCurrentCameras();
        m_MainCamera.enabled = true;
        m_MainCamera.gameObject.GetComponent<AudioListener>().enabled = true;
        m_CurrentCameraIndex = 0;
    }
    public void SetTopDownCamera() {
        TurnOffAllCurrentCameras();
        int activePlayer = m_GameManager.GetActivePlayer();
        m_TeamManagers[activePlayer].SetTopDownCamera(true);
        m_CurrentCameraIndex = 1;
    }
    public void SetFollowerCamera() {
        TurnOffAllCurrentCameras();
        int activePlayer = m_GameManager.GetActivePlayer();
        m_TeamManagers[activePlayer].SetFollowerCamera(true);
        m_CurrentCameraIndex = 2;
    }
    public void SetTurretCamera() {
        TurnOffAllCurrentCameras();
        int activePlayer = m_GameManager.GetActivePlayer();
        m_TeamManagers[activePlayer].SetTurretCamera(true);
        m_CurrentCameraIndex = 3;
    }

    public void TurnOffAllCurrentCameras() {
        int activePlayer = m_GameManager.GetActivePlayer();
        m_TeamManagers[activePlayer].SetTurretCamera(false);
        m_TeamManagers[activePlayer].SetFollowerCamera(false);
        m_TeamManagers[activePlayer].SetTopDownCamera(false);
        m_MainCamera.enabled = false;
        m_MainCamera.gameObject.GetComponent<AudioListener>().enabled = false;
    }
}
