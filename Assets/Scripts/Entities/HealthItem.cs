using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private int m_Amount;
    [SerializeField] private AudioClip m_PickUpSound;

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
            p.RaiseHealth(m_Amount);
            p.PlayHappyVoice();
            GameManager.instance.PlayOneShot(m_PickUpSound);
            EffectsManager.instance.SpawnParentEffect(EffectsManager.instance.m_Healing, p.transform);

            DestroySelf();
        }
    }
}
