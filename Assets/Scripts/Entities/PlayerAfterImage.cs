using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    private float m_ActiveTime = 0.2f;
    private float m_TimeActivated;
    private float t;
    private float m_Alpha;
    private float m_AlphaSet = 0.8f;
    private float m_AlphaMultiplier = 0.85f;

    private Transform m_Player;
    private SpriteRenderer m_Sprite;
    private SpriteRenderer m_PlayerSprite;
    private Color m_Color;

    public Color m_ImageColor;

    void OnEnable()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_PlayerSprite = m_Player.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();

        m_Alpha = m_AlphaSet;
        m_Sprite.sprite = m_PlayerSprite.sprite;
        transform.position = m_PlayerSprite.transform.position;
        m_TimeActivated = Time.time;
        t = 0.0f;
    }

    void Update()
    {
        m_Alpha *= m_AlphaMultiplier;
        m_Color.a = m_Alpha;
        if(m_Player.GetComponent<Player>().m_State == Player.EntityState.Super)
        {
            m_Sprite.color = new Color(m_Sprite.color.r, m_Sprite.color.g, m_Sprite.color.b, m_Alpha);
            m_ActiveTime = 0.4f;
        }
        else
        {
            m_Sprite.color = new Color(m_ImageColor.r, m_ImageColor.g, m_ImageColor.b, m_Alpha);
            m_ActiveTime = 0.2f;
        }

        if(Time.time >= (m_TimeActivated + m_ActiveTime))
        {
            AfterImagePool.instance.AddToPool(gameObject);
        }
    }
}
