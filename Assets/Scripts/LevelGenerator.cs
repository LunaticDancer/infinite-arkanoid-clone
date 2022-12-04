using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Brick brickPrefab;
    [SerializeField] Vector2 generationSize;
    [SerializeField] Vector2 brickSize;

    public Vector2 BrickSize { get => brickSize; }

    public Brick BrickPrefab { get => brickPrefab; }

    public List<Brick> GenerateRow(int rowIndex)
    {
        List<Brick> result = new List<Brick>();
        float perlinModifier = 0.234f;
        float perlinFlattener = 1f / (rowIndex/5f); // makes multihit blocks rarer in early game

        for (int x = 0; x < generationSize.x; x++)
        {
            float perlinResult = Mathf.PerlinNoise(x * perlinModifier, rowIndex * perlinModifier) - perlinFlattener;
            result.Add(Instantiate(brickPrefab, transform.position + new Vector3(brickSize.x * x, brickSize.y * rowIndex), Quaternion.identity, transform));
            result[result.Count - 1].Init(perlinResult);
            if (x > 0)
            {
                result.Add(Instantiate(brickPrefab, transform.position + new Vector3(-brickSize.x * x, brickSize.y * rowIndex), Quaternion.identity, transform));
                result[result.Count - 1].Init(perlinResult);
            }
        }

        return result;
    }
}
