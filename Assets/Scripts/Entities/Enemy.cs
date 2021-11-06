using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : Entity
{
    public enum EntityState
    {
        Idle,
        Lunge,
        Shoved
    }

    [Header("Overview")]
    public EntityState m_State;
    public bool m_ChasingPlayer;

    [Header("Pathfinding")]
    public Transform m_Target;
    private Vector2 m_TargetPos;
    private Vector2 m_TargetOffset;
    public float m_NextWaypointDistance = 3f;
    private Path m_Path;
    private int m_CurrentWaypoint = 0;
    private bool m_ReachedEndPath = false;
    private float m_ZombieCollision = 0f;
    Seeker m_Seeker;

    [Header("Lunge Parameters")]
    public float m_LungeSpeed;
    public float m_LungeTime;
    public float m_LungeCooldown; // How often the enemy can lunge.
    public float m_LungeDistanceReq; // How close the enemy needs to be in order to lunge.
    private float m_LungeLeft;
    private float m_NextLunge;

    // Start is called before the first frame update
    void Start()
    {
        DungeonManager.instance.m_AllEnemies.Add(this);

        m_FacingDir = Vector2.down; // Prevents the facing dir from being 0.
        m_Anim.SetFloat("Horizontal", m_FacingDir.x);
        m_Anim.SetFloat("Vertical", m_FacingDir.y);

        m_Seeker = GetComponent<Seeker>();
        GetRandomTarget();
        InvokeRepeating("UpdatePath", 0f, 0.5f);

        m_CanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case EntityState.Idle:
              // Movement
              MovementLogic();
              break;
            case EntityState.Lunge:
              m_CanMove = false;
              m_LungeLeft -= Time.deltaTime;

              if(m_LungeLeft <= 0)
              {
                  StopDust();
                  m_NextLunge = Time.time + m_LungeCooldown;
                  m_CanMove = true;
                  m_State = EntityState.Idle;
              }
              break;
            case EntityState.Shoved:
              if(m_ShoveLeft > 0)
              {
                  m_CanMove = false;
                  m_ShoveLeft -= Time.deltaTime;
              }

              if(m_ShoveLeft <= 0)
              {
                  StopDust();
                  m_CanMove = true;
                  m_State = EntityState.Idle;
              }
              break;
        }
    }

    void FixedUpdate()
    {
        switch (m_State)
        {
            case EntityState.Idle:
              Movement();
              break;
            case EntityState.Lunge:
              m_Body.velocity = m_FacingDir * m_LungeSpeed;
              break;
            case EntityState.Shoved:
              m_Body.velocity = m_ShoveDir * m_ShoveSpeed;
              break;
        }
    }

    void Movement()
    {
        if(m_CanMove == false || m_Path == null) return;

        if(m_CurrentWaypoint >= m_Path.vectorPath.Count - 1)
        {
            m_ReachedEndPath = true;
            if(!m_ChasingPlayer)
            {
                GetRandomTarget();
            }
            else
            {
                if(Time.time > m_NextLunge && Vector2.Distance(this.transform.position, m_Target.position) < m_LungeDistanceReq)
                {
                    CreateDust();
                    m_LungeLeft = m_LungeTime;
                    m_FacingDir = -TurnTo(m_Target);
                    m_State = EntityState.Lunge;
                }

            }
            return;
        }
        else
        {
            m_ReachedEndPath = false;
        }

        if(m_ChasingPlayer)
        {
            m_Body.velocity = m_MovDir * (m_Speed * 1.2f);
        }
        else
        {
            m_Body.velocity = m_MovDir * m_Speed;
        }

        float distance = Vector2.Distance(m_Body.position, m_Path.vectorPath[m_CurrentWaypoint]);
        if(distance < m_NextWaypointDistance)
        {
            m_CurrentWaypoint++;
        }
    }

    void MovementLogic()
    {
        if(m_Path == null)
            return;

        m_MovDir = ((Vector2)m_Path.vectorPath[m_CurrentWaypoint] - m_Body.position).normalized;

        if(m_MovDir.x != 0f || m_MovDir.y != 0f  && m_CanMove)
        {
            m_FacingDir = m_MovDir;
            if(!m_ChasingPlayer)
            {
                m_Anim.SetTrigger("Walking");
            }
            else
            {
                m_Anim.SetTrigger("Chasing");
            }
        }
        m_Anim.SetFloat("Horizontal", m_Body.velocity.x);
        m_Anim.SetFloat("Vertical", m_Body.velocity.y);
    }

    public void GetRandomTarget()
    {
        int r = Random.Range(0, DungeonManager.instance.RoomsList.Count - 1);
        SetTarget(DungeonManager.instance.RoomsList[r]);
    }

    public void SetTarget(Transform target)
    {
        if(target.CompareTag("Player"))
        {
            m_ChasingPlayer = true;
            m_NextLunge = Time.time + m_LungeCooldown;
        }
        else
        {
            m_ChasingPlayer = false;
        }

        m_CurrentWaypoint = 1;
        m_ReachedEndPath = false;

        m_Target = target;
        m_TargetOffset = Vector2.zero;
        if(!m_ChasingPlayer)
        {
            m_TargetOffset = new Vector2(Random.Range(-7f, 7f), Random.Range(-3f, 3f));
        }
        else
        {
            m_TargetOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        }
        m_TargetPos = target.position + (Vector3)m_TargetOffset;
    }

    void UpdatePath()
    {
        if(m_Seeker.IsDone())
        {
            m_TargetPos = m_Target.position + (Vector3)m_TargetOffset;
            m_Seeker.StartPath(m_Body.position, m_TargetPos, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            m_Path = p;
            m_CurrentWaypoint = 1;
        }
    }

    public override void Death()
    {
        DungeonManager.instance.m_AllEnemies.Remove(this);
        m_RoomLocation.GetComponent<RoomBehaviour>().m_Entities.Remove(this.transform);

        EffectsManager.instance.SpawnBloodStain(transform.position);
        EffectsManager.instance.SpawnPosEffect(EffectsManager.instance.m_DeathBlood, transform.position);
        Instantiate(LootManager.instance.GetLoot(), transform.position, Quaternion.identity);
        EffectsManager.instance.SpawnPosEffect(EffectsManager.instance.m_Flames, this.transform.position);

        Destroy(this.gameObject, 0.1f);
    }

    IEnumerator IgnoreOther(Collider2D other)
    {
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), other, true);
        yield return new WaitForSeconds(2f);
        if(other != null)
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), other, false);
        }
        m_ZombieCollision = 0.0f;
        yield return null;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if(col.transform.CompareTag("Enemy"))
        {
            m_ZombieCollision += Time.deltaTime;
            if(m_ZombieCollision >= 1f)
            {
                StartCoroutine(IgnoreOther(col.transform.GetComponent<Collider2D>()));
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Attack"))
        {
            Attack a = col.GetComponent<Attack>();
            int damage = a.m_Attack;
            float force = a.m_Force;
            Transform source = a.transform;
            Damaged(damage, force, source);
            EffectsManager.instance.SpawnParentEffect(EffectsManager.instance.m_Flames, this.transform);

            m_State = EntityState.Shoved;
        }

        if(col.gameObject.CompareTag("Room"))
        {
            RoomBehaviour room = col.gameObject.GetComponent<RoomBehaviour>();
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;

            if(room.m_Entities.Contains(player))
            {
                SetTarget(player);
            }
            else if(m_Target == player)
            {
                SetTarget(player.GetComponent<Player>().m_RoomLocation);
            }
        }
    }
}
