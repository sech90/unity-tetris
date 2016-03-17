using UnityEngine;
using System.Collections;

public class UIBrick : MonoBehaviour {

	[SerializeField] ParticleSystem ExplosionEffect;

	private Renderer _renderer;
	private Polymino.PolyminoType _type;

	public Polymino.PolyminoType Type{
		get{return _type;}
		set{
			_type = value;
			Color = UIUtils.GetPieceColor(_type);
		}
	}

	public Color Color{
		get{return _renderer.material.color;}
		set{_renderer.material.color = value;}
	}
		
	void Awake(){
		_renderer = GetComponentInChildren<Renderer>();
	}

	public void Explode(){
		ParticleSystem particles = Instantiate(ExplosionEffect,transform.position,Quaternion.identity) as ParticleSystem;
		particles.startColor = Color;

		Destroy(particles.gameObject,1.2f);
		Destroy(gameObject);
	}

	public void FallDownBy(int units, float speed){
		float distance = transform.lossyScale.x * units;
		transform.position = new Vector3(transform.position.x, transform.position.y-distance, 0);
	}
}
