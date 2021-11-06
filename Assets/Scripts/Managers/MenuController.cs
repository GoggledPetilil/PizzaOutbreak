using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Header("Menu Controller")]
    [SerializeField] protected EventSystem m_EventSystem;
    [SerializeField] protected AudioMixer m_AudioMixer;
    [SerializeField] protected GameObject m_SelectedObject; // The interactable that the game is supposed to have selected.

    protected void Init()
    {
        m_EventSystem.SetSelectedGameObject(null);
        m_SelectedObject = m_EventSystem.firstSelectedGameObject;
    }

    protected void CheckSelected()
    {
        if(m_EventSystem.currentSelectedGameObject == null)
        {
            m_EventSystem.SetSelectedGameObject(m_SelectedObject);
        }
        else if(m_EventSystem.currentSelectedGameObject != m_SelectedObject)
        {
            // Because this only activates when the currentSelectedObject != null,
            // m_SelectedObject will never be null.
            m_SelectedObject = m_EventSystem.currentSelectedGameObject;
        }
    }
}
