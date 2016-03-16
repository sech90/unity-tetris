using UnityEngine;
using System.Collections.Generic;

public class Grid {

	public int Width{get;private set;}
	public int Height{get;private set;}

	public bool[,] _grid;
	private int[] _fullCellCount;
	public Polymino CurrentPiece{get;private set;}

	public Grid(int width, int height){

		Width = width;
		Height = height;
		_grid = new bool[Height,Width]; 
		_fullCellCount = new int[height];
	}

	public bool Spawn(Polymino t){
		CurrentPiece = t;
		CurrentPiece.Row = 0;

		//polyminos with even boundsize spawn in the middle column. The others in the left-middle
		CurrentPiece.Col = Mathf.CeilToInt((float)Width/2.0f)-Mathf.CeilToInt((float)CurrentPiece.BoundSize/2.0f);

		//polymino is overlapping with some other block. Game over.
		if(Overlaps(CurrentPiece.Body))
			return false;

		return true;
	}

	public List<int> Step(){

		CurrentPiece.Row += 1;

		//polymino reached bottom of the grid or landed on top of another block
		if(CurrentPiece.BottomMost.y >= Height || Overlaps(CurrentPiece.GetLowerBounds())){
			CurrentPiece.Row -= 1;

			List<int> fullRows = AddPieceToBlocks(CurrentPiece);

			//we filled some rows! Yeah!
			if(fullRows.Count > 0)
				CompactGrid(fullRows);

			//piece became blocks!
			CurrentPiece = null;
			return fullRows;	
		}
		return null;
	}

	public bool Right(){

		//can't go outside grid
		if(CurrentPiece.RightMost.x + 1 == Width)
			return false;

		//Check for overlapping blocks
		Cell[] bounds = CurrentPiece.GetRightBounds();
		for(int i=0; i<bounds.Length; i++){
			bounds[i].x += 1;

			//can't overlap other blocks
			if(Contains(bounds[i]))
				return false;
		}

		//Update piece position
		CurrentPiece.Col += 1;
		return true;
	}

	public bool Left(){
		//can't go outside grid
		if(CurrentPiece.LeftMost.x == 0)
			return false;

		//Check for overlapping blocks
		Cell[] bounds = CurrentPiece.GetLeftBounds();
		for(int i=0; i<bounds.Length; i++){
			bounds[i].x -= 1;

			//can't overlap other blocks
			if(Contains(bounds[i]))
				return false;
		}

		//Update piece position
		CurrentPiece.Col -= 1;
		return true;
	}

	public bool Rotate(){
		Polymino rot = CurrentPiece.Rotated;
		int halfSize = Mathf.FloorToInt((float)rot.BoundSize/2.0f);

		//check bottom
		if(rot.BottomMost.y >= Height)
			return false;

		//check right
		if(rot.RightMost.x >= Width){

			//try wall kick. 
			if(rot.RightMost.x - Width > halfSize)
				return false;

			//left-kick the minimum amount to keep the polymino in the grid
			rot.Col -= (rot.RightMost.x - Width)+1;
		}

		//check left
		if(rot.LeftMost.x < 0){

			//try wall kick. 
			if(-rot.LeftMost.x > halfSize)
				return false;

			//right-kick the minimum amount to keep the polymino in the grid
			rot.Col += -rot.LeftMost.x;
		}

		//re-check right bound in case that the left wall kick moved the polymino out of the grid
		if(rot.RightMost.x >= Width)
			return false;
		
		//check if it overlaps other blocks
		if(Overlaps(rot.Body))
			return false;

		CurrentPiece = rot;
		return true;
	}

	//O(n) where n is the polymino's body length
	private List<int> AddPieceToBlocks(Polymino piece){
		
		Cell[] body = piece.Body;
		List<int> fullRows = new List<int>();
		int row, col;

		//for all the parts of the piece
		for(int i=0; i<body.Length; i++){
			
			row = body[i].y;
			col = body[i].x;

			//insert the piece into the grid
			_grid[row,col] = true;

			//update row counter
			_fullCellCount[row] += 1;

			//if row is full, mark it as full
			if(_fullCellCount[row] == Width)
				fullRows.Add(row);
		}

		return fullRows;
	}

	//O(Width*Height) on worst case. 
	private void CompactGrid(List<int> fullRows){

		//keeps track of how many times a given row should be moved down
		int jump = 0;

		//sort descending. Bottommost rows are the first
		fullRows.Sort((int a, int b) =>{ return -a.CompareTo(b);});

		//start from bottommost full row, then goes up to the top
		for(int row = fullRows[0]; row >= 0; row--){

			//we're on a full row. Clear the row
			if(_fullCellCount[row] == Width){
				ClearRow(row);
				jump++;
			}

			//we're on a row with some blocks. move the row down
			else if(_fullCellCount[row] > 0)
				MoveRow(row, row+jump);

			//no more blocks to inspect
			else
				break;
		}
	}

	//O(Width)
	private void ClearRow(int row){
		for(int col=0; col<Width; col++)
			_grid[row,col] = false;
		_fullCellCount[row] = 0;
	}

	//O(Width)
	private void MoveRow(int origin, int destination){
		for(int col=0; col<Width; col++){
			_grid[destination,col] = _grid[origin,col];
			_grid[origin,col] = false;
		}

		_fullCellCount[destination] = _fullCellCount[origin];
		_fullCellCount[origin] = 0;
	}

	//O(coords.length)
	private bool Overlaps(Cell[] coords){
		for(int i=0; i<coords.Length; i++){
			if(Contains(coords[i]))
				return true;
		}
		return false;
	}

	//O(1)
	private bool Contains(Cell c){
		return _grid[c.y,c.x];
	}
}
