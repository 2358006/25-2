using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;

    public Transform patrolRoute;
    public List<Transform> locations;

    int locationIndex = 0;
    NavMeshAgent agent;

    int lives = 3;
    public int enemyLives
    {
        get { return lives; }
        private set
        {
            lives = value;

            if (lives <= 0)
            {
                Destroy(this.gameObject);
                Debug.Log("Enmey Down");
            }
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    void Start()
    {
        InitializePatroalRoute();
        MoveToNextPatrolLocation();
    }

    void Update()
    {
        if (agent.remainingDistance < 0.2f && !agent.pathPending)
        {
            MoveToNextPatrolLocation();
        }
    }

    void InitializePatroalRoute()
    {
        foreach (Transform child in patrolRoute)
        {
            locations.Add(child);
        }
    }

    void MoveToNextPatrolLocation()
    {
        if (locations.Count == 0) { return; }

        agent.destination = locations[locationIndex].position;

        locationIndex = (locationIndex + 1) % locations.Count;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bulelt(Clone)")
        {
            enemyLives -= 1;
            Debug.Log("Critical Hit");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            agent.destination = player.position;
            Debug.Log("Player detected - attack!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            Debug.Log("Player out of range, resume patrol");
        }
    }
}