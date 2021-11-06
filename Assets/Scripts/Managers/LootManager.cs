using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance;

    [Header("Loot Table")]
    [SerializeField] private GameObject[] m_Drops;
    [SerializeField] private int[] m_DropRates;
    [SerializeField] private int[] m_CumulativeRates; // This makes it so that sorting m_DropRates from common to rarest isn't needed.

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CumulativeRates = new int[m_DropRates.Length];

        for (int i = 0; i < m_DropRates.Length; i++)
        {
            if(i - 1 < 0)
            {
                m_CumulativeRates[i] += (0 + m_DropRates[i]);
            }
            else
            {
                m_CumulativeRates[i] += (m_CumulativeRates[i - 1] + m_DropRates[i]);
            }
        }
    }

    public GameObject GetLoot()
    {
        int r = Random.Range(0, m_CumulativeRates[m_CumulativeRates.Length - 1] + 1);
        GameObject drop = null;
        for(int i = 0; i < m_CumulativeRates.Length; i++)
        {
            if(r <= m_CumulativeRates[i])
            {
                drop = m_Drops[i];
                return drop;
            }
        }

        return drop;
    }
}
