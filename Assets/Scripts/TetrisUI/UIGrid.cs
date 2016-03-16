using UnityEngine;
using System.Collections;

public class UIGrid : MonoBehaviour {

	private Bounds _bounds;
	private float _unitSize;
	private Vector3 _scale;
	private Vector3 _topLeft;

	void Start(){
		_bounds =  GetComponent<Renderer>().bounds;
		Init(5,5);
	} 

	public void Init(int width, int height){
		
		//what's the bricks' size?
		_unitSize = UIUtils.Instance.CalculateGridUnitSize(_bounds,width,height);

		//cache the correct scale for the bricks to be visualized
		_scale = new Vector3(_unitSize, _unitSize, _unitSize);

		//calculate the top left corner considering the given grid dimensions (assure that the bricks will always be inside the grid)
		float bottom = _bounds.center.x - _bounds.extents.x;
		float top = _unitSize * height + bottom;
		float left = _unitSize * Mathf.Ceil(width/2.0f) - _bounds.center.x;
		_topLeft = new Vector3(top,left,transform.position.z-_unitSize);



		GameObject piece = UIUtils.Instance.MakePolymino(new Polymino(new Cell[]{new Cell(0,0)}));

		for(int i=0; i<height; i++){
			for(int j=0; j<width; j++){
				Vector3 pos = new Vector3(j,i,0);
				GameObject o = Instantiate<GameObject>(piece);
				o.transform.localScale = _scale;
				o.transform.parent = transform;
				o.transform.position = pos + _topLeft;
			}
		}
	}
}
