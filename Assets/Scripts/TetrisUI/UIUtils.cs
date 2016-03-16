using UnityEngine;
using System.Collections;

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
	private GameObject BrickPrefab;

	//by default assume that brick size is 1 unit
	private float _unitSize = 1.0f;

	public GameObject MakePolymino(Polymino piece){

		//load 3D model if not loaded
		if(BrickPrefab == null)
			BrickPrefab = LoadBrick();

		//get color based on type
		Color color = GetPieceColor(piece.Type);

		//create new container for the single bricks
		GameObject polymino = new GameObject("Polymino"+piece.Type);

		//get the non translated body cells
		Cell[] body = piece.OriginalBody;

		//create the bricks, assign color and position relative to parent
		for(int i=0; i<body.Length; i++){
			GameObject block = GameObject.Instantiate<GameObject>(BrickPrefab);
			block.transform.parent = polymino.transform;
			block.transform.localPosition = CoordToLocalPosition(body[i]);
			block.GetComponentInChildren<Renderer>().material.color = color;
		}

		return polymino;
	}

	public float CalculateGridUnitSize(Bounds bounds, int width, int height){
		float hLength = bounds.size.x/width;
		float vLength = bounds.size.y/height;

		return Mathf.Min(hLength,vLength);
	}

	//Cell(x,y) will be Position(x,-y). Center will be the top-left corner
	private Vector3 CoordToLocalPosition(Cell c){
		float halfUnit = _unitSize/2;

		return new Vector3(
			(c.x * _unitSize) + halfUnit,
			(-c.y * _unitSize) - halfUnit,
			0
		);
	}

	public Color GetPieceColor(Polymino.PolyminoType type){
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
			return new Color32(153, 0, 255,1); 	//purple
		case Polymino.PolyminoType.L:
			return new Color32(255,102,0,1); 	//orange
		default:
			return Color.gray;
		}
	}

	//load prefab and set unit size accordingly (assumes that scale is uniform for all axes)
	private GameObject LoadBrick(){
		GameObject obj = Resources.Load<GameObject>("Brick");
		_unitSize = obj.transform.lossyScale.x;
		return obj;
	}
}
