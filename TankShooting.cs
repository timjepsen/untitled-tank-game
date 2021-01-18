using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [SerializeField] private float m_Firepower;//TODO: be consistent with variable names
    [SerializeField] private TankManager m_CurrentTarget;
    [SerializeField] private int m_current_target_index;
    private TankManager m_TankManager;
    [SerializeField] private TeamManager m_EnemyTeam;
    [SerializeField] private int m_ShotsPerTurn;
    [SerializeField] private int m_ShotsRemaining;
    [SerializeField] private float m_FiringArcAngle;
    [SerializeField] private GameObject m_Explosion;
    private Transform m_Turret;

    void Awake() {
        m_TankManager = GetComponent<TankManager>();
    }

    void Start() {
        m_Turret = gameObject.transform.Find("Turret");

        GameObject[] TeamManagers = GameObject.FindGameObjectsWithTag("TeamManager");
        for (int i = 0; i < TeamManagers.Length; i++) {
                TeamManager possibleEnemyTeam = TeamManagers[i].GetComponent<TeamManager>();
                if (possibleEnemyTeam.m_PlayerNumber != m_TankManager.GetPlayerNumber()) {
                    m_EnemyTeam = possibleEnemyTeam;
                    break;
                }
            }
        m_current_target_index = 0;
        m_CurrentTarget = m_EnemyTeam.m_Tanks[0];
    }

    void Update() {
        if (m_TankManager.GetActive()) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot(m_EnemyTeam.m_Tanks[m_current_target_index]);
            }
            if (Input.GetButtonDown("Fire2")) {
                CycleToNextAliveTarget();
            }
            if (m_CurrentTarget != null) {
                AimTurret(m_CurrentTarget);
                float cover = CheckCoverMultiplier(CheckVisibilityToTarget(m_CurrentTarget));
                m_TankManager.m_UIManager.SetCoverText(cover);
                m_TankManager.m_UIManager.SetCriticalImage(CheckCritical(m_CurrentTarget));
                m_TankManager.m_UIManager.SetFiringArcImage(CheckFiringArc(m_CurrentTarget));
            }
        }
    }

    public void MakeActive(bool active) {
        m_CurrentTarget.SetTargetted(active);
        if (active) {
            m_ShotsRemaining = m_ShotsPerTurn;
        }
    }

    public void Shoot(TankManager target) {
        float damageMultiplier = 1.0f;
        damageMultiplier = damageMultiplier * CheckCoverMultiplier(CheckVisibilityToTarget(target));
        bool isCritical = CheckCritical(target);
        if (isCritical) {
            damageMultiplier = damageMultiplier * 1.5f;
        }
        Debug.Log("Damage multiplier = " + damageMultiplier.ToString());
        if (damageMultiplier > 0.00001f && CheckFiringArc(target) && GetShotRemaining() && !m_TankManager.GetInWater()) {
            m_ShotsRemaining--;
            GetComponent<AudioSource>().Play(0);
            Instantiate(m_Explosion, target.transform.position, Quaternion.identity);
            target.TakeDamage(m_Firepower * damageMultiplier, isCritical);
            m_EnemyTeam.CheckIfTeamDestroyed();
            StartCoroutine(WaitAndEndTurn());
        }
    }

    IEnumerator WaitAndEndTurn() {
        yield return new WaitForSeconds(1.0f);
        if (m_TankManager.GetActive()) {
            m_TankManager.m_TeamManager.m_GameManager.EndTurn();
        }
    }

    bool CheckCritical(TankManager target) {
        Vector3 targetForward = target.transform.forward;
        Vector3 shotDirection = target.GetFrontPosition() - GetTurretEndPosition();

        float relativeAngle = Vector3.Angle(shotDirection, targetForward);
        if (relativeAngle < 90f) {
            return true;
        }
        else return false;
    }
    float CheckVisibilityToTarget(TankManager target) {
        Transform obj = target.gameObject.transform.Find("CoverCheckers");
        List<Transform> coverCheckers = new List<Transform>();
        int total_colliders = 0;
        int visibleColliders = 0;
        foreach (Transform child in obj) {
            coverCheckers.Add(child);
            RaycastHit hit;

            if (!Physics.Linecast(GetTurretEndPosition(), child.position, out hit) || hit.transform == target.transform || hit.transform == transform) {
                visibleColliders++;
            }
            total_colliders++;
        }
        float fraction_visible = (float)visibleColliders / (float)total_colliders;
        return fraction_visible;
    }

    private float CheckCoverMultiplier(float enemyTankVisibility) {//TODO: make this function better
        if (enemyTankVisibility < 0.0001f) {
            return 0.0f;
        }
        else if (enemyTankVisibility < 0.5) {
            return 0.3f;
        }
        else if (enemyTankVisibility < 1) {
            return 0.6f;
        }
        else {
            return 1.0f;
        }
    }
    bool CheckFiringArc(TankManager target) {//this method is very similar to CheckCritical(). could be worth tidying them both into one method?
        Vector3 forward = transform.forward;
        Vector3 shotDirection = target.transform.position - transform.position;
        float firingAngle = Vector3.Angle(shotDirection, forward);

        return (firingAngle < m_FiringArcAngle && firingAngle > -1.0f * m_FiringArcAngle);
    }
    void AimTurret(TankManager target) {
        Vector3 m_LookAt = new Vector3(target.transform.position.x, 
            gameObject.transform.position.y, 
            target.transform.position.z);
        m_Turret.LookAt(m_LookAt, Vector3.up);
        //m_turret.LookAt(target);
    }
    
    void CycleToNextAliveTarget() {
        //If changing UI colour (in SetTargetted) is slow, then I should rewrite this to only
        //change the colours of the original and final tanks chosen in the function
        m_CurrentTarget.SetTargetted(false);
        m_current_target_index++;
        if (m_current_target_index >= m_EnemyTeam.m_Tanks.Count) {
            m_current_target_index = 0;
        }

        m_CurrentTarget = m_EnemyTeam.m_Tanks[m_current_target_index];
        m_CurrentTarget.SetTargetted(true);

        if (!m_CurrentTarget.IsAlive()) {
            CycleToNextAliveTarget();
        }
    }
    
    Vector3 GetTurretEndPosition() {
        return (m_Turret.Find("TurretCamera").position);
    }
    public void SetFirepower(float newFirepower) {
        m_Firepower = newFirepower;
    }
    public float GetFirepower() {
        return m_Firepower;
    }
    public bool GetShotRemaining() {
        return (m_ShotsRemaining > 0);
    }
}
