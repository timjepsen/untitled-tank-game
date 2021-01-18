using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_HealthUIPrefab;
    [SerializeField] private GameObject m_HealthUI;

    private Text m_HealthText;
    private HealthLossText m_HealthLossText;
    private Image m_CanMoveImage;
    [SerializeField] private Color m_TeamColor;
    [SerializeField] private Color m_ActiveColor;
    [SerializeField] private Color m_TargettedColor;
    
    public void SetUp(TeamManager teamManager, int tankNumber, float startHealth) {
        m_HealthUI = Instantiate(m_HealthUIPrefab) as GameObject;
        m_HealthUI.transform.SetParent(GameObject.Find("MainCanvas").transform);

        Vector2 anchorPosition;
        int x_position;
        int playerNumber = teamManager.GetPlayerNumber();
        switch (playerNumber) { //TODO: refactor
            case 0:
                anchorPosition = new Vector2(0,1);
                x_position = 150;
                break;
            case 1:
                anchorPosition = new Vector2(1,1);
                x_position = -150;
                break;
            default:
                Debug.Log(playerNumber.ToString() + "UI creation went wrong");
                anchorPosition = new Vector2(0,0);
                x_position = 0;
                break;
        }
        int y_position = - 50 - 100 * tankNumber;

        m_TeamColor = teamManager.GetTeamColour();
        
        RectTransform healthRect = m_HealthUI.GetComponent<RectTransform>();
        healthRect.anchorMin = anchorPosition;
        healthRect.anchorMax = anchorPosition;
        healthRect.anchoredPosition = new Vector2(x_position,y_position);

        m_HealthText = m_HealthUI.transform.Find("Text").GetComponent<Text>();
        UpdateHealth(startHealth);

        m_HealthLossText = m_HealthUI.transform.Find("HealthLossText").GetComponent<HealthLossText>();
        m_CanMoveImage = m_HealthUI.transform.Find("CanMoveIcon").GetComponent<Image>();

        if (playerNumber ==1) {
            m_CanMoveImage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, -38);
        }

        ShowActive(false);
    }

    public void UpdateHealth(float health) {//TODO: these two parameters have some redundancy. fix
        string sHealth = Mathf.Round((float) health).ToString();
        m_HealthText.text = "Health: " + sHealth;

    }
    public void ShowDamage(float damage) {
        m_HealthLossText.Show(damage);
    }

    public void ShowActive(bool Active) {//TODO: make this function and the next better
        if (Active) {
            m_HealthUI.GetComponent<Image>().color = m_ActiveColor;
        }
        else {
            m_HealthUI.GetComponent<Image>().color = m_TeamColor;
        }
    }
    public void ShowTargetted(bool Active) {
        if (Active) {
            m_HealthUI.GetComponent<Image>().color = m_TargettedColor;
        }
        else {
            m_HealthUI.GetComponent<Image>().color = m_TeamColor;
        }
    }
    public void SetCanMove(bool canMove) {
        m_CanMoveImage.enabled = canMove;
    }

}
