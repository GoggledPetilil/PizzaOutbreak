using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Parameters")]
    public int m_Speed;
    public float m_DestrucTime;
    [SerializeField] private AudioClip m_ShootSound;
    [SerializeField] private GameObject m_Explosion;
    [SerializeField] private float m_shakeDur;
    [SerializeField] private float m_shakeMag;
    [SerializeField] private float m_shakePow;
    private bool detonated;

    [Header("Components")]
    [SerializeField] private AudioSource m_Audio;
    [SerializeField] private Rigidbody2D m_Body;
    [SerializeField] private ParticleSystem m_Smoke;
    [SerializeField] private SpriteRenderer m_Sprite;
    [SerializeField] private BoxCollider2D m_Collider;

    void Start()
    {
        m_Audio.clip = m_ShootSound;
        m_Audio.Play();
        Invoke("SpawnExplosion", m_DestrucTime);
    }

    void FixedUpdate()
    {
        m_Body.velocity = transform.right * m_Speed;
    }

    void SpawnExplosion()
    {
        if(detonated) return;

        m_Sprite.enabled = false;
        m_Collider.enabled = false;
        detonated = true;
        Instantiate(m_Explosion, transform.position, Quaternion.identity);
        CameraManager.instance.ShakeCamera(m_shakeDur, m_shakeMag, m_shakePow);
        m_Smoke.Stop(); // The particle system will destroy the object once it's finished.
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Obstruct"))
        {
            SpawnExplosion();
        }
    }
}
