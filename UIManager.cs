using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private TextMeshProUGUI m_FuelText;
    private Slider m_FuelSlider;
    private GameObject m_GameEndPanel; 
    private TextMeshProUGUI winnerText;
    private TextMeshProUGUI m_CoverText;
    private Image m_FiringArcImage;
    private Image m_CriticalImage;
    private GameObject m_PlayerTurnPanel;
    private GameObject m_AbandonTankPanel;

    public void Awake () {
        m_FuelText = GameObject.Find("TankHPText").GetComponent<TextMeshProUGUI>();
        m_FuelSlider = GameObject.Find("FuelSlider").GetComponent<Slider>();
        m_GameEndPanel = GameObject.Find("GameEndCanvas").transform.Find("GameEndPanel").gameObject;
        winnerText = m_GameEndPanel.transform.Find("WinnerText").GetComponent<TextMeshProUGUI>();
        m_CoverText = GameObject.Find("CoverText").GetComponent<TextMeshProUGUI>();
        m_FiringArcImage = GameObject.Find("FiringArcImage").GetComponent<Image>();
        m_CriticalImage = GameObject.Find("CriticalHitImage").GetComponent<Image>();
        m_PlayerTurnPanel = GameObject.Find("PlayerTurnCanvas").transform.Find("PlayerTurnPanel").gameObject;
        m_AbandonTankPanel = GameObject.Find("AbandonTankCanvas").transform.Find("AbandonTankPanel").gameObject;        
    }

    public void SetFuelAmount(float normalizedFuelAmount) {
        m_FuelSlider.normalizedValue = (normalizedFuelAmount);
    }

    public void SetHealth(float currentHealth) {
        m_FuelText.text = "HP: " + currentHealth.ToString();
    }

    public void DisplayWinner(int winningPlayerNumber) {
        m_GameEndPanel.SetActive(true);
        winnerText.text = "Player " + (winningPlayerNumber + 1).ToString() + " wins";
    }
    public void SetCoverText(float coverDamageModifier) {
        if (coverDamageModifier < 0.01f) {
            m_CoverText.text = "No line of sight";
        }
        else {
        m_CoverText.text = "Cover modifier: " + coverDamageModifier.ToString("F1");
        }
    }
    public void SetFiringArcImage(bool isInFiringArc) {
        m_FiringArcImage.enabled = !isInFiringArc;
    }
    public void SetCriticalImage(bool isCritical) {
        m_CriticalImage.enabled = isCritical;
    }
    public void ShowPlayerTurn(int playerNumber) {
        m_PlayerTurnPanel.SetActive(true);
        TextMeshProUGUI playerTurnText = m_PlayerTurnPanel.transform.Find("PlayerTurnText").GetComponent<TextMeshProUGUI>();
        playerTurnText.text = "Player " + playerNumber.ToString() + "'s Turn";
    }
    public void HidePlayerTurn() {
        m_PlayerTurnPanel.SetActive(false);
    }
    public void SetAbandonTankPanel(bool visible) {
        m_AbandonTankPanel.SetActive(visible);
    }
}