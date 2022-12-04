using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float LERP_SPEED = 12;

    private Rigidbody2D rb;
    private Paddle paddle;
    private bool isInMotion = false;
    [SerializeField] float speed;
    [SerializeField] Vector3 paddleOffset;

    private void Update()
	{
        if (!isInMotion)
        {
            transform.position = Vector3.Lerp(transform.position, paddle.transform.position + paddleOffset, LERP_SPEED * Time.deltaTime);
            if (Input.GetMouseButtonDown(0))
            {
                rb.isKinematic = false;
                Bounce();
                isInMotion = true;
            }
        }
	}

	public void InitBall(Paddle paddle)
    {
        rb = GetComponent<Rigidbody2D>();
        this.paddle = paddle;
    }

    public void BypassInit(Paddle paddle)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        this.paddle = paddle;
        isInMotion = true;
    }

    public void LoadFromData(DataManager.BallData data)
    {
        Reroute(new Vector2(data.xDirection, data.yDirection));
    }

    public DataManager.BallData CovertToData()
    {
        return new DataManager.BallData(transform.position, rb.velocity);
    }

    private void Bounce()
    {
        rb.velocity = paddle.GetBounceDirection(transform.position) * speed * Mathf.Lerp(1, 2, GameController.Instance.NewestRow/50f);
    }

    public void Duplicate()
    {
        Vector2 direction = rb.velocity.normalized;
        Ball clone = Instantiate(this, transform.position, Quaternion.identity, transform.parent);
        clone.BypassInit(paddle);
        GameController.Instance.AddBall(clone);
        Reroute(-Vector2.Perpendicular(direction));
        clone.Reroute(Vector2.Perpendicular(direction));
    }

    public void Reroute(Vector2 direction)
    {
        rb.velocity = direction * speed * Mathf.Lerp(1, 2, GameController.Instance.NewestRow / 50f);
    }

	private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Paddle")
        {
            Bounce();
        }
        if (collision.gameObject.tag == "KillZone")
        {
            GameController.Instance.DestroyBall(this);
        }
    }
}
