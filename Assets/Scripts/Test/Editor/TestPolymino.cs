using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class TestPolymino {

	class Coords{

		public Cell[][] All{get{return new Cell[][]{I,T,L,J,S,Z,O};}}

		public Cell[] I;
		public Cell[] T;
		public Cell[] L;
		public Cell[] J;
		public Cell[] S;
		public Cell[] Z;
		public Cell[] O;
	}

	private Polymino I,T,L,J,S,Z,O;
	private Polymino[] All;

	private Coords coords;

	public TestPolymino(){
		coords = new Coords();
		coords.I = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(3,1)};
		coords.T = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(1,0)};
		coords.L = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(2,0)};
		coords.J = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(0,0)};
		coords.S = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(1,0), new Cell(2,0)};
		coords.Z = new Cell[]{new Cell(0,0), new Cell(1,0), new Cell(1,1), new Cell(2,1)};
		coords.O = new Cell[]{new Cell(0,0), new Cell(1,0), new Cell(1,1), new Cell(0,1)};


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
		List<Cell[]>[] testRots = new List<Cell[]>[toTest.Length];

		testRots[0] = new List<Cell[]>();
		testRots[0].Add(coords.O);
		testRots[0].Add(coords.O);
		testRots[0].Add(coords.O);

		testRots[1] = new List<Cell[]>();
		testRots[1].Add(new Cell[]{new Cell(1,0), new Cell(1,1), new Cell(2,1), new Cell(1,2)});
		testRots[1].Add(new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(1,2)});
		testRots[1].Add(new Cell[]{new Cell(1,0), new Cell(1,1), new Cell(0,1), new Cell(1,2)});

		testRots[2] = new List<Cell[]>();
		testRots[2].Add(new Cell[]{new Cell(2,0), new Cell(2,1), new Cell(2,2), new Cell(2,3)});
		testRots[2].Add(new Cell[]{new Cell(0,2), new Cell(1,2), new Cell(2,2), new Cell(3,2)});
		testRots[2].Add(new Cell[]{new Cell(1,0), new Cell(1,1), new Cell(1,2), new Cell(1,3)});

		for(int i=0; i< testRots.Length; i++){
			Polymino p = toTest[i];
			List<Cell[]> rotations = testRots[i];

			foreach(Cell[] rot in rotations){
				
				//rotation clockwise must be correct
				p.Rotate();
				Assert.That(rot, Is.EquivalentTo(p.Body)," at index "+i);
			}
		}
	}
		
	[Test]
	public void Rotation2() {
		foreach(Polymino p in All){
			Cell[] initial = p.Body;

			for(int i=0; i<4;i++){
				Cell[] rotated = p.Rotated.Body;
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

		Cell[] boundsO = new Cell[]{new Cell(1,1), new Cell(0,1)};
		Cell[] boundsI = coords.I;
		Cell[] boundsS = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,0)};
		Cell[] boundsIrot = new Cell[]{new Cell(2,3)};

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

		Cell[] boundsO = new Cell[]{new Cell(1,0), new Cell(1,1)};
		Cell[] boundsI = new Cell[]{new Cell(3,1)};
		Cell[] boundsS = new Cell[]{new Cell(2,0), new Cell(1,1)};
		Cell[] boundsT = new Cell[]{new Cell(1,0), new Cell(2,1)};

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
		Cell[] boundsO = new Cell[]{new Cell(0,0), new Cell(0,1)};
		Cell[] boundsI = new Cell[]{new Cell(0,1)};
		Cell[] boundsS = new Cell[]{new Cell(0,1), new Cell(1,0)};
		Cell[] boundsT = new Cell[]{new Cell(0,1), new Cell(1,0)};

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

	private string PrintCoords(Cell[] coords){
		string s = "";
		foreach(Cell c in coords)
			s += c;

		return s;
	}
}
