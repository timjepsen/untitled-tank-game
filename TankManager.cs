using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    // Start is called before the first frame update

    public TeamManager m_TeamManager;
    [SerializeField] public TankMovement m_TankMovement;
    public TankUIManager m_TankUIManager; ///TODO: be consistent with which components are private/public

    public TankShooting m_TankShooting;
    public UIManager m_UIManager;
    private CameraManager m_CameraManager;
    
    private bool m_InWater;
    [SerializeField] private float m_StartHealth;
    [SerializeField] private float m_CurrentHealth;
    [SerializeField] private bool m_Active = true;
    [SerializeField] private GameObject[] m_ObjectsToColour;
    [SerializeField] private GameObject m_DeadTurretPrefab;
    private bool m_ReadyToAbandon;

    public void SetUp(TeamManager teamManager, int tankNumber, Material tankMaterial) {
        m_TeamManager = teamManager;
        m_TankMovement = gameObject.GetComponent<TankMovement>();
        m_TankMovement.MakeActive(false);

        m_TankShooting = gameObject.GetComponent<TankShooting>();

        m_TankUIManager = gameObject.GetComponent<TankUIManager>();

        m_CameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_TankUIManager.SetUp(m_TeamManager, tankNumber, m_StartHealth);

        m_ReadyToAbandon = false;
        m_InWater = false;

        foreach (GameObject obj in m_ObjectsToColour) {//this needs a refactor: m_ObjectsTocolour has all the individual objects, could just be the parents
            obj.GetComponent<Renderer>().material = tankMaterial;
        }

        m_CurrentHealth = m_StartHealth;

        m_Active = false;
    }

    void Update() {
        if (m_Active) {
            if (Input.GetButtonDown("Abandon")  && m_TankShooting.GetShotRemaining()) {
                if (m_ReadyToAbandon) {
                    StartCoroutine(Abandon());
                }
                else {
                    SetReadyToAbandon(true);
                }
            }
            if (Input.GetButtonDown("Fire2") && m_ReadyToAbandon) {
                SetReadyToAbandon(false);
            }
        }
    }

    public void TakeDamage(float damage, bool isCritical) {
        SetHealth(m_CurrentHealth - damage);
        m_TankUIManager.ShowDamage(damage);
        
        if (isCritical) {
            m_TankMovement.SetAbleToMove(false);
        }
    }

    //Destroys the tank and starts the next tank's turn
    IEnumerator Abandon() { //So far it seems this has to be a coroutine as it triggers on the frame the Abandin button is pressed, and so if it starts the next tank's turn immediately then that tank abdndons immediately (as we're still on the frame where the button was pressed)
        yield return null;
        SetHealth(0);
        m_UIManager.SetAbandonTankPanel(false);
        MakeActive(false);
        if (!m_TeamManager.CheckIfTeamDestroyed()) {
            m_TeamManager.StartNextTankTurn();
        }
    }
    public void SetReadyToAbandon(bool value) {
        m_UIManager.SetAbandonTankPanel(value);
        m_ReadyToAbandon = value;
    }

    private void Destroy() {
        m_CurrentHealth = 0;
        GameObject turret = gameObject.transform.Find("Turret").gameObject;
        GameObject destroyedTurret = Instantiate(m_DeadTurretPrefab, turret.transform.position, turret.transform.rotation);
        destroyedTurret.GetComponent<DestroyedTurret>().SetUp(transform.up, m_TeamManager.GetTeamMaterial());
        turret.SetActive(false);
    }

    //Setters and getters
    public void MakeActive(bool active) {
        if (IsAlive() || !active) {
            m_Active = active;
            if (!active) {
                m_CameraManager.TurnOffAllCurrentCameras();
                SetReadyToAbandon(false);
            }
            m_TankUIManager.ShowActive(active);
            m_UIManager.SetHealth(m_CurrentHealth);
            m_TankMovement.MakeActive(active);
            m_TankShooting.MakeActive(active);
        }
    }
    public bool IsAlive() {
        return (GetHealth() > 0.0f);
    }
    public bool GetActive() {
        return m_Active;
    }
    public int GetPlayerNumber() {
        return m_TeamManager.GetPlayerNumber();
    }
    public Vector3 GetFrontPosition() {
        return (transform.position + (transform.forward *2.5f));
    }
    public void SetTurretCamera(bool active) {
        Camera follower = transform.Find("Turret").transform.Find("TurretCamera").GetComponent<Camera>();
        AudioListener listener = transform.Find("Turret").transform.Find("TurretCamera").GetComponent<AudioListener>();
        listener.enabled = active;
        follower.enabled = active;
    }
    public void SetFollowerCamera(bool active) {
        Camera turret = transform.Find("FollowerCameraRig").transform.Find("FollowerCamera").GetComponent<Camera>();
        AudioListener listener = transform.Find("FollowerCameraRig").transform.Find("FollowerCamera").GetComponent<AudioListener>();
        listener.enabled = active;
        turret.enabled = active;
    }//consider moving these two into just one function maybe
    public void SetTopDownCamera(bool active) {
        Camera topdown = transform.Find("TopDownCameraRig").transform.Find("TopDownCamera").GetComponent<Camera>();
        AudioListener listener = transform.Find("TopDownCameraRig").transform.Find("TopDownCamera").GetComponent<AudioListener>();
        listener.enabled = active;
        topdown.enabled = active;
    }
    public void SetTargetted(bool targetted) {
        m_TankUIManager.ShowTargetted(targetted);
    }
    public void SetHealth(float NewHealth) {
        float oldHealth = m_CurrentHealth;
        m_CurrentHealth = NewHealth;
        if (NewHealth <= 0 && oldHealth > 0) {
             Destroy();   
        }
        m_TankUIManager.UpdateHealth(m_CurrentHealth);
    }
    public void SetInWater(bool inWater) {
        m_InWater = inWater;
    }
    public bool GetInWater() {
        return m_InWater;
    }
    public float GetHealth() {
        return m_CurrentHealth;
    }
}
