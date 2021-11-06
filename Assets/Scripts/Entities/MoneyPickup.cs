using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [SerializeField] private float m_Worth;
    [SerializeField] private AudioClip m_PickUpSound;
    [SerializeField] private bool m_Special;
    private Rigidbody2D m_rb;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Invoke("DestroySelf", 20f);
    }

    void DestroySelf()
    {
        EffectsManager.instance.SpawnPosEffect(EffectsManager.instance.m_Sparkles, this.transform.position);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            Player p = collision.transform.GetComponent<Player>();
            p.ChangeMoney(m_Worth);
            if(m_Special)
            {
                p.PlayHappyVoice();
            }
            GameManager.instance.PlayOneShot(m_PickUpSound);

            DestroySelf();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Attack"))
        {
            float speed = col.gameObject.GetComponent<Attack>().m_Force;
            Vector2 dir = (transform.position - col.transform.position).normalized;
            m_rb.AddForce(dir * speed * 100f);
        }
    }
}
