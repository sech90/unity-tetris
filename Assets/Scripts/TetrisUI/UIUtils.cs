using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Provides useful functions expecially for handling the 3D bricks in the grid
/// </summary>
public class UIUtils {

	private static UIUtils _instance;
	public static UIUtils Instance{
		get{
			if(_instance == null)
				_instance = new UIUtils();
			return _instance;
		}
	}

	//will store a reference to the 3D brick model
	private UIBrick BrickPrefab;
	private Material cyan,blue,green,red,orange,purple,grey, yellow;

	//by default assume that brick size is 1 unit
	private float _unitSize = 1.0f;

	//Creates a 3D polymino.
	public static GameObject MakePolymino(Polymino piece){

		//load 3D model if not loaded
		if(Instance.BrickPrefab == null)
			Instance.BrickPrefab = LoadBrick();

		//create new container for the single bricks
		GameObject polymino = new GameObject("Polymino"+piece.Type);

		//get the non translated body cells
		Cell[] body = piece.OriginalBody;

		//create the bricks, assign color and position relative to parent
		for(int i=0; i<body.Length; i++){
			UIBrick brick = GameObject.Instantiate<UIBrick>(Instance.BrickPrefab);
			brick.transform.parent = polymino.transform;
			brick.transform.localPosition = CellToLocalPosition(body[i]);
			brick.Type = piece.Type;
		}
		return polymino;
	}

	//reuse bricks and rearrange them according to the new placement
	public static void RearrangeBricks(GameObject bricks, Polymino piece){

		Cell[] body = piece.OriginalBody;

		//rearrange the bricks
		for(int i=0; i<body.Length; i++){
			Transform brick = bricks.transform.GetChild(i);
			brick.transform.localPosition = CellToLocalPosition(body[i]);
		}
	}

	//used for calculating the bricks scale
	public static float CalculateGridUnitSize(Bounds bounds, int width, int height){
		float hLength = bounds.size.x/width;
		float vLength = bounds.size.y/height;

		return Mathf.Min(hLength,vLength);
	}

	//Cell(x,y) will be Position(x,-y). Center will be the top-left corner
	public static Vector3 CellToLocalPosition(Cell c){
		float halfUnit = Instance._unitSize/2;

		return new Vector3(
			(c.x * Instance._unitSize) + halfUnit,
			(-c.y * Instance._unitSize) - halfUnit,
			0
		);
	}

	//inverse operation of CoordToLocalPosition
	public static Cell LocalPositionToCell(Vector3 pos){

		//prevent very unlikely case, but one never knows..
		if(Instance._unitSize == 0)
			throw new Exception("unitSize is 0. Preventing division by 0");

		float halfUnit = Instance._unitSize/2;
		Cell c = new Cell();

		c.x = (int)((pos.x - halfUnit)/Instance._unitSize);
		c.y = (int)(-(pos.y + halfUnit)/Instance._unitSize);

		return c;
	}

	//Shared materials are way more efficient than assigning separate colors to renderers
	public static Material GetPieceMaterial(Polymino.PolyminoType type){
		switch(type){
		case Polymino.PolyminoType.I:
			if(Instance.cyan == null)
				Instance.cyan = Resources.Load<Material>("Materials/cyan");
			return Instance.cyan;
		case Polymino.PolyminoType.J:
			if(Instance.blue == null)
				Instance.blue = Resources.Load<Material>("Materials/blue");
			return Instance.blue;
		case Polymino.PolyminoType.O:
			if(Instance.yellow == null)
				Instance.yellow = Resources.Load<Material>("Materials/yellow");
			return Instance.yellow;
		case Polymino.PolyminoType.S:
			if(Instance.green == null)
				Instance.green = Resources.Load<Material>("Materials/green");
			return Instance.green;
		case Polymino.PolyminoType.Z:
			if(Instance.red == null)
				Instance.red = Resources.Load<Material>("Materials/red");
			return Instance.red;
		case Polymino.PolyminoType.T:
			if(Instance.purple == null)
				Instance.purple = Resources.Load<Material>("Materials/purple");
			return Instance.purple;
		case Polymino.PolyminoType.L:
			if(Instance.orange == null)
				Instance.orange = Resources.Load<Material>("Materials/orange");
			return Instance.orange;
		default:
			if(Instance.grey == null)
				Instance.grey = Resources.Load<Material>("Materials/grey");
			return Instance.grey;
		}
	}

	//load prefab and set unit size accordingly (assumes that scale is uniform for all axes)
	private static UIBrick LoadBrick(){
		UIBrick obj = Resources.Load<UIBrick>("Brick");
		Instance._unitSize = obj.transform.lossyScale.x;
		return obj;
	}
}
