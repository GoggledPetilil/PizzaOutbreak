using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [Header("Parameters")]
    public int m_Health;
    public int m_DropAmount;
    [SerializeField] private bool m_Box; // This is a box that can be opened.
    private bool m_Looted;

    [Header("Components")]
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private AudioClip m_OpenSFX;

    void Awake()
    {
        m_rb = this.GetComponent<Rigidbody2D>();
    }

    void ReduceHealth()
    {
        m_Health--;
        EffectsManager.instance.SpawnParentEffect(EffectsManager.instance.m_Flames, this.transform);

        if(m_Health <= 0)
        {
            EffectsManager.instance.SpawnPosEffect(EffectsManager.instance.m_Flames, this.transform.position);
            Destroy(this.gameObject);
        }
    }

    void DropLoot()
    {
        if(!m_Looted)
        {
            m_Looted = true;
            if(m_Box && m_OpenSFX != null)
            {
                GameManager.instance.PlayOneShot(m_OpenSFX);
            }
            SpawnMoney();
        }
    }

    void SpawnMoney()
    {
        Vector2 dir;
        float speed;
        for(int i = 0; i < m_DropAmount; i++)
        {
            GameObject go = Instantiate(LootManager.instance.GetLoot(), this.transform.position, Quaternion.identity) as GameObject;
            dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            speed = Random.Range(1f, 5f);
            go.GetComponent<Rigidbody2D>().AddForce(dir * speed * 100f);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Attack"))
        {
            float speed = col.gameObject.GetComponent<Attack>().m_Force;
            Vector2 dir = (transform.position - col.transform.position).normalized;
            m_rb.AddForce(dir * speed * 300f);

            DropLoot();
            ReduceHealth();
        }
    }
}
