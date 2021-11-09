using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageSelect : MonoBehaviour
{
    private GameObject m_StageSelectHUD;
    private GameObject m_LoadingCanvas;

    [Header("Stage Select")]
    public Stage[] m_Stages;
    [SerializeField] private TMP_Text m_StageName;
    [SerializeField] private SpriteRenderer m_StageBuilding;
    private int m_Selected; // The current levelID the player has selected.
    private bool m_CanMove; // Player can actually select a stage.

    [Header("Scooter Components")]
    [SerializeField] private ParticleSystem m_BigDust;
    private GameObject m_PlayerScooter;
    private Animator m_ScooterAnimator;

    [Header("Sound Components")]
    [SerializeField] private AudioSource m_Sound;
    [SerializeField] private AudioSource m_Voice;
    public AudioClip m_ScooterVroom;
    public AudioClip[] m_VoiceClips;

    void Awake()
    {
        m_StageSelectHUD = TitleScreen.instance.m_StageSelectHUD;
        m_LoadingCanvas = TitleScreen.instance.m_LoadingCanvas;
        m_PlayerScooter = TitleScreen.instance.m_PlayerScooter;
        m_ScooterAnimator = m_PlayerScooter.GetComponent<Animator>();

        m_StageName.text = "";
    }

    void Start()
    {
        m_StageSelectHUD.SetActive(true);
        m_ScooterAnimator.SetBool("isLookingUp", true);
        StartCoroutine("UpdateStagePreview");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            m_StageSelectHUD.SetActive(!m_StageSelectHUD.activeSelf);
        }

        if(Input.GetKey(KeyCode.RightArrow) && m_CanMove)
        {
            if(m_Selected >= m_Stages.Length - 1)
            {
                m_Selected = 0;
            }
            else
            {
                m_Selected++;
            }
            StartCoroutine("UpdateStagePreview");
        }
        if(Input.GetKey(KeyCode.LeftArrow) && m_CanMove)
        {
            if(m_Selected <= 0)
            {
                m_Selected = m_Stages.Length - 1;
            }
            else
            {
                m_Selected--;
            }
            StartCoroutine("UpdateStagePreview");
        }
        if(Input.GetKeyDown(KeyCode.Z) && m_CanMove)
        {
            m_CanMove = false;
            GameManager.instance.m_GameStarted = true;
            GameManager.instance.m_StageData = m_Stages[m_Selected];
            StartCoroutine("GoToStage");

        }
    }

    void UpdateStage()
    {
        m_StageBuilding.sprite = m_Stages[m_Selected].stageBuilding;
        m_StageName.text = m_Stages[m_Selected].name;
    }

    IEnumerator UpdateStagePreview()
    {
        m_CanMove = false;

        float t = 0.0f;
        float fadeDuration = 0.2f;
        while(t <= 1)
        {
            t += Time.deltaTime / fadeDuration;
            float a = Mathf.Lerp(1.0f, 0.0f, t);

            m_StageBuilding.color = new Color(1f, 1f, 1f, a);

            yield return null;
        }

        UpdateStage();
        yield return null;

        t = 0.0f;
        while(t <= 1)
        {
            t += Time.deltaTime / fadeDuration;
            float a = Mathf.Lerp(0.0f, 1.0f, t);

            m_StageBuilding.color = new Color(1f, 1f, 1f, a);

            yield return null;
        }

        m_CanMove = true;

        yield return null;
    }

    IEnumerator GoToStage()
    {
        m_ScooterAnimator.SetBool("isLookingUp", false);

        m_Voice.clip = m_VoiceClips[Random.Range(0, m_VoiceClips.Length - 1)];
        m_Sound.clip = m_ScooterVroom;
        m_Voice.Play();
        m_Sound.Play();
        m_BigDust.Play();

        Vector2 startPos = m_PlayerScooter.transform.position;
        Vector2 endPos = new Vector2(20.0f, m_PlayerScooter.transform.position.y);
        float t = 0.0f;
        while(t <= 1)
        {
            float driveDuration = 0.8f;
            t += Time.deltaTime / driveDuration;
            m_PlayerScooter.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        m_LoadingCanvas.SetActive(true);
        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = GameManager.instance.LoadLevelAsync(m_Stages[m_Selected].levelID);
        asyncLoad.allowSceneActivation = false;

        while(!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
