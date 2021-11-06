using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public enum RoomType
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public RoomType m_RoomType;
    private float m_SpawnTime;

    private List<GameObject> m_Contacts = new List<GameObject>(); // A list of all the objects this point has touched.

    public void Start()
    {
        DungeonManager.instance.m_GenerationTimer = 1f;
        Invoke("SpawnRoom", 0.05f);
    }

    public void SpawnRoom()
    {
        int max = 7;
        int r = 0;
        GameObject[] points = GameObject.FindGameObjectsWithTag("RoomSpawnPoint");
        int roomAmount = DungeonManager.instance.RoomsList.Count + points.Length;

        if(roomAmount < DungeonManager.instance.m_MinRooms)
        {
            r = Random.Range(1, max);
        }
        else if(roomAmount >= DungeonManager.instance.m_MaxRooms)
        {
            r = 0;
        }
        else
        {
            r = Random.Range(0, max);
        }

        switch (m_RoomType)
        {
            case RoomType.Top:
                Instantiate(DungeonManager.instance.templates.m_TopRooms[r], transform.position, Quaternion.identity);
                if(transform.position.y > DungeonManager.instance.m_FurthestUp)
                {
                    DungeonManager.instance.m_FurthestUp = transform.position.y;
                }
                break;
            case RoomType.Left:
                Instantiate(DungeonManager.instance.templates.m_LeftRooms[r], transform.position, Quaternion.identity);
                if(transform.position.x < DungeonManager.instance.m_FurthestLeft)
                {
                    DungeonManager.instance.m_FurthestLeft = transform.position.x;
                }
                break;
            case RoomType.Bottom:
                Instantiate(DungeonManager.instance.templates.m_BottomRooms[r], transform.position, Quaternion.identity);
                if(transform.position.y < DungeonManager.instance.m_FurthestDown)
                {
                    DungeonManager.instance.m_FurthestDown = transform.position.y;
                }
                break;
            case RoomType.Right:
                Instantiate(DungeonManager.instance.templates.m_RightRooms[r], transform.position, Quaternion.identity);
                if(transform.position.x > DungeonManager.instance.m_FurthestRight)
                {
                    DungeonManager.instance.m_FurthestRight = transform.position.x;
                }
                break;
        }
        SpawnMarker();

        Destroy(this.gameObject);
    }

    void SpawnMarker()
    {
        Vector2 basePos = this.transform.root.position;
        Vector2 posOffset = Vector2.zero;

        switch (m_RoomType)
        {
            case RoomType.Top:
                posOffset = new Vector2(0.0f, 5f);
                Instantiate(DungeonManager.instance.templates.m_HorizontalMarker, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
            case RoomType.Left:
                posOffset = new Vector2(-9f, -1f);
                Instantiate(DungeonManager.instance.templates.m_VerticalMarker, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
            case RoomType.Bottom:
                posOffset = new Vector2(0.0f, -5f);
                Instantiate(DungeonManager.instance.templates.m_HorizontalMarker, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
            case RoomType.Right:
                posOffset = new Vector2(9f, -1f);
                Instantiate(DungeonManager.instance.templates.m_VerticalMarker, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
        }
    }

    void CoverThisRoom()
    {
        Vector2 basePos = this.transform.root.position;
        Vector2 posOffset = Vector2.zero;
        switch (m_RoomType)
        {
            case RoomType.Top:
                posOffset = new Vector2(0.0f, 10f);
                Instantiate(DungeonManager.instance.templates.m_TopCover, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
            case RoomType.Left:
                posOffset = new Vector2(-18f, 0.0f);
                Instantiate(DungeonManager.instance.templates.m_LeftCover, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
            case RoomType.Bottom:
                posOffset = new Vector2(0.0f, -10f);
                Instantiate(DungeonManager.instance.templates.m_BottomCover, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
            case RoomType.Right:
                posOffset = new Vector2(18f, 0f);
                Instantiate(DungeonManager.instance.templates.m_RightCover, basePos + posOffset, Quaternion.identity, this.transform.root);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!m_Contacts.Contains(other.gameObject))
        {
            m_Contacts.Add(other.gameObject);
        }

        if(other.gameObject.CompareTag("Room"))
        {
            RoomBehaviour room = other.gameObject.GetComponent<RoomBehaviour>();

            switch (m_RoomType)
            {
                case RoomType.Top:
                    if(room.m_BottomOpen == false)
                    {
                        CoverThisRoom();
                    }
                    break;
                case RoomType.Left:
                    if(room.m_RightOpen == false)
                    {
                        CoverThisRoom();
                    }
                    break;
                case RoomType.Bottom:
                    if(room.m_TopOpen == false)
                    {
                        CoverThisRoom();
                    }
                    break;
                case RoomType.Right:
                    if(room.m_LeftOpen == false)
                    {
                        CoverThisRoom();
                    }
                    break;
            }
            Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("RoomSpawnPoint"))
        {
            // Get the root RoomList ID for both this and other to see which one is older.
            int thisIndex = DungeonManager.instance.RoomsList.IndexOf(this.transform.root);
            int otherIndex = DungeonManager.instance.RoomsList.IndexOf(other.transform.root);

            if(thisIndex > otherIndex)
            {
                // Check to see if this space is empty.
                // If it is, then cover this passage.
                /*if(m_Contacts.Count == 1)
                {
                    CoverThisRoom();
                    // Now tell the middlepoint that this passageway is blocked.
                    RoomBehaviour room = this.transform.root.GetComponentInChildren<RoomBehaviour>();
                    switch (m_RoomType)
                    {
                        case RoomType.Top:
                            room.m_TopOpen = false;
                            break;
                        case RoomType.Left:
                            room.m_LeftOpen = false;
                            break;
                        case RoomType.Bottom:
                            room.m_BottomOpen = false;
                            break;
                        case RoomType.Right:
                            room.m_RightOpen = false;
                            break;
                    }

                    Debug.Log("Spawn Point conflicted!");
                }*/

                Destroy(this.gameObject);
            }
        }
    }
}
