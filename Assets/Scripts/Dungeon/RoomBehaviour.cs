using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    [Header("Room Components")]
    public RoomCover m_Cover;
    public GameObject m_PointHolder;
    public Transform m_RoomQuirk; // This can be stuff like stairs or chests.
    public List<Transform> m_Entities = new List<Transform>();

    [Header("Room Openings")]
    public bool m_TopOpen;
    public bool m_LeftOpen;
    public bool m_BottomOpen;
    public bool m_RightOpen;
    // If any of these are true, it means that side of the room is a passageway.

    void Awake()
    {
        DungeonManager.instance.RoomsList.Add(this.transform.root);
        Invoke("RoomDesign", 0.05f);
    }

    void RoomDesign()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        int openingCount = m_TopOpen.GetHashCode() + m_LeftOpen.GetHashCode() + m_BottomOpen.GetHashCode() + m_RightOpen.GetHashCode();
        if(openingCount == 1 && pos != Vector2.zero)
        {
            // Room is a dead end, see if stairs or other loot can be spawned.
            float r = Random.Range(0.1f, 1.0f);

            if(r <= DungeonManager.instance.m_StairSpawnRate)
            {
                SpawnStairs();
            }
            else
            {
                // Stairs haven't spawned, so increase stair rate.
                DungeonManager.instance.m_StairSpawnRate += 0.25f;

                // Spawn something else now.
                if(r <= 0.8)
                {
                    int q = Random.Range(0, 4);
                    if(q == 0)
                    {
                        SpawnQuirk(DungeonManager.instance.templates.m_HealthPickup);
                    }
                    else
                    {
                        SpawnQuirk(DungeonManager.instance.templates.m_Treasure);
                    }
                }
            }
        }
        else
        {
            // Decorate the room
            int d = Random.Range(0, DungeonManager.instance.templates.m_RoomsDecor.Count);
            int i = Random.Range(0, 2);
            if(i == 0) Instantiate(DungeonManager.instance.templates.m_RoomsDecor[d], pos, Quaternion.identity, this.gameObject.transform.root);
            // Rug
            i = Random.Range(0, 3);
            if(i == 0) Instantiate(DungeonManager.instance.templates.m_Rug, pos + new Vector2(0, -1), Quaternion.identity, this.gameObject.transform.root);
        }
    }

    public void SpawnStairs()
    {
        DeleteQuirk();

        // Stairs have been spawned.
        Vector2 basePos = this.transform.position;

        Vector2 offSet = Vector2.zero;
        Vector2 dir = Vector2.zero;
        if(!m_TopOpen)
        {
            offSet = new Vector2(0.0f, 1.5f);
            dir = Vector2.up;
            // Spawn a door here too.
        }
        else if(!m_LeftOpen)
        {
            offSet = new Vector2(-7.5f, -1.0f);
            dir = Vector2.left;
        }
        else if(!m_BottomOpen)
        {
            offSet = new Vector2(0.0f, -3.5f);
            dir = Vector2.down;
        }
        else if(!m_RightOpen)
        {
            offSet = new Vector2(7.5f, -1.0f);
            dir = Vector2.right;
        }
        GameObject q = Instantiate(DungeonManager.instance.templates.m_Stairs, basePos + offSet, Quaternion.identity, this.gameObject.transform.root) as GameObject;
        m_RoomQuirk = q.transform;
        q.GetComponent<DungeonStairs>().FaceDirection(dir);

        DungeonManager.instance.m_StairSpawnRate = 0.0f;
        DungeonManager.instance.m_StairsSpawned = true;
    }

    public void SpawnQuirk(GameObject obj)
    {
        GameObject q = Instantiate(obj, this.transform.position, Quaternion.identity, this.gameObject.transform.root) as GameObject;
        m_RoomQuirk = q.transform;
    }

    public void DeleteQuirk()
    {
        if(m_RoomQuirk != null)
        {
            Destroy(m_RoomQuirk.gameObject);
            m_RoomQuirk = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            m_Entities.Add(other.transform);
            other.gameObject.GetComponent<Entity>().m_RoomLocation = this.transform;

            if(other.gameObject.CompareTag("Player"))
            {
                m_Cover.m_Visible = true;
                // Tell the zombies in this room to notice the player!
                foreach(var enemy in m_Entities)
                {
                    if(enemy == null) return;

                    Enemy zombie = enemy.GetComponent<Enemy>();
                    if(zombie != null)
                    {
                        zombie.SetTarget(other.transform);
                    }
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            m_Entities.Remove(other.transform);

            if(other.gameObject.CompareTag("Player"))
            {
                m_Cover.m_Visible = false;
            }
        }
    }
}
