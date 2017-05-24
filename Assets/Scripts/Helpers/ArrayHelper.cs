using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayHelper : MonoBehaviour {


	public static Vector3[,] getArrayPos(int row, int col){
		Vector3[,] res = new Vector3[row, col];
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				res [r, c] = getPos (r, c, row, col);
			}
		}

		return res;
	}

	public static Vector3 getPos (int r, int c, int row, int col){
		float x = (c - (col - 1) / 2) * Define.deltaPos;
		float y = (r - (row - 1) / 2) * Define.deltaPos;
		return new Vector3 (x, y, 0);
	}

	public bool haveSame(List<CubeController> L){
		for (int i = 0; i < L.Count - 1; i++) {
			if (L [i].Me.value == L [i + 1].Me.value) {
				return true;
			}
		}
		return false;
	}

	public static bool haveRightNeiboor(int r, int c, GameObject[,] cubes, int col)
	{
		if (c == col - 1) {
			return false;
		}
		if (cubes [r, c + 1].GetComponent<CubeController> ()) {
			if (cubes [r, c + 1].GetComponent<CubeController> ().Me.isSelected ()) {
				return true;
			}
		}
		return false;
	}

	public static bool haveUpNeiboor(int r, int c, GameObject[,] cubes, int row)
	{
		if (r == row - 1) {
			return false;
		}
		if (cubes [r + 1, c].GetComponent<CubeController> ()) {
			if (cubes [r + 1, c].GetComponent<CubeController> ().Me.isSelected ()) {
				return true;
			}
		}
		return false;
	}



	public static List<int> haveSameColor(List<CubeController> wraper)
	{
		List<int> res = new List<int> ();
		for (int i = 0; i < wraper.Count - 1; i++) {
			for (int j = i + 1; j < wraper.Count; j++) {
				if (wraper [i].Me.value == wraper [j].Me.value) {
					res.Add (i);
					if (res.Count == wraper.Count) {
						break;
					}
					res.Add (j);
					if (res.Count == wraper.Count) {
						break;
					}
				}
			}
		}
		return res;
	}

	public static void refreshListCube(List<CubeController> wraper)
	{
		for (int i = 0; i < wraper.Count; i++) {
			wraper [i].refresh ();
		}
	}

	public static bool isFullCol(int colId, int maxRow, GameObject[,] cubes){
		for (int i = 0; i < maxRow; i++) {
			if (cubes [i, colId] == null) {
				return false;
			}
		}
		return true;
	}

	public static List<GameObject> getCellsInCollumn(int colId, int maxRow, GameObject[,] cubes){
		List<GameObject> res = new List<GameObject> ();
		for (int i = 0; i < maxRow; i++) {
			if (cubes [i, colId] != null || (cubes [i, colId] && !cubes [i, colId].GetComponent<CubeController>())) {
				res.Add (cubes [i, colId]);
			}
		}
		return res;
	}

	public static List<GameObject> getCellsWillDrop(int colId, int maxRow, GameObject[,] cubes){
		List<GameObject> res = new List<GameObject> ();
		for(int i = maxRow - 1; i >= 0; i --){
			if (cubes [i, colId] != null || !cubes [i, colId].GetComponent<CubeController>()) {
				res.Add (cubes [i, colId]);
			} else {
				break;
			}
		}
		Debug.Log (res.Count);
		return res;
	}

	public static bool isNearAndSame(CubeController.Cube a, CubeController.Cube b)
	{
		return(((a.rowId == b.rowId && Mathf.Abs (a.colId - b.colId) == 1) || (a.colId == b.colId && Mathf.Abs (a.rowId - b.rowId) == 1))
		&& a.value == b.value);
	}

	public static bool isSameColor(GameObject a, GameObject b){
		if (!a.GetComponent<CubeController> () || !b.GetComponent<CubeController> ()) {
			return false;
		}
		CubeController.Cube ca = a.GetComponent<CubeController> ().Me;
		CubeController.Cube cb = b.GetComponent<CubeController> ().Me;
		return ca.value == cb.value;
	}

	public static bool filled(GameObject[,] cube, int r, int c){
		for (int i = 0; i < r; i++) {
			for (int j = 0; j < c; j++) {
				if (cube [i, j] == null) {
					return false;
				}
			}
		}
		return true;
	}

	public static int getMinValue(List<CubeController> L){
		int res = L [0].curNumberSame;
		for(int i = 1; i < L.Count; i ++){
			if (res > L [i].curNumberSame) {
				res = L [i].curNumberSame;
			}
		}

		return res;
	}

	public static List<GameObject> getAroundACell(GameObject[,] cubes, int r, int c, int row, int col){
		List<GameObject> res = new List<GameObject> ();
		if (isValidIndexAndNotNull (cubes, r - 1, c, row, col)) {
			res.Add (cubes [r - 1, c]);
		}
		if (isValidIndexAndNotNull (cubes, r + 1, c, row, col)) {
			res.Add (cubes [r + 1, c]);
		}

		if (isValidIndexAndNotNull (cubes, r, c - 1, row, col)) {
			res.Add (cubes [r, c - 1]);
		}

		if (isValidIndexAndNotNull (cubes, r, c + 1, row, col)) {
			res.Add (cubes [r, c + 1]);
		}
		if (isValidIndexAndNotNull (cubes, r - 1, c - 1, row, col)) {
			res.Add (cubes [r - 1, c - 1]);
		}

		if (isValidIndexAndNotNull (cubes, r - 1, c + 1, row, col)) {
			res.Add (cubes [r - 1, c + 1]);
		}

		if (isValidIndexAndNotNull (cubes, r + 1, c - 1, row, col)) {
			res.Add (cubes [r + 1, c - 1]);
		}

		if (isValidIndexAndNotNull (cubes, r + 1, c + 1, row, col)) {
			res.Add (cubes [r + 1, c + 1]);
		}
		return res;
	}

	public static bool isValidIndex(int r, int c, int row, int col){
		return r >= 0 && r < row && c >= 0 && c < col;
	}

	public static bool isValidIndexAndNotNull(GameObject[,] cubes, int r, int c, int row, int col){
		return isValidIndex (r, c, row, col) && cubes [r, c] != null;
	}

	public static List<CubeController> getBigValueCube(List<CubeController> wrapper){
		List<CubeController> res = new List<CubeController> ();
		for (int i = 0; i < wrapper.Count; i++) {
			if (wrapper [i].Me.value == Define.bigValue) {
				res.Add (wrapper [i]);
			}
		}
		return res;
	}

	public static CubeController getLeftCube(GameObject[,] CUBES, int r, int c){
		CubeController res = null;
		if (c - 1 >= 0) {
			res = CUBES [r, c - 1].GetComponent<CubeController> ();
		} else {
			res = null;
		}
		return res;
	}

	public static CubeController getBelowCube(GameObject[,] CUBES,int r, int c){
		CubeController res = null;
		if (r - 1 >= 0) {
			res = CUBES [r - 1, c].GetComponent<CubeController> ();
		} else {
			res = null;
		}
		return res;
	}
}


