using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthLossText : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_ScaleRate;
    [SerializeField] private float m_SecondsBeforeDestroying;
    private Vector2 m_StartingPosition;
    private Vector3 m_StartingScale;
    private RectTransform m_RectTransform;
    private Text m_HealthLossText;
    void Start ()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_HealthLossText = GetComponent<Text>();
        m_StartingPosition = m_RectTransform.anchoredPosition;
        m_StartingScale = m_RectTransform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
    Vector2 position = m_RectTransform.anchoredPosition;
    position.y += m_Speed * Time.deltaTime;
    m_RectTransform.anchoredPosition = position;

    Vector3 scale = m_RectTransform.localScale;
    scale -= Vector3.one * m_ScaleRate * Time.deltaTime;
    m_RectTransform.localScale = scale;
    }
    public void Show(float damage) {
        m_RectTransform.anchoredPosition = m_StartingPosition;
        m_RectTransform.localScale = m_StartingScale;
        m_HealthLossText.enabled = true;
        m_HealthLossText.text = Mathf.Round(damage * -1.0f).ToString();
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy() {
        yield return new WaitForSeconds(m_SecondsBeforeDestroying);
        m_HealthLossText.enabled = false;
    }
}
