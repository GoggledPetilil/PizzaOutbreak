using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallMarker : MonoBehaviour
{
    private bool m_Marked;
    [SerializeField] private GameObject m_Particles;
    public GameObject m_MarkerObject;

    // Start is called before the first frame update
    void Start()
    {
        m_MarkerObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(!m_Marked)
            {
                EffectsManager.instance.SpawnPosEffect(m_Particles, other.transform.position);
                m_MarkerObject.SetActive(true);
                m_Marked = true;
            }
        }
    }

}
