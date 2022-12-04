using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUps
    {
        Widen,
        Duplicate,
        Gun
    }

    private PowerUps heldPower;
    [SerializeField] private GameObject[] graphics;

    void Awake()
    {
        int randomPick = Random.Range(0, System.Enum.GetValues(typeof(PowerUps)).Length);
        heldPower = (PowerUps)randomPick;
        graphics[randomPick].SetActive(true);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.tag == "Paddle")
        {
            Activate(collision.GetComponent<Paddle>());
            GameController.Instance.DestroyPowerUp(this);
        }
        if (collision.gameObject.tag == "KillZone")
        {
            GameController.Instance.DestroyPowerUp(this);
        }
    }

    public void LoadFromData(DataManager.PowerData data)
    {
        heldPower = (PowerUps)data.type;
        graphics[data.type].SetActive(true);
    }

    public DataManager.PowerData CovertToData()
    {
        return new DataManager.PowerData(transform.position, (int)heldPower);
    }

    private void Activate(Paddle paddle)
    {
        AudioController.Instance.PlayPowerUpSound();
        if (heldPower == PowerUps.Widen)
        {
            paddle.Width += 3f;
        }
        else if (heldPower == PowerUps.Duplicate)
        {
            GameController.Instance.DuplicateBalls();
        }
        else if (heldPower == PowerUps.Gun)
        {
            paddle.GunTimer = 12;
        }
    }
}
