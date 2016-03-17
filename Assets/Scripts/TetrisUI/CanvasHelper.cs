using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Deals with menu, buttons and small UI related tasks
/// </summary>
public class CanvasHelper : MonoBehaviour {

	[Header("UI references")]
	[SerializeField] GameObject MainMenu;
	[SerializeField] GameObject PauseMenu;
	[SerializeField] GameObject GameOverPanel;
	[SerializeField] Text ErrorText;
	[SerializeField] Text ScoreText;
	[SerializeField] Text GameOverScoreText;
	[SerializeField] Text BestScoreText;
	[SerializeField] Text LevelText;
	[SerializeField] Text TimerText;
	[SerializeField] Slider LevelSlider;
	[SerializeField] InputField WidthField;
	[SerializeField] InputField HeightField;

	private int _latestScore = 0;
	private GameManager _manager;

	void Start(){
		_manager = GameObject.FindObjectOfType<GameManager>();
		ShowMenu();
	}

	public void ValidateInputAndStart(){
		if(ReadHeight() < 4 || ReadWidth() < 4){
			ErrorText.gameObject.SetActive(true);
			ErrorText.text = "Minimum grid size is 4x4";
		}
		else
			_manager.StartGame();
	}

	public void ShowMenu(){
		MainMenu.SetActive(true);
		PauseMenu.SetActive(false);
		GameOverPanel.SetActive(false);

		ErrorText.gameObject.SetActive(false);
		BestScoreText.text = PlayerPrefs.GetInt("HighScore",0).ToString();
		WidthField.text = PlayerPrefs.GetInt("Width",10).ToString();
		HeightField.text = PlayerPrefs.GetInt("Height",20).ToString();
		LevelSlider.value = PlayerPrefs.GetInt("Level",1);
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

	public void Reload(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ResetGameArea(){
		SetTimer(0,0);
		SetScore(0);
	}

	public int ReadWidth(){
		int w = int.Parse(WidthField.text); 
		PlayerPrefs.SetInt("Width",w);
		return w;
	}

	public int ReadHeight(){
		int h = int.Parse(HeightField.text); 
		PlayerPrefs.SetInt("Height",h);
		return h;
	}

	public int ReadLevel(){
		int level = (int)LevelSlider.value;
		PlayerPrefs.SetInt("Level",level);
		return level;
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
