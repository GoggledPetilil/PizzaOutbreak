using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;
    public RoomTemplates templates;

    [Header("Dungeon Data")]
    public int m_Floor;
    public int m_MaxFloors;
    public int m_MinRooms = 4;
    public int m_MaxRooms;
    public List<Transform> RoomsList = new List<Transform>();
    public float m_GenerationTimer;
    private bool m_GenerationDone;

    [Header("Enemy Data")]
    public int m_MaxEnemies;
    public GameObject[] m_EnemyPrefabs;
    public List<Enemy> m_AllEnemies = new List<Enemy>();

    [Header("Stairs Data")]
    public float m_StairSpawnRate;
    public bool m_StairsSpawned;

    [Header("Pathfinder")]
    public float m_FurthestUp;
    public float m_FurthestDown;
    public float m_FurthestRight;
    public float m_FurthestLeft;

    [Header("UI Components")]
    public TMP_Text m_DungeonNameText;
    public TMP_Text m_FloorText;
    [SerializeField] private Animator m_ScreenFade;
    [SerializeField] private GameObject m_DeathScreen;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Stage data = GameManager.instance.m_StageData;

        if(data != null)
        {
            m_DungeonNameText.text = data.name;
            m_MaxFloors = data.floors;
            m_MaxRooms = data.maxRooms;
        }
        else
        {
            m_DungeonNameText.text = "Unknown Dungeon";
            m_MaxFloors = 99;
            m_MaxRooms = 10;
        }

        SetFloor(1);
        NewLayout();
        InvokeRepeating("SpawnNewEnemy", 1f, 1f);
    }

    void Update()
    {
        if(m_GenerationTimer > 0.0f)
        {
            m_GenerationTimer -= Time.deltaTime;
            if(m_GenerationTimer <= 0.0f && !m_GenerationDone)
            {
                if(!m_StairsSpawned)
                {
                    RoomsList[RoomsList.Count - 1].GetComponentInChildren<RoomBehaviour>().SpawnStairs();
                    m_StairsSpawned = true;
                }
                CreateGrid();
                FadeIn();
                GameObject player = GameObject.FindWithTag("Player");
                m_MaxEnemies = 1 + Mathf.RoundToInt(Random.Range(RoomsList.Count * 1.5f, RoomsList.Count * 2));
                m_GenerationDone = true;
                player.GetComponent<Player>().m_CanMove = true;
            }
        }
    }

    public void ClearFloor()
    {
        foreach (var go in RoomsList)
        {
            Destroy(go.gameObject);
        }
        RoomsList.Clear();
        foreach (var go in m_AllEnemies)
        {
            Destroy(go.gameObject);
        }
        m_AllEnemies.Clear();
        GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");
        foreach(var go in pickUps)
        {
            Destroy(go.gameObject);
        }

        EffectsManager.instance.ClearBloodstains();

        m_MaxEnemies = 0;
        m_FurthestUp = 0;
        m_FurthestDown = 0;
        m_FurthestRight = 0;
        m_FurthestLeft = 0;
    }

    public void NewLayout()
    {
        ClearFloor();

        GameObject obj = null;
        int r = Random.Range(0, 4);
        Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        switch (r)
        {
            case 0:
              obj = templates.m_TopRooms[0];
              p.m_FacingDir = Vector2.down;
              break;
            case 1:
              obj = templates.m_LeftRooms[0];
              p.m_FacingDir = Vector2.right;
              break;
            case 2:
              obj = templates.m_BottomRooms[0];
              p.m_FacingDir = Vector2.up;
              break;
            case 3:
              obj = templates.m_RightRooms[0];
              p.m_FacingDir = Vector2.left;
              break;
        }
        Instantiate(obj, Vector3.zero, Quaternion.identity);

        m_StairSpawnRate = 0.0f;
        m_StairsSpawned = false;
        m_GenerationTimer = 0.5f;
        m_GenerationDone = false;
    }

    public void SetFloor(int number)
    {
        m_Floor = number;
        m_FloorText.text = number.ToString() + "F";
    }

    public void CreateNewFloor()
    {
        StartCoroutine("NewFloor");
    }

    void CreateGrid()
    {
        AstarData data = AstarPath.active.astarData;
        GridGraph m_Grid = data.gridGraph;

        float posX = (m_FurthestLeft + m_FurthestRight) / 2;
        float posY = (m_FurthestUp + m_FurthestDown) / 2;
        m_Grid.center = new Vector3(posX, posY, -5f);

        int width = Mathf.RoundToInt((m_FurthestRight + 9) - (m_FurthestLeft - 9));
        int depth = Mathf.RoundToInt((m_FurthestUp + 5) - (m_FurthestDown - 5));
        int nodeSize = 1;
        m_Grid.SetDimensions(width, depth, nodeSize);

        AstarPath.active.Scan();
    }

    void SpawnNewEnemy()
    {
        if(m_Floor <= 1) return;

        if(m_GenerationDone && m_AllEnemies.Count < m_MaxEnemies)
        {
            int rp = Random.Range(0, m_EnemyPrefabs.Length);
            int rr = Random.Range(0, RoomsList.Count);
            GameObject player = GameObject.FindWithTag("Player");
            while(RoomsList[rr].GetComponentInChildren<RoomBehaviour>().m_Entities.Contains(player.transform) == true)
            {
                rr = Random.Range(0, RoomsList.Count);
            }

            Instantiate(m_EnemyPrefabs[rp], RoomsList[rr].position, Quaternion.identity);
        }
    }

    public void FadeIn()
    {
        m_ScreenFade.SetTrigger("Fade_In");
    }

    public void FadeOut()
    {
        m_ScreenFade.SetTrigger("Fade_Out");
    }

    public void GameOver()
    {
        StartCoroutine("GameOverProcess");
    }

    IEnumerator GameOverProcess()
    {
        m_DeathScreen.SetActive(true);

        yield return new WaitForSeconds(2f);

        AsyncOperation asyncLoad = GameManager.instance.LoadLevelAsync(0);
        asyncLoad.allowSceneActivation = false;

        while(!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= 0.9f)
            {
                // PLay some kinda sound
                // Wait for sound to finish
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator NewFloor()
    {
        float fadeTime = (1f / 3f);
        GameObject player = GameObject.FindWithTag("Player");

        player.GetComponent<Player>().m_CanMove = false;
        player.GetComponent<Player>().m_MovDir = Vector2.zero;
        FadeOut();
        yield return new WaitForSeconds(fadeTime);

        NewLayout();
        SetFloor(m_Floor + 1);

        player.transform.position = new Vector3(0.0f, 0.0f, player.transform.position.z);

        yield return null;
    }
}
