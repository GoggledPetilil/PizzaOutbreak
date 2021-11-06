using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    private float m_OriginX;
    private float m_Extents;

    // Start is called before the first frame update
    void Start()
    {
        m_OriginX = this.transform.position.x;
        m_Extents = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.left * m_Speed * Time.deltaTime);

        if(transform.position.x < m_OriginX - (m_Extents * 1.0f))
        {
            transform.position = new Vector2((m_OriginX + m_Extents * 1.0f), transform.position.y);
        }
    }
}
