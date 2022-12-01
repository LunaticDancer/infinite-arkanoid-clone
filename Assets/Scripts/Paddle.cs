using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    private const float MOVE_LERP_SPEED = 0.8f;
    private const float MAX_PADDLE_SIZE = 15f;
    private const float GUN_COOLDOWN = 0.5f;

    private float gunTimer;         // how long the power up lasts
    private float gunCooldown;      // time between shots
    [SerializeField] Projectile projectilePrefab;
    private float width;
    [SerializeField] private float reachLimit;
    private float calculatedReachLimit = 7.5f;
    [SerializeField] private Transform directionInfluencer = null;      // influences direction of the ball bounce
    private Rigidbody2D rb = null;

    public float Width
    {
        get => width; set
        {
            width = value;
            if (width > MAX_PADDLE_SIZE)
            {
                width = MAX_PADDLE_SIZE;
            }
            transform.localScale = new Vector3(width, 0.25f);
            calculatedReachLimit = reachLimit - (width / 2);
        }
    }
    public float GunTimer
    {
        get => gunTimer; set
        {
            if (value == 0)
            {
                gunTimer = 0;
            }
            else
            {
                if (gunTimer < 0)
                {
                    gunTimer = value;
                }
                else
                {
                    gunTimer += value;
                }
            }
        }
    }
    public Transform DirectionInfluencer { get => directionInfluencer; }

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
	}

	void Update()
    {
        Move();
        gunTimer -= Time.deltaTime;
        if (gunTimer > 0)
        {
            gunCooldown -= Time.deltaTime;
            if (gunCooldown < 0)
            {
                Shoot();
                gunCooldown += GUN_COOLDOWN;
            }
        }
    }

    private void Move()
    {
        Vector3 targetPosition = Vector3.Lerp(transform.position, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
            transform.position.y), MOVE_LERP_SPEED);
        // there is no Vector3 clamp, so I have to do this the barbaric way
        if (targetPosition.x > calculatedReachLimit)
        {
            targetPosition = new Vector3(calculatedReachLimit, transform.position.y);
        }
        if (targetPosition.x < -calculatedReachLimit)
        {
            targetPosition = new Vector3(-calculatedReachLimit, transform.position.y);
        }

        rb.MovePosition(targetPosition); // using Rigidbody.Move() instead of Transform.position for more accurate physics interactions
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, transform.position + Vector3.up * 0.3f + Vector3.left * width/2, Quaternion.identity).Init(8);
        Instantiate(projectilePrefab, transform.position + Vector3.up * 0.3f + Vector3.right * width/2, Quaternion.identity).Init(8);
    }

    public Vector3 GetBounceDirection(Vector3 position)
    {
        return (position - directionInfluencer.position).normalized;
    }
}
