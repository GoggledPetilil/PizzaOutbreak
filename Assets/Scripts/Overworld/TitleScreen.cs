using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public static TitleScreen instance;
    [SerializeField] private AudioClip m_StartSound;

    [Header("HUDs")]
    [SerializeField] private StageSelect m_StageSelect;
    public GameObject m_TitleHUD;
    public GameObject m_StageSelectHUD;
    public GameObject m_LoadingCanvas;
    [SerializeField] private GameObject m_FadeIn;

    [Header("Player Components")]
    public GameObject m_PlayerScooter;
    [SerializeField] private ParticleSystem m_ScooterDust;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_StageSelect.enabled = false;
        m_TitleHUD.SetActive(true);
        m_StageSelectHUD.SetActive(false);
        m_LoadingCanvas.SetActive(false);
        m_FadeIn.SetActive(false);

        if(GameManager.instance.m_GameStarted)
        {
            m_TitleHUD.SetActive(false);
            m_FadeIn.SetActive(true);

            if(GameManager.instance.m_PlayerHasDied)
            {
                m_FadeIn.SetActive(false);
                m_PlayerScooter.transform.position = new Vector2(-7, m_PlayerScooter.transform.position.y);
                StartCoroutine("PlayerRevives");
            }
            else
            {
                m_PlayerScooter.transform.position = new Vector2(-14, m_PlayerScooter.transform.position.y);
                StartCoroutine("PlayerDrivesIn");
            }

            m_StageSelect.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && GameManager.instance.m_GameStarted == false)
        {
            GameManager.instance.m_GameStarted = true;
            StartCoroutine("StartGame");
        }
    }

    IEnumerator StartGame()
    {
        GameManager.instance.PlayOneShot(m_StartSound);
        m_TitleHUD.GetComponent<Animator>().SetTrigger("Start");

        m_ScooterDust.Stop();
        Vector2 startPos = new Vector2(7.0f, m_PlayerScooter.transform.position.y);
        Vector2 endPos = new Vector2(-7.0f, m_PlayerScooter.transform.position.y);
        float t = 0.0f;
        while(t <= 1)
        {
            float duration = 1.5f;
            t += Time.deltaTime / duration;
            m_PlayerScooter.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        m_ScooterDust.Play();

        m_TitleHUD.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.6f);
        m_TitleHUD.SetActive(false);

        // You get asked if u wanna log in to NewGrounds.

        m_StageSelect.enabled = true;
        yield return null;
    }

    IEnumerator PlayerRevives()
    {
        m_PlayerScooter.GetComponent<Animator>().SetTrigger("Died");

        yield return new WaitForSeconds(1.0f);

        GameManager.instance.m_PlayerHasDied = false;

        yield return null;
    }

    IEnumerator PlayerDrivesIn()
    {
        Vector2 startPos = new Vector2(-14.0f, m_PlayerScooter.transform.position.y);
        Vector2 endPos = new Vector2(-7.0f, m_PlayerScooter.transform.position.y);
        float t = 0.0f;
        while(t <= 1)
        {
            float duration = 1.2f;
            t += Time.deltaTime / duration;
            m_PlayerScooter.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        yield return null;
    }
}
