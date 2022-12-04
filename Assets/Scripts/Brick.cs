using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] GameObject[] stateObjects;
    private int currentState;

    public void Init(float noiseValue)
    {
        float scaledValue = noiseValue * stateObjects.Length;
        for (int i = 0; i < stateObjects.Length; i++)
        {
            if (scaledValue < 1)
            {
                ChangeState(i);
                return;
            }
            scaledValue -= 1;
        }
    }

    public void LoadFromData(DataManager.BrickData data)
    {
        ChangeState(data.state);
    }

    public DataManager.BrickData CovertToData()
    {
        return new DataManager.BrickData(transform.position, currentState);
    }

    private void ChangeState(int state)
    {
        currentState = state;
        foreach(GameObject obj in stateObjects)
        {
            obj.SetActive(false);
        }
        stateObjects[state].SetActive(true);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.tag == "Paddle")
        {
            GameController.Instance.EndGame();
        }
        else
        {
            GameController.Instance.CurrentScore += GameController.Instance.NewestRow;
            AudioController.Instance.PlayRandomDestroySound();
            if (currentState > 0)
            {
                ChangeState(currentState - 1);
            }
            else
            {
                GameController.Instance.DestroyBrick(this);
            }
        }
	}
}
