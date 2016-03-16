using UnityEngine;
using System.Collections.Generic;
using System;

public class Polymino {

	public enum PolyminoType{NONE, I, J, L, S, Z, T, O};

	public int Row{get;set;}
	public int Col{get;set;}
	public int BoundSize{get;private set;}
	public PolyminoType Type{get;private set;}

	public int Degrees{get; private set;}
	public Polymino Rotated{
		get{
			Polymino rotation = Clone();

			rotation.Rotate();
			return rotation;
		}
	}

	public Cell BottomMost{get{return GetLowerBounds()[0];}}
	public Cell LeftMost{get{return GetLeftBounds()[0];}}
	public Cell RightMost{get{return GetRightBounds()[0];}}
	public Cell[] Body{get{return GetBody();}}
	public Cell[] OriginalBody{get{return (Cell[])_body.Clone();}}

	private Cell[] _body;
	private List<Cell> _lowerBounds;
	private List<Cell> _leftBounds; 
	private List<Cell> _rightBounds;   

	public Polymino(Cell[] body) : this(body,PolyminoType.NONE,0,0){}

	public Polymino(Cell[] body, PolyminoType type) : this(body,type,0,0){}
		
	public Polymino(Cell[] body, PolyminoType type, int col, int row){
		_body = (Cell[])body.Clone();
		_lowerBounds = new List<Cell>(); 
		_leftBounds = new List<Cell>();
		_rightBounds = new List<Cell>(); 

		Type = type;
		Row = row;
		Col = col;
		BoundSize = GetMaxCoordinate(_body) + 1;
	}

	public Polymino Clone(){
		Polymino clone = new Polymino(_body, Type, Col, Row);
		clone.Degrees = Degrees;
		clone.Type = Type;
		return clone;
	}

	//return a copy of the body translated to coordinates (Col, Row)
	private Cell[] GetBody(){
		Cell[] translatedBody = new Cell[_body.Length]; 
		for(int i=0; i<translatedBody.Length; i++){
			translatedBody[i].x = _body[i].x + Col;
			translatedBody[i].y = _body[i].y + Row;
		}
		return translatedBody;
	}

	public void Rotate(){
		_body = Rotate90Clockwise(_body);
		Degrees = (Degrees+90)%360;

		_lowerBounds.Clear();
		_leftBounds.Clear();
		_rightBounds.Clear();
	}
		
	public Cell[] GetLowerBounds(){

		//return cached
		if(_lowerBounds.Count > 0)
			return Translate(_lowerBounds.ToArray());

		//sort array by x coordinate (sort by column)
		Array.Sort(_body, (Cell a, Cell b) => {return a.x.CompareTo(b.x);});

		Cell potentialBound = _body[0];
		Cell current;

		for(int i=1; i<_body.Length; i++){

			current = _body[i];

			//Same column of the potential bound. Must check y position.
			if(current.x == potentialBound.x){
				if(current.y > potentialBound.y)
					potentialBound = current;
			}

			//Different column. Confirm the current lower bound and set a new potential bound 
			else{
				_lowerBounds.Add(potentialBound);
				potentialBound = current;
			}
		}

		//write the last identified bound
		_lowerBounds.Add(potentialBound);

		//sort array by y coordinate descending (so the bottommost point is the first) 
		_lowerBounds.Sort((Cell a, Cell b) => {return -a.y.CompareTo(b.y);});

		return Translate(_lowerBounds.ToArray());
	}

	public Cell[] GetRightBounds(){

		//return cached
		if(_rightBounds.Count > 0)
			return Translate(_rightBounds.ToArray());

		//sort array by y coordinate (sort by row)
		Array.Sort(_body, (Cell a, Cell b) => {return a.y.CompareTo(b.y);});

		Cell potentialBound = _body[0];
		Cell current;

		for(int i=1; i<_body.Length; i++){

			current = _body[i];

			//Same row of the potential bound. Must check x position.
			if(current.y == potentialBound.y){
				if(current.x > potentialBound.x){
					potentialBound = current;
				}
			}

			//Different row. Confirm the current right bound and set a new potential bound 
			else{
				_rightBounds.Add(potentialBound);
				potentialBound = current;
			}
		}

		//write the last identified bound
		_rightBounds.Add(potentialBound);

		//sort array by x coordinate descending (so the rightmost point is the first) 
		_rightBounds.Sort((Cell a, Cell b) => {return -a.x.CompareTo(b.x);});

		return Translate(_rightBounds.ToArray());
	}

	public Cell[] GetLeftBounds(){

		//return cached
		if(_leftBounds.Count > 0)
			return Translate(_leftBounds.ToArray());

		//sort array by y coordinate (sort by row)
		Array.Sort(_body, (Cell a, Cell b) => {return a.y.CompareTo(b.y);});

		Cell potentialBound = _body[0];
		Cell current;

		for(int i=1; i<_body.Length; i++){

			current = _body[i];

			//Same row of the potential bound. Must check x position.
			if(current.y == potentialBound.y){
				if(current.x < potentialBound.x)
					potentialBound = current;
			}

			//Different row. Confirm the current left bound and set a new potential bound 
			else{
				_leftBounds.Add(potentialBound);
				potentialBound = current;
			}
		}

		//write the last identified bound
		_leftBounds.Add(potentialBound);

		//sort array by x coordinate ascending (so the leftmost point is the first) 
		_leftBounds.Sort((Cell a, Cell b) => {return a.x.CompareTo(b.x);});

		return Translate(_leftBounds.ToArray());
	}
		
	//http://stackoverflow.com/questions/1457605/rotating-cordinates-around-pivot-tetris
	//rotates the given coordinates by 90 degrees clockwise [O(n)]
	private Cell[] Rotate90Clockwise(Cell[] coords){
		int temp;
		for(int i=0; i<coords.Length; i++){
			temp = coords[i].x;
			coords[i].x =  1 - (coords[i].y - (BoundSize-2));
			coords[i].y = temp;
		}
		return coords;
	}

	private Cell[] Rotate90CounterClockwise(Cell[] coords){
		int temp;
		for(int i=0; i<coords.Length; i++){
			temp = coords[i].x;
			coords[i].x = coords[i].y;
			coords[i].y = 1 - (temp - (BoundSize-2));
		}
		return coords;
	}

	private Cell[] Translate(Cell[] coords){
		for(int i=0; i<coords.Length; i++){
			coords[i].x += Col;
			coords[i].y += Row;
		}
		return coords;
	}

	//calculates the size of the box containing the piece [O(n)]
	private int GetMaxCoordinate(Cell[] coords){
		int max = 0;
		int greater;

		for(int i=0; i<coords.Length; i++){
			greater = Mathf.Max(coords[i].x,coords[i].y);
			if(greater>max)
				max = greater;
		}
		return max;
	}
}
