using UnityEngine;
using System.Collections;
using System;

public delegate void PieceLandedHandler(int[] clearedRows, int points);
public delegate void RowClearedHandler(int[] rows);
public delegate void SpawnHandler(Polymino current, Polymino next);
public delegate void UpdateHandler(Polymino current);
public delegate void ScoreUpdateHandler(int score);
public delegate void GameOverHandler();

public interface ITetris {
	event SpawnHandler 			PieceSpawned;
	event UpdateHandler			PieceUpdated;
	event PieceLandedHandler 	PieceLanded;
	event ScoreUpdateHandler 	ScoreUpdated;
	event GameOverHandler 		GameOver;

	int Score{get;}

	void Initialize(int width, int height, int level);
	void StartGame();
	void Stop();
	void Pause();
	void Resume();

	bool Rotate();
	bool Left();
	bool Right();
	void SetFastDrop(bool enable);


}
