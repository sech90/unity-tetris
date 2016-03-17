using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Main controller for the game. Controls the flow, the input and sets up events between logic ad UI
/// </summary>
public class GameManager : MonoBehaviour {

	[Header("Audio references")]
	[SerializeField] AudioClip MoveClip;
	[SerializeField] AudioClip RotateClip;
	[SerializeField] AudioClip LandClip;
	[SerializeField] AudioClip RowClearClip;
	[SerializeField] AudioClip GameOverClip;

	private ITetris _tetris;
	private UIGrid _grid;
	private UINextPiece _nextPiece;
	private AudioSource _audio;
	private CanvasHelper _canvas;
	private Coroutine _timerCoroutine;
	private int _highScore;
	private bool _isPlaying;
	private float _startTime, _elapsedTime;
	private bool _paused;

	void Awake () {
		_tetris 	= GetComponent<Tetris>();
		_audio		= GetComponent<AudioSource>();
		_grid 		= GameObject.FindObjectOfType<UIGrid>();
		_nextPiece 	= GameObject.FindObjectOfType<UINextPiece>();
		_canvas		= GameObject.FindObjectOfType<CanvasHelper>();
		_highScore  = PlayerPrefs.GetInt("HighScore",0);
	}

	//allows controlling the game with the keyboard
	#if UNITY_EDITOR
	void Update(){
		if(_isPlaying && !_paused){
			if(Input.GetKeyDown(KeyCode.LeftArrow))
				Left();
			if(Input.GetKeyDown(KeyCode.RightArrow))
				Right();
			if(Input.GetKeyDown(KeyCode.UpArrow))
				Rotate();
			if(Input.GetKeyDown(KeyCode.DownArrow))
				StartDrop();
			if(Input.GetKeyUp(KeyCode.DownArrow))
				StopDrop();
			if(Input.GetKeyDown(KeyCode.P))
				TogglePause();
		}
		else if(_paused && Input.GetKeyDown(KeyCode.P))
			TogglePause();
	}
	#endif

	public void StartGame(){
		InitComponents();
		RegisterListeners();

		_isPlaying = true;
		_startTime = Time.time;
		_timerCoroutine = StartCoroutine(StartTimer());

		_tetris.StartGame();
	}

	//called when there is a game over condition
	public void OnGameOver(){
		_audio.Stop();
		_audio.PlayOneShot(GameOverClip);
		_grid.GameOver();

		_isPlaying = false;
		StopCoroutine(_timerCoroutine);
		Invoke("QuitGame",1.5f);
	}

	//called after GameOver or when player decides to leave the game
	public void QuitGame(){

		Clean();
		UnRegisterListeners();

		//update high score
		if( _tetris.Score > _highScore){
			_highScore = _tetris.Score;
			PlayerPrefs.SetInt("HighScore",_highScore);
		}

		_canvas.ShowGameOver();
	}

	public void Left(){
		if(_tetris.Left())
			_audio.PlayOneShot(MoveClip);
	}

	public void Right(){
		if(_tetris.Right())
			_audio.PlayOneShot(MoveClip);
	}

	public void Rotate(){
		if(_tetris.Rotate())
			_audio.PlayOneShot(RotateClip);
	}

	public void StartDrop(){
		_tetris.SetFastDrop(true);
	}
	public void StopDrop(){
		_tetris.SetFastDrop(false);
	}

	public void TogglePause(){
		if(_paused){
			_audio.UnPause();
			_tetris.Resume();
			_canvas.ShowGame();
			_startTime = Time.time - _elapsedTime;
			_timerCoroutine = StartCoroutine(StartTimer());
		}
		else{
			_audio.Pause();
			_tetris.Pause();
			_canvas.ShowPause();
			_elapsedTime = Time.time - _startTime;
			StopCoroutine(_timerCoroutine);
		}

		_paused = !_paused;
	}
		
	private void InitComponents(){
		int w = _canvas.ReadWidth();
		int h = _canvas.ReadHeight();
		int l = _canvas.ReadLevel();

		_grid.Clear();
		_nextPiece.Clear();

		_grid.Init(w,h);
		_tetris.Initialize(w,h,l);
		_canvas.ShowGame();
		_audio.time = 0;
		_audio.Play();
	}

	private void OnSpawn(Polymino current, Polymino next){
		_nextPiece.ShowPolymino(next);
		_grid.Spawn(current);
	}

	private void OnScore(int score){
		_canvas.SetScore(score);
	}

	private void OnLand(int[] rows, int score){
		_grid.Land(rows);

		if(rows.Length > 0)
			_audio.PlayOneShot(RowClearClip);
		else
			_audio.PlayOneShot(LandClip);

		/*TODO: show how many points you got with the last move*/
	}

	private void ShowEndScreen(){
		_canvas.ShowGameOver();
	}

	private IEnumerator StartTimer(){
		int min,sec;
		while(_isPlaying){
			
			_elapsedTime = (Time.time - _startTime);
			min = (int)_elapsedTime / 60;
			sec = (int)_elapsedTime % 60;

			_canvas.SetTimer(min,sec);
			yield return new WaitForSeconds(1);
		}
	}

	private void RegisterListeners(){
		_tetris.GameOver += OnGameOver;
		_tetris.PieceSpawned += OnSpawn;
		_tetris.PieceLanded += OnLand;
		_tetris.PieceUpdated += _grid.UpdateCurrent;
		_tetris.ScoreUpdated += OnScore;
	}

	private void UnRegisterListeners(){
		_tetris.GameOver -= OnGameOver;
		_tetris.PieceSpawned -= OnSpawn;
		_tetris.PieceLanded -= OnLand;
		_tetris.PieceUpdated -= _grid.UpdateCurrent;
		_tetris.ScoreUpdated -= OnScore;
	}
		
	private void Clean(){
		_nextPiece.Clear();

		_isPlaying = false;
		_paused = false;
		_startTime = 0;
		_elapsedTime = 0;
	}
}
