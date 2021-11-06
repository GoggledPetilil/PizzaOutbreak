using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodstain : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Sprite;
    [SerializeField] private Sprite[] m_BloodSprites;
    [SerializeField] private float cleanDuration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
          int r = Random.Range(0, m_BloodSprites.Length);
          m_Sprite.sprite = m_BloodSprites[r];
    }

    public void DeleteSelf()
    {
        EffectsManager.instance.m_BloodList.Remove(this.transform);
        StartCoroutine("DeleteBlood");
        Destroy(this.gameObject, cleanDuration);
    }

    IEnumerator DeleteBlood()
    {
        float t = 0.0f;
        while(t <= 1)
        {
            t += Time.deltaTime / cleanDuration;
            m_Sprite.color = new Color(m_Sprite.color.r, m_Sprite.color.g, m_Sprite.color.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
    }
}
