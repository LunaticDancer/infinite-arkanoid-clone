using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

	[SerializeField] GameObject MainMenu = null;
	[SerializeField] GameObject GameUI = null;
	[SerializeField] GameObject PauseMenu = null;
	[SerializeField] GameObject GameOverScreen = null;

	[SerializeField] GameObject ContinueButton = null;
	[SerializeField] GameObject SavedPopUp = null;

	[SerializeField] TMPro.TextMeshProUGUI currentScore = null;
	[SerializeField] TMPro.TextMeshProUGUI highestScore = null;

	private void Awake()
	{
		Instance = this;
	}

	#region MainMenu
	public void SetContinueButtonVisible(bool value)
	{
		ContinueButton.SetActive(value);
	}

	public void OnContinuePressed()
	{
		MainMenu.SetActive(false);
		GameUI.SetActive(true);
		GameController.Instance.LoadGame();
	}

	public void OnNewGamePressed()
	{
		MainMenu.SetActive(false);
		GameUI.SetActive(true);
		GameController.Instance.InitializeGameplay();
	}

	public void OnQuitPressed()
	{
		Application.Quit();
	}
	#endregion

	#region PauseMenu
	public void ShowPauseMenu()
	{
		PauseMenu.SetActive(true);
		GameUI.SetActive(false);
	}

	public void OnResumePressed()
	{
		GameController.Instance.UnpauseGame();
		PauseMenu.SetActive(false);
		GameUI.SetActive(true);
	}

	public void OnSavePressed()
	{
		GameController.Instance.SaveGame();
	}

	public void OnReturnToMenuPressed()
	{
		GameController.Instance.DeinitializeGameplay();
		PauseMenu.SetActive(false);
		MainMenu.SetActive(true); 
		DataManager.ProfileData profile = DataManager.Instance.LoadProfile();
		if (profile != null)
		{
			SetContinueButtonVisible(profile.ongoingGame);
		}
	}
	#endregion

	public void ShowGameOver()
	{
		GameOverScreen.SetActive(true);
		GameUI.SetActive(false);
	}
	public void ProceedGameOver()
	{
		GameOverScreen.SetActive(false);
		MainMenu.SetActive(true);
		SetContinueButtonVisible(false);
	}

	public void SetScore(int value)
	{
		currentScore.text = value.ToString();
	}

	public void SetHiScore(int value)
	{
		highestScore.text = "Highest Score: " + value.ToString();
	}

	private IEnumerator SavedPopUpAnimation(float timeVisible)
	{
		SavedPopUp.SetActive(true);
		yield return new WaitForSecondsRealtime(timeVisible);
		SavedPopUp.SetActive(false);

	}
}
