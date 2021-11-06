using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private AudioSource m_Audio;
    [SerializeField] private AudioClip m_ConfirmSFX;

    public Stage m_StageData;

    public bool m_GameStarted;    // To indicate that the game has started, skipping the intro.
    public bool m_PlayerHasDied;  // The player has died.

    [SerializeField] private GameObject m_OneShotAudio;

    void Awake()
    {
        if(GameManager.instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public void LoadLevel(int sceneID)
    {
        Debug.Log("Loading Level #" + sceneID);
        SceneManager.LoadScene(sceneID);
    }

    public AsyncOperation LoadLevelAsync(int sceneID)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);
        return asyncLoad;
    }

    public void PlayOneShot(AudioClip clip)
    {
        GameObject go = Instantiate(m_OneShotAudio) as GameObject;
        go.GetComponent<AudioSource>().clip = clip;
    }

    public void PlayConfirmSFX()
    {
        m_Audio.clip = m_ConfirmSFX;
        m_Audio.Play();
    }
}
