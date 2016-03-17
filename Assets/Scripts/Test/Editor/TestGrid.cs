using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class TestGrid {


	[Test]
	public void TestPosToCell(){
		Cell[] j = new Cell[]{new Cell(0,1), new Cell(1,1), new Cell(2,1), new Cell(0,0)};

		foreach(Cell c in j){
			Vector3 p = UIUtils.CellToLocalPosition(c);
			Cell expect = UIUtils.LocalPositionToCell(p);
			Assert.AreEqual(c,expect);
		}

	}

	private string PrintCoords(Cell[] coords){
		string s = "";
		foreach(Cell c in coords)
			s += c;

		return s;
	}
}
