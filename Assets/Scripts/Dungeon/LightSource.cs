using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    [SerializeField] private GameObject m_LitObject;
    [SerializeField] private GameObject m_UnlitObject;

    void Start()
    {
        int r = Random.Range(0, 10);
        if(r == 0)
        {
            InvokeRepeating("Flickering", 0.1f, Random.Range(4.0f, 6.6f));
        }
    }

    void Light()
    {
        m_LitObject.SetActive(true);
        m_UnlitObject.SetActive(false);
    }

    void Unlit()
    {
        m_LitObject.SetActive(false);
        m_UnlitObject.SetActive(true);
    }

    void Flickering()
    {
        StartCoroutine("FlickerLight");
    }

    IEnumerator FlickerLight()
    {
        float delay = 0.05f;
        int flickerTimes = 3;

        for (int i = 0; i < flickerTimes; i++)
        {
            Light();
            yield return new WaitForSeconds(delay * 1.5f);
            Unlit();
            yield return new WaitForSeconds(delay);
        }

        yield return null;
    }
}
