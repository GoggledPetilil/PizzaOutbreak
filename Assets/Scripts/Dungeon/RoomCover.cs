using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCover : MonoBehaviour
{
    public bool m_Visible;
    public float m_FadeSpeed;
    private float m_Alpha;
    [SerializeField] private SpriteRenderer m_Sprite;

    void Awake()
    {
        m_Alpha = 1.0f;
        m_Sprite.color = new Color(m_Sprite.color.r, m_Sprite.color.g, m_Sprite.color.b, m_Alpha);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Visible && m_Alpha > 0.0f)
        {
            m_Alpha -= m_FadeSpeed * Time.deltaTime;
            m_Sprite.color = new Color(m_Sprite.color.r, m_Sprite.color.g, m_Sprite.color.b, m_Alpha);
        }
        else if(!m_Visible && m_Alpha < 1.0f)
        {
            m_Alpha += m_FadeSpeed * Time.deltaTime;
            m_Sprite.color = new Color(m_Sprite.color.r, m_Sprite.color.g, m_Sprite.color.b, m_Alpha);
        }
    }
}
