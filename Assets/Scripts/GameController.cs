using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	private const int MAX_BALLS_COUNT = 42;
	private const float BRICK_SCROLL_SPEED_MIN = 0.04f;
	private const float BRICK_SCROLL_SPEED_MAX = 1f;
	private const int BRICK_SCROLL_SPEED_THRESHOLD = 100;

	public enum GameStates
	{
		MainMenu,
		ActiveGameplay,
		PausedGameplay,
		GameOver
	}

	[SerializeField] GameObject gameAreaCore = null;
	[SerializeField] Ball ballPrefab = null;
	[SerializeField] PowerUp powerUpPrefab = null;
	[SerializeField] Paddle paddle = null;
	[SerializeField] LevelGenerator levelGenerator = null;
	private GameStates currentState = GameStates.MainMenu;
	private int currentScore = 0;
	private int highestScore = 0;

	private int newestRow = 1;

	// this section could be improved with pooling
	List<Ball> balls = null;
	List<Brick> bricks = null;
	List<PowerUp> powerUps = null;
	List<Projectile> projectiles = null;

	public LevelGenerator LevelGenerator { get => levelGenerator; }
	public int CurrentScore { get => currentScore; set
		{
			currentScore = value;
			UIController.Instance.SetScore(currentScore);
		}
	}
	public int NewestRow { get => newestRow; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		projectiles = new List<Projectile>();
		DataManager.ProfileData profile = DataManager.Instance.LoadProfile();
		if (profile != null)
		{
			highestScore = profile.highestScore;
			UIController.Instance.SetContinueButtonVisible(profile.ongoingGame);
		}
	}

	private void Update()
	{
		if (currentState == GameStates.GameOver)
		{
			if (Input.GetMouseButtonDown(0))
			{
				DeinitializeGameplay();
			}
		}
		else if (currentState == GameStates.ActiveGameplay)
		{
			HandleScrollingBricks();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (currentState == GameStates.PausedGameplay)
			{
				UIController.Instance.OnResumePressed();
			}
			else if (currentState == GameStates.ActiveGameplay)
			{
				UIController.Instance.ShowPauseMenu();
				PauseGame();
			}
		}
	}

	private void HandleScrollingBricks()
	{
		levelGenerator.transform.position += Vector3.down * Mathf.Lerp(BRICK_SCROLL_SPEED_MIN, BRICK_SCROLL_SPEED_MAX, (float)newestRow/BRICK_SCROLL_SPEED_THRESHOLD) * Time.deltaTime;
		while (levelGenerator.transform.position.y + newestRow * levelGenerator.BrickSize.y < 6)
		{
			bricks.AddRange(levelGenerator.GenerateRow(newestRow));
			newestRow++;
		}
	}

	public void InitializeGameplay()
	{
		newestRow = 0;
		currentState = GameStates.ActiveGameplay;
		gameAreaCore.SetActive(true);
		levelGenerator.transform.position = new Vector3(0, 3.5f);
		bricks = new List<Brick>();
		paddle.Width = 2;
		paddle.GunTimer = 0;
		CurrentScore = 0;
		UIController.Instance.SetHiScore(highestScore);
		powerUps = new List<PowerUp>();
		balls = new List<Ball>();
		InitBall();
	}

	public void LoadGame()
	{
		DataManager.GameStateData loadedData = DataManager.Instance.LoadGameSession();
		if (loadedData != null)
		{
			newestRow = loadedData.newestRow;
			currentState = GameStates.ActiveGameplay;
			gameAreaCore.SetActive(true);
			UIController.Instance.SetHiScore(highestScore);
			paddle.Width = loadedData.paddleWidth;
			paddle.GunTimer = loadedData.paddleGunTimer;
			CurrentScore = loadedData.currentScore;

			powerUps = new List<PowerUp>();
			balls = new List<Ball>();
			bricks = new List<Brick>();

			foreach (DataManager.PowerData power in loadedData.powers)
			{
				PowerUp powerUp = Instantiate(powerUpPrefab, new Vector3(power.xPosition, power.yPosition), Quaternion.identity);
				powerUps.Add(powerUp);
				powerUp.LoadFromData(power);
			}

			foreach (DataManager.BallData ball in loadedData.balls)
			{
				Ball ballObj = Instantiate(ballPrefab, new Vector3(ball.xPosition, ball.yPosition), Quaternion.identity, gameAreaCore.transform);
				balls.Add(ballObj);
				ballObj.BypassInit(paddle);
				ballObj.LoadFromData(ball);
			}

			foreach (DataManager.BrickData brick in loadedData.bricks)
			{
				Brick brickObj = Instantiate(levelGenerator.BrickPrefab, new Vector3(brick.xPosition, brick.yPosition), Quaternion.identity, levelGenerator.transform);
				bricks.Add(brickObj);
				brickObj.LoadFromData(brick);
			}
		}
		UnpauseGame();
	}

	public void SaveGame()
	{
		DataManager.BallData[] ballsData = new DataManager.BallData[balls.Count];
		DataManager.BrickData[] bricksData = new DataManager.BrickData[bricks.Count];
		DataManager.PowerData[] powersData = new DataManager.PowerData[powerUps.Count];

		for (int i = 0; i < balls.Count; i++)
		{
			ballsData[i] = balls[i].CovertToData();
		}
		for (int i = 0; i < bricks.Count; i++)
		{
			bricksData[i] = bricks[i].CovertToData();
		}
		for (int i = 0; i < powerUps.Count; i++)
		{
			powersData[i] = powerUps[i].CovertToData();
		}

		DataManager.GameStateData saveData = new DataManager.GameStateData(newestRow, currentScore, paddle.Width, paddle.GunTimer, 
			ballsData, bricksData, powersData);
		DataManager.Instance.SaveGameSession(saveData);
		DataManager.ProfileData data = new DataManager.ProfileData(highestScore, true);
		DataManager.Instance.SaveProfile(data);
	}

	private void InitBall()
	{
		balls.Add(Instantiate(ballPrefab, paddle.transform.position + Vector3.up, Quaternion.identity, paddle.transform.parent));
		balls[0].InitBall(paddle);
	}

	public void PauseGame()
	{
		currentState = GameStates.PausedGameplay;
		Time.timeScale = 0;
	}

	public void UnpauseGame()
	{
		currentState = GameStates.ActiveGameplay;
		Time.timeScale = 1;
	}

	public void DestroyBrick(Brick target)
	{
		if (Random.Range(0, 1f) < Mathf.Lerp(.12f, .06f, newestRow / 50f))
		{
			powerUps.Add(Instantiate(powerUpPrefab, target.transform.position, Quaternion.identity));
		}
		bricks.Remove(target);
		Destroy(target.gameObject);
	}

	public void DestroyBall(Ball target)
	{
		balls.Remove(target);
		Destroy(target.gameObject);
		if (balls.Count < 1)
		{
			EndGame();
		}

	}

	private void EndGame()
	{
		UIController.Instance.ShowGameOver();
		currentState = GameStates.GameOver;
		ClearPowerUps();
		paddle.GunTimer = 0;
		if (currentScore > highestScore)
		{
			highestScore = currentScore;
		}
		DataManager.ProfileData data = new DataManager.ProfileData(highestScore, false);
		DataManager.Instance.SaveProfile(data);
	}

	public void DuplicateBalls()
	{
		int currentCount = balls.Count;
		for (int i = 0; i < currentCount; i++)
		{
			if (balls.Count < MAX_BALLS_COUNT)
			{
				balls[i].Duplicate();
			}
		}
	}

	public void AddBall(Ball ball)
	{
		balls.Add(ball);
	}

	public void DestroyPowerUp(PowerUp target)
	{
		powerUps.Remove(target);
		Destroy(target.gameObject);
	}

	public void ClearPowerUps()
	{
		foreach (PowerUp power in powerUps)
		{
			Destroy(power.gameObject);
		}
		powerUps = new List<PowerUp>();
	}

	public void AddProjectile(Projectile thing)
	{
		projectiles.Add(thing);
	}

	public void RemoveProjectile(Projectile thing)
	{
		projectiles.Remove(thing);
	}

	public void ClearProjectiles()
	{
		foreach (Projectile proj in projectiles)
		{
			Destroy(proj.gameObject);
		}
		projectiles = new List<Projectile>();
	}


	public void DeinitializeGameplay()
	{
		currentState = GameStates.MainMenu;
		foreach (Brick brick in bricks)
		{
			Destroy(brick.gameObject);
		}
		foreach (Ball ball in balls)
		{
			Destroy(ball.gameObject);
		}
		ClearPowerUps();
		ClearProjectiles();
		gameAreaCore.SetActive(false);
		UIController.Instance.ProceedGameOver();
	}
}
