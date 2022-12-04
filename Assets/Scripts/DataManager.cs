using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;     // does all the saving and loading

    [System.Serializable]
    public class BallData
    {
        public float xPosition;
        public float yPosition;
        public float xDirection;
        public float yDirection;
        public BallData(Vector3 position, Vector3 velocity)
        {
            xPosition = position.x;
            yPosition = position.y;
            xDirection = velocity.normalized.x;
            yDirection = velocity.normalized.y;
        }
    }

    [System.Serializable]
    public class BrickData
    {
        public float xPosition;
        public float yPosition;
        public int state;
        public BrickData(Vector3 position, int type)
        {
            xPosition = position.x;
            yPosition = position.y;
            state = type;
        }
    }

    [System.Serializable]
    public class PowerData
    {
        public float xPosition;
        public float yPosition;
        public int type;
        public PowerData(Vector3 position, int type)
        {
            xPosition = position.x;
            yPosition = position.y;
            this.type = type;
        }
    }

    [System.Serializable]
    public class GameStateData
    {
        public int newestRow;
        public int currentScore;
        public float paddleWidth;
        public float paddleGunTimer;
        public BallData[] balls;
        public BrickData[] bricks;
        public PowerData[] powers;
        public GameStateData(int row, int score, float width, float gunTimer, BallData[] balls, BrickData[] bricks, PowerData[] powers)
        {
            newestRow = row;
            currentScore = score;
            paddleWidth = width;
            paddleGunTimer = gunTimer;
            this.balls = balls;
            this.bricks = bricks;
            this.powers = powers;

        }
    }

    [System.Serializable]
    public class ProfileData
    {
        public int highestScore;
        public bool ongoingGame;
        public ProfileData(int hiScore, bool isGameOngoing)
        {
            highestScore = hiScore;
            ongoingGame = isGameOngoing;
        }
    }

	private void Awake()
	{
        Instance = this;
	}

	public ProfileData LoadProfile()
    {
        if (System.IO.File.Exists(Application.dataPath + "/Profile.json"))
        {
            string dataString = System.IO.File.ReadAllText(Application.dataPath + "/Profile.json");
            return JsonUtility.FromJson<ProfileData>(dataString);
        }
        return null;
    }

    public void SaveProfile(ProfileData data)
    {
        string jsonConversion = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.dataPath + "/Profile.json", jsonConversion);
    }

    public GameStateData LoadGameSession()
    {
        if (System.IO.File.Exists(Application.dataPath + "/CurrentGame.json"))
        {
            string dataString = System.IO.File.ReadAllText(Application.dataPath + "/CurrentGame.json");
            return JsonUtility.FromJson<GameStateData>(dataString);
        }
        return null;
    }

    public void SaveGameSession(GameStateData data)
    {
        string jsonConversion = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.dataPath + "/CurrentGame.json", jsonConversion);
    }
}
