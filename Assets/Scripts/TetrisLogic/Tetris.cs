using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Facade to external user. Dispatches events and exposes APIs to interface with the game 
/// </summary>
public class Tetris : MonoBehaviour, ITetris {

	public event SpawnHandler 		PieceSpawned;
	public event UpdateHandler 		PieceUpdated;
	public event PieceLandedHandler PieceLanded;
	public event ScoreUpdateHandler ScoreUpdated;
	public event GameOverHandler 	GameOver;

	[SerializeField] float MaxInterval;
	[SerializeField] float MinInterval;
	[SerializeField] float DropInterval;

	private Grid _grid;
	public int Score {get; private set;}
	public int Level {
		get{return _level;}
		set{
			_level = Mathf.Clamp(value,1,10);
			_curInterval =  GetIntervalTime();
		}
	}

	private int _level = 1;
	private float _curInterval;
	private float _timeWhenPaused;
	private bool _gameOver = true;
	private bool _paused = false;


	private List<Polymino> _pieces; 
	private Coroutine _mainLoop;
	private Polymino _nextPiece;

	void Awake (){
		_pieces = LoadPolyminos();
	}

	public void Initialize (int width, int height, int level){ 
		_grid = new Grid(width, height);
		Level = level;
	}

	public void StartGame (){
		_gameOver = false;
		_nextPiece = GetRandomPiece();
		_mainLoop = StartCoroutine(Loop());
	}

	public void Stop (){
		StopCoroutine(_mainLoop);
		_gameOver = true;
		GameOver();
	}

	public void Pause (){
		_paused = true;
		_timeWhenPaused = Time.time;
	}

	public void Resume (){
		_paused = false;
	}

	public bool Rotate (){
		//updating piece doesn't work after game over, while respawning or when operation is not possible
		if(_gameOver || _grid.CurrentPiece == null || !_grid.Rotate())
			return false;

		PieceUpdated(_grid.CurrentPiece);
		return true;
	}

	public bool Left (){
		//updating piece doesn't work after game over, while respawning or when operation is not possible
		if(_gameOver || _grid.CurrentPiece == null || !_grid.Left())
			return false;

		PieceUpdated(_grid.CurrentPiece);
		return true;
	}

	public bool Right (){
		//updating piece doesn't work after game over, while respawning or when operation is not possible
		if(_gameOver || _grid.CurrentPiece == null || !_grid.Right())
			return false;

		PieceUpdated(_grid.CurrentPiece);
		return true;
	}

	public void SetFastDrop (bool enable){

		//fast drop doesn't work after game over or while respawning
		if(_gameOver || _grid.CurrentPiece == null)
			return;

		StopCoroutine(_mainLoop);

		_curInterval = enable ? DropInterval : GetIntervalTime(); 
		_mainLoop = StartCoroutine(Loop());
	}

	private IEnumerator Loop(){

		while(!_gameOver){


			yield return new WaitForSeconds(_curInterval);

			//
			if(_paused){
				//record current time
				float currentTime = Time.time;

				//stay paused
				while(_paused)
					yield return new WaitForEndOfFrame();

				//recover the time that should have been spent in pause
				yield return new WaitForSeconds(currentTime - _timeWhenPaused);
			}

			//Grid has no active piece. Must spawn new piece
			if(_grid.CurrentPiece == null){
				
				bool ok = _grid.Spawn(_nextPiece);

				//piece spawned correctly
				if(ok){
					Polymino currentPiece = _nextPiece;
					_nextPiece = GetRandomPiece();
					PieceSpawned(currentPiece, _nextPiece);
				}

				//game over
				else{
					_gameOver = true;
					GameOver();
				}
			}

			//grid has active piece.
			else{

				//step the piece down
				List<int> clearedRows = _grid.Step();

				//piece still on mid-air
				if(clearedRows == null){
					PieceUpdated(_grid.CurrentPiece);

				}

				//piece is landed
				else{
					int points = CalculatePoints(clearedRows);
		
					PieceLanded(clearedRows.ToArray(), points);
					UpdateScore(points);

					//restore loop interval (in case of fast drops)
					_curInterval = GetIntervalTime();
				}
			}
		}
	}

	private void UpdateScore(int points){
		Score += points;
		ScoreUpdated(Score);
	}

	//Original Nintendo Scoring System
	private int CalculatePoints(List<int> clearedRows){
		if(clearedRows.Count == 0)
			return 0;
		
		int bonus = 40;
		if(clearedRows.Count == 2)
			bonus = 100;
		else if(clearedRows.Count == 3)
			bonus = 300;
		else if(clearedRows.Count == 4)
			bonus = 1200;
		
		return bonus * Level;
	}

	private Polymino GetRandomPiece(){
		int random = Random.Range(0, _pieces.Count); 
		return _pieces[random].Clone(); 
	}

	private float GetIntervalTime(){
		return Mathf.Lerp(MaxInterval, MinInterval, (float)(_level-1)/9.0f);
	}

	//this function could load the raw data from a file or some other source.
	//it's possible to create all kind of shapes in this way. Here are only the standard piece.
	private List<Polymino> LoadPolyminos(){
		List<Polymino> pieces = new List<Polymino>();

		pieces.Add(new Polymino(new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(1,0)},Polymino.PolyminoType.T));
		pieces.Add(new Polymino(new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(3,1)},Polymino.PolyminoType.I));
		pieces.Add(new Polymino(new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(2,0)},Polymino.PolyminoType.L));
		pieces.Add(new Polymino(new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(0,0)},Polymino.PolyminoType.J));
		pieces.Add(new Polymino(new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(1,0), new Cell(2,0)},Polymino.PolyminoType.S));
		pieces.Add(new Polymino(new Cell[]{new Cell(0,0), new Cell(1,0), new Cell(1,1), new Cell(2,1)},Polymino.PolyminoType.Z));
		pieces.Add(new Polymino(new Cell[]{new Cell(0,0), new Cell(1,0), new Cell(1,1), new Cell(0,1)},Polymino.PolyminoType.O));

		return pieces;
	}
}
