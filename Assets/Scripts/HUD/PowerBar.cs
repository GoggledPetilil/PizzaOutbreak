using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public enum State
    {
        Charging,
        Ready,
        Draining
    }

    [Header("Overview")]
    public State m_State;
    public Player m_Player;

    [Header("Meter Parameters")]
    public float m_ChargeRate = 0.01f; // How fast the power meter charges.
    public float m_DrainRate = 0.1f; // How fast the meter depletes.
    public bool m_Charged;

    [Header("Components")]
    [SerializeField] private Image m_Fill;
    [SerializeField] private GameObject m_MaxIndicator;

    [Header("Danger Parameters")]
    [SerializeField] private Image m_FlashImage;
    [SerializeField] private Color m_FlashColor;
    [SerializeField] private float m_FlashSpeed;
    private Color m_DefaultColor;
    private Color m_StartColor;
    private Color m_EndColor;
    private float m_FlashTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_DefaultColor = m_FlashImage.color;
        m_StartColor = m_DefaultColor;
        m_EndColor = m_FlashColor;

        m_Fill.fillAmount = 0.0f;
        m_MaxIndicator.SetActive(false);
        m_State = State.Charging;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_State)
        {
            case State.Charging:
              m_Fill.fillAmount += m_ChargeRate * Time.deltaTime;
              if(m_Fill.fillAmount >= 1.0f)
              {
                  m_MaxIndicator.SetActive(true);
                  m_Charged = true;
                  m_State = State.Ready;
              }
              break;
            case State.Ready:
              // Stuff
              break;
            case State.Draining:
              m_Fill.fillAmount -= m_DrainRate * Time.deltaTime;

              if(m_Fill.fillAmount <= 0.25f)
              {
                  m_FlashTimer += m_FlashSpeed * Time.deltaTime;

                  float r = Mathf.Lerp(m_StartColor.r, m_EndColor.r, m_FlashTimer);
                  float g = Mathf.Lerp(m_StartColor.g, m_EndColor.g, m_FlashTimer);
                  float b = Mathf.Lerp(m_StartColor.b, m_EndColor.b, m_FlashTimer);
                  m_FlashImage.color = new Color(r, g, b);

                  if(m_FlashTimer > 1.0f)
                  {
                      Color temp = m_StartColor;
                      m_StartColor = m_EndColor;
                      m_EndColor = temp;
                      m_FlashTimer = 0.0f;
                  }

              }

              if(m_Fill.fillAmount <= 0.0f)
              {
                  DeactivatePower();
              }
              break;
        }
    }

    public void IncreasePower(float addition)
    {
        Mathf.Clamp(addition, 0.0f, 1.0f);
        m_Fill.fillAmount += addition;
    }

    public void ActivatePower()
    {
        m_MaxIndicator.SetActive(false);
        m_Charged = false;
        m_Player.SuperDashActivate();
        m_State = State.Draining;
    }

    void DeactivatePower()
    {
        m_FlashImage.color = m_DefaultColor;
        m_StartColor = m_DefaultColor;
        m_EndColor = m_FlashColor;
        m_FlashTimer = 0.0f;

        m_State = State.Charging;
        m_Player.DePower();
    }

    public void Revert()
    {
        StartCoroutine("DrainPower");
    }

    IEnumerator DrainPower()
    {
      float startFill = m_Fill.fillAmount;
      float endFill = 0.0f;
      float t = 0.0f;
      float drainDuration = 0.2f;
      while(t <= 1)
      {
          t += Time.deltaTime / drainDuration;
          m_Fill.fillAmount = Mathf.Lerp(startFill, endFill, t);
          yield return null;
      }
    }
}
