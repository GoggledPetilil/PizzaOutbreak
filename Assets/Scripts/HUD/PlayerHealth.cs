using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Components")]
    [SerializeField] private Image m_HealthFill;
    [SerializeField] private Image m_HurtFill;
    [SerializeField] private Animator m_Anim;

    [Header("Danger Components")]
    [SerializeField] private Image m_FlashImage;
    [SerializeField] private Color m_DangerColor;
    [SerializeField] private float m_FlashSpeed;
    [SerializeField] private AudioSource m_AlarmAudio;

    private Color m_DefaultColor;
    private Color m_StartColor;
    private Color m_EndColor;
    private float m_FlashTimer;

    [Header("Player Info")]
    public int maxHealth;
    public int currentHealth;
    public Player m_Player;

    private float waitTime = 1f;
    private float hurtDuration = 0.25f;
    private float criticalThreshold = 0.20f;

    void Awake()
    {
        m_Anim = gameObject.GetComponent<Animator>();
        m_Anim.SetFloat("Health", 1f);

        m_DefaultColor = m_FlashImage.color;
        m_StartColor = m_DefaultColor;
        m_EndColor = m_DangerColor;
    }

    void Update()
    {
        if(m_HealthFill.fillAmount <= criticalThreshold)
        {
            // The player is in critical condition.
            float r = Mathf.Lerp(m_StartColor.r, m_EndColor.r, m_FlashTimer);
            float g = Mathf.Lerp(m_StartColor.g, m_EndColor.g, m_FlashTimer);
            float b = Mathf.Lerp(m_StartColor.b, m_EndColor.b, m_FlashTimer);
            m_FlashImage.color = new Color(r, g, b);

            m_FlashTimer += m_FlashSpeed * Time.deltaTime;

            if(m_FlashTimer > 1.0f)
            {
                Color temp = m_StartColor;
                m_StartColor = m_EndColor;
                m_EndColor = temp;
                m_FlashTimer = 0.0f;
            }

            if(m_AlarmAudio.isPlaying == false)
            {
                m_AlarmAudio.Play();
            }

        }
        else if(m_HealthFill.fillAmount > criticalThreshold && (m_FlashImage.color != m_DefaultColor || m_AlarmAudio.isPlaying))
        {
            m_FlashImage.color = m_DefaultColor;
            m_StartColor = m_DefaultColor;
            m_EndColor = m_DangerColor;
            m_FlashTimer = 0.0f;

            m_AlarmAudio.Stop();
        }
    }

    public void SetHealth()
    {
        if(currentHealth > m_Player.m_HP)
        {
            ReduceHealth();
        }
        else
        {
            HealHealth();
        }

        maxHealth = m_Player.m_MaxHP;
        currentHealth = m_Player.m_HP;
    }

    void HealHealth()
    {
        StopAllCoroutines();
        currentHealth = Mathf.Clamp(m_Player.m_HP, 0, m_Player.m_MaxHP);
        if((float)currentHealth / (float)maxHealth > m_HurtFill.fillAmount)
        {
            m_HealthFill.fillAmount = (float)this.currentHealth / (float)this.maxHealth;
            m_HurtFill.fillAmount = m_HealthFill.fillAmount;
        }
        else
        {
            StartCoroutine(HealthReduce(maxHealth, currentHealth));
        }

        m_Anim.SetFloat("Health", m_HealthFill.fillAmount);
    }

    void ReduceHealth()
    {
        StopAllCoroutines();
        currentHealth = Mathf.Clamp(m_Player.m_HP, 0, m_Player.m_MaxHP);
        m_Anim.SetTrigger("Damaged");
        StartCoroutine(HealthReduce(maxHealth, currentHealth));

        m_Anim.SetFloat("Health", m_HealthFill.fillAmount);
    }

    IEnumerator HealthReduce(int maxHealth, int currentHealth)
    {
        m_HealthFill.fillAmount = (float)this.currentHealth / (float)this.maxHealth;

        yield return new WaitForSeconds(waitTime);

        float startFill = m_HurtFill.fillAmount;
        float endFill = m_HealthFill.fillAmount;
        float t = 0.0f;
        while(t <= 1)
        {
            t += Time.deltaTime / hurtDuration;
            m_HurtFill.fillAmount = Mathf.Lerp(startFill, endFill, t);
            yield return null;
        }
    }
}
