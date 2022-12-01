using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    public void Init(float speed)
    {
        rb.velocity = Vector2.up * speed;
        GameController.Instance.AddProjectile(this);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.tag == "SolidWall" || collision.gameObject.tag == "Brick")
        {
            GameController.Instance.RemoveProjectile(this);
            Destroy(gameObject);
        }

    }
}
