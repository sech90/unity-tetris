using UnityEngine;
using System.Collections;

/// <summary>
/// The smallest piece that makes up a 3D polymino
/// </summary>
public class UIBrick : MonoBehaviour {

	[SerializeField] ParticleSystem ExplosionEffect;

	private Renderer _renderer;
	private Polymino.PolyminoType _type;

	public Polymino.PolyminoType Type{
		get{return _type;}
		set{ 
			_type = value;
			_renderer.material = UIUtils.GetPieceMaterial(_type);
		}
	}

	public Color Color{
		get{return _renderer.material.color;}
	}
		
	void Awake(){
		_renderer = GetComponentInChildren<Renderer>();
	}

	//Spawn partile system of the same color
	public void Explode(){
		ParticleSystem particles = Instantiate(ExplosionEffect,transform.position,Quaternion.identity) as ParticleSystem;
		particles.startColor = Color;

		//destroy after short period
		Destroy(particles.gameObject,1.2f);
		Destroy(gameObject);
	}

	//TODO: implement lerp fo the brick falls gracefully
	public void FallDownBy(int units, float speed){
		float distance = transform.lossyScale.x * units;
		transform.position = new Vector3(transform.position.x, transform.position.y-distance, 0);
	}

	public void GreyOut(){
		_renderer.material = UIUtils.GetPieceMaterial(Polymino.PolyminoType.NONE);
	}
}
