using UnityEngine;
using System.Collections;

public class UINextPiece : MonoBehaviour {

	private Bounds _bounds;
	private float _unitSize;
	private Vector3 _scale;
	private Vector3 _position;
	private GameObject _lastBricks;

	void Start(){
		_bounds =  GetComponent<Renderer>().bounds;

		//for semplicity, suppose that polyminos fits in a 4x4 grid
		_unitSize = UIUtils.CalculateGridUnitSize(_bounds,4,4);

		//give 10% margin to all edges
		_unitSize *= 0.8f;

		//cache the correct scale for the bricks to be visualized
		_scale = new Vector3(_unitSize, _unitSize, _unitSize);
	} 
		
	public void ShowPolymino(Polymino piece){

		Clear();

		//get the bricks group
		GameObject bricks = UIUtils.MakePolymino(piece);

		//set scale and position
		bricks.transform.localScale = _scale;
		bricks.transform.parent = transform;

		//center the bricks
		bricks.transform.position = Centered(piece.BoundSize);
		_lastBricks = bricks;
	}

	public void Clear(){
		if(_lastBricks != null)
			Destroy(_lastBricks);
	}

	private Vector3 Centered(int size){
		Vector3 pos = _bounds.center;

		//move top-left by half size of bricks group
		pos.x -= (_unitSize*size/2);
		pos.y += (_unitSize*size/2);

		return pos;
	}
}
