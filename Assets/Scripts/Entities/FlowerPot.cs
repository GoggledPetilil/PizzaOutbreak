using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Flower;
    [SerializeField] private Sprite[] m_FlowerSprites;

    // Start is called before the first frame update
    void Start()
    {
        UpdateFlower();
    }

    void UpdateFlower()
    {
        int r = Random.Range(0, m_FlowerSprites.Length);
        m_Flower.sprite = m_FlowerSprites[r];
    }
}
