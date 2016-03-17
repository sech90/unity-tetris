using UnityEngine;
using System.Collections;
using System;

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

	//by default assume that brick size is 1 unit
	private float _unitSize = 1.0f;

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

	public static void RearrangeBricks(GameObject bricks, Polymino piece){

		Cell[] body = piece.OriginalBody;

		//create the bricks, assign color and position relative to parent
		for(int i=0; i<body.Length; i++){
			Transform brick = bricks.transform.GetChild(i);
			brick.transform.localPosition = CellToLocalPosition(body[i]);
		}
			
		//bricks.transform.GetChild(0).localRotation =  Quaternion.Euler(0,0,degrees);
	}

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

		//prevent stupid input
		if(Instance._unitSize == 0)
			throw new Exception("unitSize is 0. Preventing division by 0");

		float halfUnit = Instance._unitSize/2;
		Cell c = new Cell();

		c.x = (int)((pos.x - halfUnit)/Instance._unitSize);
		c.y = (int)(-(pos.y + halfUnit)/Instance._unitSize);

		return c;
	}

	public static Color GetPieceColor(Polymino.PolyminoType type){
		switch(type){
		case Polymino.PolyminoType.I:
			return Color.cyan;
		case Polymino.PolyminoType.J:
			return Color.blue;
		case Polymino.PolyminoType.O:
			return Color.yellow;
		case Polymino.PolyminoType.S:
			return Color.green;
		case Polymino.PolyminoType.Z:
			return Color.red;
		case Polymino.PolyminoType.T:
			return new Color32(153, 0, 255,255); 	//purple
		case Polymino.PolyminoType.L:
			return new Color32(255,102,0,255); 		//orange
		default:
			return Color.gray;
		}
	}

	//load prefab and set unit size accordingly (assumes that scale is uniform for all axes)
	private static UIBrick LoadBrick(){
		UIBrick obj = Resources.Load<UIBrick>("Brick");
		Instance._unitSize = obj.transform.lossyScale.x;
		return obj;
	}
}
