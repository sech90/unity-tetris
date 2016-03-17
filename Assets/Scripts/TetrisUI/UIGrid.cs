using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Tetris Grid. Can adapt to all grid sizes and adjust the bricks scale according to the parent dimensions.
/// </summary>
public class UIGrid : MonoBehaviour {

	[SerializeField] float BrickFallSpeed = 10.0f;

	private Bounds _bounds;
	private Vector3 _scale;
	private Vector3 _topLeft;
	private Transform _container;
	private float _unitSize;
	private float _width;

	private GameObject _currentBricks;
	private Polymino _currentPiece;
	private Dictionary<Cell, UIBrick> _landed;

	void Start(){
		_landed = new Dictionary<Cell, UIBrick>();
		_bounds = GetComponent<Renderer>().bounds;

		//create a container for the bricks
		_container = new GameObject("BricksContainer").transform;
		_container.parent = transform;
		_container.localPosition = Vector3.zero;
	} 

	public void Init(int width, int height){

		_width = width;

		//what's the bricks' size?
		_unitSize = UIUtils.CalculateGridUnitSize(_bounds,width,height);

		//cache the correct scale for the bricks to be visualized
		_scale = new Vector3(_unitSize, _unitSize, _unitSize);

		//calculate the viewport corners considering the given grid dimensions.
		//the center will be on the middle bottom point
		float bottom 	= _bounds.center.y - _bounds.extents.y; 
		float top 		= _unitSize * height + bottom;
		float left 		= _bounds.center.x - (_unitSize * width/2.0f);
		//float right 	= _bounds.center.x + (_unitSize * width/2.0f);

		//place one unit in front of the parent (z)
		_topLeft = new Vector3(left,top,transform.position.z-_unitSize);
	}
		
	public void Spawn(Polymino newPiece){

		//create new brick
		_currentBricks = UIUtils.MakePolymino(newPiece);

		//place it into the grid
		_currentBricks.transform.localScale = _scale;
		_currentBricks.transform.parent = _container;

		//move to correct position
		UpdateCurrent(newPiece);
	}

	//update the position of the falling bricks
	public void UpdateCurrent(Polymino piece){
		Vector3 pos = _topLeft;
		pos.x += piece.Col * _unitSize;
		pos.y -= piece.Row * _unitSize;

		_currentPiece = piece;
		_currentBricks.transform.position = pos;
		UIUtils.RearrangeBricks(_currentBricks,piece);
	}

	public void Land(int[] clearedRows){

		//add current polymino to the landed bricks list
		MapBriks(_currentBricks.transform);

		//no blocks to move
		if(clearedRows.Length == 0)
			return;
		
		//keeps track of how many times a given brick should be moved down
		int jump = 0;

		//iterate through the cleared rows and make the bricks explode
		//start from bottommost full row, then goes up to the top
		for(int row = clearedRows[0]; row >= 0; row--){

			//we're on a full row. Clear the row
			if(Array.Exists<int>(clearedRows, r => r.Equals(row))){
				ExplodeRow(row);
				jump++;
			}

			//we're on a row with some blocks. move the row down
			else 
				MoveRow(row, jump);
		}
	}

	//like it says
	private void ExplodeRow(int y){
		Cell cell = new Cell(0,y);
		for(int i=0; i<_width; i++){
			cell.x = i;
			_landed[cell].Explode();
			_landed.Remove(cell);
		}
	}

	//TODO: find better implementation
	private void MoveRow(int y, int jump){
		Cell indexCell = new Cell(0,y);
		UIBrick brick;

		for(int i=0; i<_width; i++){
			indexCell.x = i;

			//there is one brick to move
			if(_landed.TryGetValue(indexCell, out brick)){

				Cell newIndex = indexCell;
				newIndex.y += jump;

				//change its current key to the new position
				_landed.Remove(indexCell);
				_landed.Add(newIndex,brick);

				//let it fall down
				brick.FallDownBy(jump, BrickFallSpeed);
			}
		}
	}

	//At Gameover, turn all bricks to gray
	public void GameOver(){
		foreach(var kv in _landed){
			kv.Value.GreyOut();
		}
	}
		
	//landed bricks must be mapped so we can access them later
	private void MapBriks(Transform briksParent){
		Transform brick;

		//for all the bricks in the polymino
		for(int i=0; i<briksParent.childCount; i++){
			brick = briksParent.GetChild(i);

			//get the cell where they belong
			Cell c = UIUtils.LocalPositionToCell(brick.localPosition);
			c.x += _currentPiece.Col;
			c.y += _currentPiece.Row;

			//add to the map
			_landed.Add(c, brick.GetComponent<UIBrick>());
		}
	}

	//clean up the grid
	public void Clear(){
		
		_landed.Clear();
		_currentPiece = null;

		Destroy(_currentBricks);

		//kills all children bricks
		for(int i=0; i<_container.childCount; i++)
			Destroy(_container.GetChild(i).gameObject);
	}
}
