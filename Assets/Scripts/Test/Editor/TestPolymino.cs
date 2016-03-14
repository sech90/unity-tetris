using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class TestPolymino {

	class Coords{

		public Coord[][] All{get{return new Coord[][]{I,T,L,J,S,Z,O};}}

		public Coord[] I;
		public Coord[] T;
		public Coord[] L;
		public Coord[] J;
		public Coord[] S;
		public Coord[] Z;
		public Coord[] O;
	}

	private Polymino I,T,L,J,S,Z,O;
	private Polymino[] All;

	private Coords coords;

	public TestPolymino(){
		coords = new Coords();
		coords.I = new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(2,1), new Coord(3,1)};
		coords.T = new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(2,1), new Coord(1,0)};
		coords.L = new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(2,1), new Coord(2,0)};
		coords.J = new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(2,1), new Coord(0,0)};
		coords.S = new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(1,0), new Coord(2,0)};
		coords.Z = new Coord[]{new Coord(0,0), new Coord(1,0), new Coord(1,1), new Coord(2,1)};
		coords.O = new Coord[]{new Coord(0,0), new Coord(1,0), new Coord(1,1), new Coord(0,1)};


		I = new Polymino(coords.I);
		T = new Polymino(coords.T);
		L = new Polymino(coords.L);
		J = new Polymino(coords.J); 
		S = new Polymino(coords.S);
		Z = new Polymino(coords.Z);
		O = new Polymino(coords.O);
		All = new Polymino[]{I,T,L,J,S,Z,O};
	}

	[Test]
	public void InitialPosition() {
		foreach(Polymino p in All){
			Assert.AreEqual(0,p.Col);
			Assert.AreEqual(0,p.Row);
		}
	}

	[Test]
	public void Size() {
		Assert.AreEqual(2, O.BoundSize); 
		Assert.AreEqual(3, T.BoundSize);
		Assert.AreEqual(4, I.BoundSize);
	}

	[Test]
	public void Clone(){
		Polymino clone = I.Clone();
		Assert.AreEqual(I.Col, clone.Col);
		Assert.AreEqual(I.Row, clone.Row);
		Assert.AreEqual(I.Degrees, clone.Degrees);
		Assert.That(I.Body, Is.EquivalentTo(clone.Body));
	}

	[Test]
	public void Translation(){
		Polymino i = new Polymino(coords.I);
		i.Rotate();
		i.Col -= 2;
		i.Rotate();
		Assert.AreEqual(-2,i.LeftMost.x);
	}

	[Test]
	public void Rotation1() {
		Polymino[] toTest = {new Polymino(coords.O), new Polymino(coords.T), new Polymino(coords.I)};
		List<Coord[]>[] testRots = new List<Coord[]>[toTest.Length];

		testRots[0] = new List<Coord[]>();
		testRots[0].Add(coords.O);
		testRots[0].Add(coords.O);
		testRots[0].Add(coords.O);

		testRots[1] = new List<Coord[]>();
		testRots[1].Add(new Coord[]{new Coord(1,0), new Coord(1,1), new Coord(2,1), new Coord(1,2)});
		testRots[1].Add(new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(2,1), new Coord(1,2)});
		testRots[1].Add(new Coord[]{new Coord(1,0), new Coord(1,1), new Coord(0,1), new Coord(1,2)});

		testRots[2] = new List<Coord[]>();
		testRots[2].Add(new Coord[]{new Coord(2,0), new Coord(2,1), new Coord(2,2), new Coord(2,3)});
		testRots[2].Add(new Coord[]{new Coord(0,2), new Coord(1,2), new Coord(2,2), new Coord(3,2)});
		testRots[2].Add(new Coord[]{new Coord(1,0), new Coord(1,1), new Coord(1,2), new Coord(1,3)});

		for(int i=0; i< testRots.Length; i++){
			Polymino p = toTest[i];
			List<Coord[]> rotations = testRots[i];

			foreach(Coord[] rot in rotations){
				
				//rotation clockwise must be correct
				p.Rotate();
				Assert.That(rot, Is.EquivalentTo(p.Body)," at index "+i);
			}
		}
	}
		
	[Test]
	public void Rotation2() {
		foreach(Polymino p in All){
			Coord[] initial = p.Body;

			for(int i=0; i<4;i++){
				Coord[] rotated = p.Rotated.Body;
				p.Rotate();	

				//rotation should be the same as rotated
				Assert.That(rotated, Is.EquivalentTo(p.Body));
			}

			//after 4 rotations the ppiece must be the same
			Assert.That(initial, Is.EquivalentTo(p.Body));
		}
	}

	[Test]
	public void LowerBounds(){
		Polymino i = new Polymino(coords.I);
		Polymino t = new Polymino(coords.T);
		i.Rotate();
		t.Rotate();

		Coord[] boundsO = new Coord[]{new Coord(1,1), new Coord(0,1)};
		Coord[] boundsI = coords.I;
		Coord[] boundsS = new Coord[]{new Coord(0,1), new Coord(1,1), new Coord(2,0)};
		Coord[] boundsIrot = new Coord[]{new Coord(2,3)};

		//low bounds
		Assert.That(O.GetLowerBounds(), Is.EquivalentTo(boundsO)); 
		Assert.That(I.GetLowerBounds(), Is.EquivalentTo(boundsI)); 
		Assert.That(S.GetLowerBounds(), Is.EquivalentTo(boundsS)); 
		Assert.That(i.GetLowerBounds(), Is.EquivalentTo(boundsIrot)); 

		//test bottom most element
		Assert.AreEqual(1,O.BottomMost.y);
		Assert.AreEqual(1,I.BottomMost.y);
		Assert.AreEqual(1,S.BottomMost.y);
		Assert.AreEqual(2,t.BottomMost.y);
		Assert.AreEqual(3,i.BottomMost.y);
	}

	[Test]
	public void RightBounds(){

		Coord[] boundsO = new Coord[]{new Coord(1,0), new Coord(1,1)};
		Coord[] boundsI = new Coord[]{new Coord(3,1)};
		Coord[] boundsS = new Coord[]{new Coord(2,0), new Coord(1,1)};
		Coord[] boundsT = new Coord[]{new Coord(1,0), new Coord(2,1)};

		//low bounds
		Assert.That(O.GetRightBounds(), Is.EquivalentTo(boundsO)); 
		Assert.That(I.GetRightBounds(), Is.EquivalentTo(boundsI)); 
		Assert.That(S.GetRightBounds(), Is.EquivalentTo(boundsS)); 
		Assert.That(T.GetRightBounds(), Is.EquivalentTo(boundsT)); 

		//test bottom most element
		Assert.AreEqual(1,O.RightMost.x);
		Assert.AreEqual(3,I.RightMost.x);
		Assert.AreEqual(2,S.RightMost.x);
		Assert.AreEqual(2,T.RightMost.x);
	}

	[Test]
	public void LeftBounds(){
		Coord[] boundsO = new Coord[]{new Coord(0,0), new Coord(0,1)};
		Coord[] boundsI = new Coord[]{new Coord(0,1)};
		Coord[] boundsS = new Coord[]{new Coord(0,1), new Coord(1,0)};
		Coord[] boundsT = new Coord[]{new Coord(0,1), new Coord(1,0)};

		//low bounds
		Assert.That(O.GetLeftBounds(), Is.EquivalentTo(boundsO)); 
		Assert.That(I.GetLeftBounds(), Is.EquivalentTo(boundsI)); 
		Assert.That(S.GetLeftBounds(), Is.EquivalentTo(boundsS)); 
		Assert.That(T.GetLeftBounds(), Is.EquivalentTo(boundsT)); 

		//test bottom most element
		Assert.AreEqual(0,O.LeftMost.x);
		Assert.AreEqual(0,I.LeftMost.x);
		Assert.AreEqual(0,S.LeftMost.x);
		Assert.AreEqual(0,T.LeftMost.x);
	}

	private string PrintCoords(Coord[] coords){
		string s = "";
		foreach(Coord c in coords)
			s += c;

		return s;
	}
}
