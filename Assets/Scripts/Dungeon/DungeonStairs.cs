using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonStairs : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite m_TopSprite;
    [SerializeField] private Sprite m_LeftSprite;
    [SerializeField] private Sprite m_BottomSprite;
    [SerializeField] private Sprite m_RightSprite;
    [SerializeField] private GameObject m_DoorObject;
    private bool m_Entered;

    public void FaceDirection(Vector2 dir)
    {
        m_DoorObject.SetActive(false);
        if(dir == Vector2.up)
        {
            m_SpriteRenderer.sprite = m_TopSprite;
            m_DoorObject.SetActive(true);
        }
        else if(dir == Vector2.left)
        {
            m_SpriteRenderer.sprite = m_LeftSprite;
        }
        else if(dir == Vector2.down)
        {
            m_SpriteRenderer.sprite = m_BottomSprite;
        }
        else if(dir == Vector2.right)
        {
            m_SpriteRenderer.sprite = m_RightSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && m_Entered == false)
        {
            other.gameObject.GetComponent<Player>().Revert();
            PauseManager.instance.Resume();
            DungeonManager.instance.CreateNewFloor();

            m_Entered = true;
        }
    }
}
