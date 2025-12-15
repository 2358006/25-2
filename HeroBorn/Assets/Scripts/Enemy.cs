using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bullet;
    public float bulletSpeed = 100f;
    public float fireRate = 3f;

    // NavMesh
    Transform player;
    Transform patrolRoute;
    List<Transform> locations;
    NavMeshAgent agent;
    int locationIndex = 0;

    int lives = 1;
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
                GameManager.instance.enemys++;
            }
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        patrolRoute = GameObject.Find("PatrolRoute").transform;

        locations = new List<Transform>();
    }

    void Start()
    {
        InitializePatroalRoute();
        MoveToNextPatrolLocation();
        StartCoroutine(Shoot());
    }

    void Update()
    {
        if (agent.remainingDistance < 0.2f && !agent.pathPending) { MoveToNextPatrolLocation(); }
        if (GameManager.instance.isGameFinished) { Destroy(this.transform.gameObject); }
        if (Keyboard.current.lKey.isPressed) { enemyLives -= 2; }
    }

    #region Funcion

    void ShootBullet()
    {
        GameObject newBullet = Instantiate(bullet, this.transform.position + new Vector3(0, 0, 1), this.transform.rotation);
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = this.transform.forward * bulletSpeed;
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            ShootBullet();
            yield return new WaitForSeconds(fireRate);
        }
    }

    #endregion

    #region Navmesh
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
    #endregion

    #region  Collision & Trigger
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bullet")
        {
            enemyLives -= Random.Range(1, 4);
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
    #endregion
}