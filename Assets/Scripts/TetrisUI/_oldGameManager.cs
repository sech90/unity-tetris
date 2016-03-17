using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class _oldGameManager : MonoBehaviour {

	public Text GameArea;
	public Text Next;
	public Text Score;
	public Tetris tetris;
	public Button StartButton;
	public Button Pause;
	public Button Right;
	public Button Left;
	public Button Rotate;
	public InputField H;
	public InputField W;
	public GameObject MenuPanel;
	public Slider Level;

	private Polymino _current;
	private bool _started = false;
	private bool paused = false;
	private bool[,] _next;

	void Start () {

		StartButton.onClick.AddListener(StartGame);
		Pause.onClick.AddListener(TogglePause);

		Right.onClick.AddListener(GoRight);
		Left.onClick.AddListener(GoLeft);
		Rotate.onClick.AddListener(DoRotate);

		tetris.ScoreUpdated += (int score) => {Score.text = score.ToString();};
		tetris.PieceUpdated += UpdateArea;
		tetris.PieceLanded += (clearedRows, points) => {
			Debug.Log("Yay! Cleared "+clearedRows.Length+" and got +"+points+" points!");
			UpdateArea(_current);
		};
		tetris.PieceSpawned += (current, next) => {
			_current = current;
			PrintNext(next);
		};

		tetris.GameOver += () => {
			_started = false;
			Debug.Log("GameOver...");
		};
	}

	void Update(){
		if(_started && !paused){
			if(Input.GetKeyDown(KeyCode.LeftArrow))
				GoLeft();
			if(Input.GetKeyDown(KeyCode.RightArrow))
				GoRight();
			if(Input.GetKeyDown(KeyCode.UpArrow))
				DoRotate();
			if(Input.GetKeyDown(KeyCode.DownArrow))
				StartDrop();
			if(Input.GetKeyUp(KeyCode.DownArrow))
				StopDrop();
			if(Input.GetKeyDown(KeyCode.P))
				TogglePause();
		}
	}

	void UpdateArea (Polymino current){
		bool[,] grid = (bool[,])tetris.Grid._grid.Clone();

		foreach(Cell c in current.Body)
			grid[c.y,c.x] = true;

		GameArea.text = "";

		for(int i=0; i<tetris.Grid.Height; i++){
			for(int j=0; j<tetris.Grid.Width; j++){
				GameArea.text += grid[i,j] ? "1" : "0";
			}
			GameArea.text += "\n";
		}
	}

	private void PrintNext(Polymino next){
		_next = new bool[4,4];
		foreach(Cell c in next.Body)
			_next[c.y, c.x] = true;

		Next.text = "";

		for(int i=0; i<4; i++){
			for(int j=0; j<4; j++){
				Next.text += _next[i,j] ? "1" : "0";
			}
			Next.text += "\n";
		}
	}

	private void StartGame(){
		int w = int.Parse(W.text);
		int h = int.Parse(H.text);
		int l = (int)Level.value;
		MenuPanel.SetActive(false);

		tetris.Initialize(w,h,l);
		tetris.StartGame();
		_started = true;
	}

	public void GoRight(){
		tetris.Right();
	}

	public void GoLeft(){
		tetris.Left();

	}
	public void DoRotate(){
		tetris.Rotate();
	}

	public void StartDrop(){
		tetris.SetFastDrop(true);
	}
	public void StopDrop(){
		tetris.SetFastDrop(false);
	}

	public void TogglePause(){
		if(paused)
			tetris.Resume();
		else
			tetris.Pause();
		paused = !paused;

	}
}
