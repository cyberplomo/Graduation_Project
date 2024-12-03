using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControllerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public NavMeshAgent NavMeshAgent;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask PlayerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float edgeDistance = 0.5F;

    public Transform[] waypoints;
    private int m_CurrentWaypointIndex;

    private Vector3 playerLastPosition = Vector3.zero;
    private Vector3 m_PlayerPosition;

    private float m_WaitTime;
    private float m_TimeToRotate;
    private bool m_PlayerInRange;
    private bool m_PlayerNear;
    private bool m_IsPatrol;
    private bool m_CaughtPlayer;


    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;
        NavMeshAgent = GetComponent<NavMeshAgent>();

        NavMeshAgent.isStopped = false;
        NavMeshAgent.speed = speedWalk;
        NavMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

    }

    // Update is called once per frame
    void Update()
    {
        EnviromentView();
        if (!m_IsPatrol)
        {
            Chasing();          
        }
        else
        {
            Patroling();
        }
    }

    private void Chasing()
    {
        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            Move(speedRun);
            NavMeshAgent.SetDestination(m_PlayerPosition);
        }

        if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position,
                    GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_TimeToRotate = timeToRotate;
                m_WaitTime = startWaitTime;
                NavMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position,
                        GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }
    private void Patroling()
    {
        if (m_PlayerNear)
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;
            NavMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }
    void Move(float speed)
    {
        NavMeshAgent.isStopped = false;
        NavMeshAgent.speed = speed;
    }

    void Stop()
    {
        NavMeshAgent.isStopped = true;
        NavMeshAgent.speed = 0;

    }

    public void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        NavMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

    }

    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        NavMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                NavMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, PlayerMask);
        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                }
                else
                {
                    m_PlayerInRange = false;
                }
            }

            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                m_PlayerInRange = false;
            }

            if (m_PlayerInRange)
            {
                m_PlayerPosition = player.position; 
            }
            
            //if (m_PlayerInRange)
            //{
             //   m_PlayerPosition = playerInRange.transform.position; 
           // }

        }
    }
}