using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Brick brickPrefab;
    [SerializeField] Vector2 generationSize;
    [SerializeField] Vector2 brickSize;

    public Brick BrickPrefab { get => brickPrefab; }

    public List<Brick> GenerateLevel(int index)
    {
        List<Brick> result = new List<Brick>();
        float perlinModifier = index * 0.234f;
        float perlinFlattener = 1f / (GameController.Instance.CurrentLevel); // makes multihit blocks rarer in early levels

        for (int y = 0; y < generationSize.y; y++)
        {
            for (int x = 0; x < generationSize.x; x++)
            {
                float perlinResult = Mathf.PerlinNoise(x * perlinModifier, y * perlinModifier) - perlinFlattener;
                result.Add(Instantiate(brickPrefab, transform.position + new Vector3(brickSize.x * x, -brickSize.y * y), Quaternion.identity, transform));
                result[result.Count-1].Init(perlinResult);
                if (x > 0)
                {
                    result.Add(Instantiate(brickPrefab, transform.position + new Vector3(-brickSize.x * x, -brickSize.y * y), Quaternion.identity, transform));
                    result[result.Count - 1].Init(perlinResult);
                }
            }
        }

        return result;
    }
}
