using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CanvasHelper : MonoBehaviour {

	[Header("UI references")]
	[SerializeField] GameObject MainMenu;
	[SerializeField] GameObject PauseMenu;
	[SerializeField] GameObject GameOverPanel;
	[SerializeField] Text ScoreText;
	[SerializeField] Text GameOverScoreText;
	[SerializeField] Text BestScoreText;
	[SerializeField] Text LevelText;
	[SerializeField] Text TimerText;
	[SerializeField] Slider LevelSlider;
	[SerializeField] InputField WidthField;
	[SerializeField] InputField HeightField;

	private int _latestScore = 0;

	void Start(){
		ShowMenu();
	}

	public void ShowMenu(){
		MainMenu.SetActive(true);
		PauseMenu.SetActive(false);
		GameOverPanel.SetActive(false);
		BestScoreText.text = PlayerPrefs.GetInt("HighScore",0).ToString();
	}

	public void ShowPause(){
		MainMenu.SetActive(false);
		PauseMenu.SetActive(true);
		GameOverPanel.SetActive(false);
	}

	public void ShowGame(){
		MainMenu.SetActive(false);
		PauseMenu.SetActive(false);
		GameOverPanel.SetActive(false);
		GameOverScoreText.text = _latestScore.ToString();
	}

	public void ShowGameOver(){
		MainMenu.SetActive(false);
		PauseMenu.SetActive(false);
		GameOverPanel.SetActive(true);
		GameOverScoreText.text = _latestScore.ToString();
	}

	public void ResetGameArea(){
		SetTimer(0,0);
		SetScore(0);
	}

	public int ReadWidth(){
		return int.Parse(WidthField.text); 
	}

	public int ReadHeight(){
		return int.Parse(HeightField.text); 
	}

	public int ReadLevel(){
		return (int)LevelSlider.value;
	}

	public void SetTimer(int min, int sec){
		TimerText.text = String.Format("{0:00}:{1:00}",min,sec);
	}

	public void SetScore(int score){
		_latestScore = score;
		ScoreText.text = score.ToString();
	}

	public void UpdateLevel(float value){
		LevelText.text = "Level: "+(int)value;
	}
}
