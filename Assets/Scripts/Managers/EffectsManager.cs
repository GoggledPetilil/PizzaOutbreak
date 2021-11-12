using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [Header("Particle Effects")]
    public GameObject m_BloodParticles;
    public GameObject m_DeathBlood;
    public GameObject m_Flames;
    public GameObject m_Sparkles;
    public GameObject m_Healing;

    [Header("Blood Stains")]
    [SerializeField] private GameObject m_BloodStain;
    [SerializeField] private int m_MaxBlood;
    public List<Transform> m_BloodList = new List<Transform>();


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void SpawnPosEffect(GameObject effect, Vector3 pos)
    {
        Instantiate(effect, pos, Quaternion.identity);
    }

    public void SpawnParentEffect(GameObject effect, Transform parent)
    {
        Instantiate(effect, parent);
    }

    public void SpawnBloodStain(Vector3 pos)
    {
        GameObject go = Instantiate(m_BloodStain, pos, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f))) as GameObject;
        m_BloodList.Add(go.transform);
        UpdateBloodList();
    }

    public void UpdateBloodList()
    {
        int q = GameManager.instance.m_QualityLevel;
        if(q == 0)
        {
            m_MaxBlood = 5;
        }
        else if(q == 1)
        {
            m_MaxBlood = 30;
        }
        else if(q >= 2)
        {
            m_MaxBlood = 80;
        }

        if(m_BloodList.Count > m_MaxBlood)
        {
            for (int i = m_MaxBlood; i < m_BloodList.Count; i++)
            {
                m_BloodList[i].GetComponent<Bloodstain>().DeleteSelf();
            }
        }
    }

    public void ClearBloodstains()
    {
        foreach(var go in m_BloodList)
        {
            Destroy(go.gameObject);
        }
        m_BloodList.Clear();
    }
}
