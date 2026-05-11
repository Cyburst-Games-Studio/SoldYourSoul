using UnityEngine;
using System.Collections;

public class WeaponProjectileBehavior : MonoBehaviour
{
    [Header("Basic Elements")]
    [SerializeField]    private string      targetTag;
    [SerializeField]    private float       movementSpeed;
    [SerializeField]    private float       projectileLifetime;
    [Tooltip("Projectile will not disappear until its lifetime counter is done"),SerializeField]    
                        private bool        persists;

    [Header("Projectile Seeking"), Tooltip("Projectile aims toward nearest Point of Interest")]
    [SerializeField]    private bool seeking;
    [SerializeField]    private float seekingTurnSpeed;

    [Header("Summonable"), Tooltip("Spawns an object upon contact")]
    [SerializeField] bool       summons;
    [SerializeField] GameObject summonedObject;


    [HideInInspector]   
    public int damage;
    [HideInInspector]   
    public Vector2 dir;
    private GameObject[] targets;
    private Vector3 closestTarget;
    private float closestDistance;
    private Rigidbody2D rb;

    private void Start()
    {
        // gather rigidbody and set the speed
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * movementSpeed;

        // start the lifetime timer for the projectile
        StartCoroutine(BeginLifeTime(projectileLifetime));

        if (seeking)
        {
            targets = GameObject.FindGameObjectsWithTag(targetTag);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (seeking && targets.Length > 0)
        {
            float dist = Mathf.Infinity;
            foreach (GameObject g in targets)
            {
                if (g != null)
                {
                    dist = Vector2.Distance(transform.position, g.transform.position);

                    if (dist > closestDistance * 0.8)
                    {
                        closestTarget = g.transform.position;
                        closestDistance = dist;
                    }
                }
            }
            Vector2 direction = closestTarget - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle-90);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, seekingTurnSpeed);

            rb.linearVelocity = transform.up * movementSpeed;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the projectile should persist, early return
        if (persists) return;
        // continue if the object is of the target tag or the Ground layer
        if (collision.gameObject.tag != targetTag && collision.gameObject.layer != 3) return;

        StopCoroutine("BeginLifeTime");

        // if the object spawns another object, spawn that object
        if (summons)
        {
            GameObject proj = Instantiate(summonedObject, transform.position, Quaternion.identity);
            proj.GetComponent<WeaponProjectileBehavior>().damage = damage;
        }

        // destroy the object
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if the projectile should persist, early return
        if (persists) return;
        // check both the tag and layer of the collided object
        // continue if the object is of the target tag or the Ground layer
        if (collision.gameObject.tag != targetTag && collision.gameObject.layer != 3) return;

        StopCoroutine("BeginLifeTime");

        // if the object spawns another object, spawn that object
        if (summons)
        {
            GameObject proj = Instantiate(summonedObject, transform.position, Quaternion.identity);
            proj.GetComponent<WeaponProjectileBehavior>().damage = damage;
        }

        // destroy the object
        Destroy(this.gameObject);
    }

    private IEnumerator BeginLifeTime(float timer)
    {
        yield return new WaitForSeconds(timer);

        Destroy(gameObject);
    }
}
