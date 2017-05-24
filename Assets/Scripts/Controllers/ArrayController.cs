using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayController : MonoBehaviour {
	
	public GameObject[] cubesToInstantiate;
	public GameObject[,] CUBES;
	public CubeController.Cube[,] CUBE_OBJECTS;
	public int row;
	public int col;
	public CubeController firstCube;
	public CubeController secondCube;
	List<CubeController> SELECTED_CUBE;
	bool processing;

	public int waitCube;
	public int curentWaitCube;

	public List<GameObject> pooling;

	public GameObject staticCube;
	string[,] styleGrid;

	public BoderController bc;

	// Use this for initialization
	void Start () {
		pooling = new List<GameObject> ();
		SELECTED_CUBE = new List<CubeController> ();
		styleGrid = StyleGrid.style0;
		CUBES = new GameObject[row * 2, col];
		spawnGirdCube ();
		connectSame ();
		//collectSame ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {

			startDrop ();
		}
	}

	void spawnGirdCube()
	{
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (!styleGrid [r, c].Equals ("#")) {
					int id = Random.Range (0, cubesToInstantiate.Length);
					GameObject cube = Instantiate (cubesToInstantiate [id], ArrayHelper.getPos (r, c, row, col), transform.rotation) as GameObject; 
					cube.GetComponent<CubeController> ().setRowCol (r, c);
					cube.GetComponent<CubeController> ().AC = this;
					CUBES [r, c] = cube;
					//CUBE_OBJECTS[r,c] = new CubeController
				} else {
					GameObject cube = Instantiate (staticCube, ArrayHelper.getPos (r, c, row, col), transform.rotation) as GameObject; 
					CUBES [r, c] = cube;
				}
			}
		}

//		CUBES [0 + 1, 1].SetActive (false);
//		CUBES [1 + 1, 1].SetActive (false);
//		Debug.Log(CUBES[0,0].GetComponent<CubeController>().Me.value + " === " + CUBES[0,0 + 1].GetComponent<CubeController>().Me.value);

	}

	public void fillCell(int r, int c, GameObject cube){
		CUBES [r, c] = cube;
	}

	public void removeCell(int r, int c){
		//Debug.Log (r + "  " + c);
		CUBES [r, c] = null;
	}

	public void onClickCube(CubeController cube)
	{
		//List<GameObject> aa = collectSameStartFromCell (cube.Me.rowId, cube.Me.colId, 99);
		if (!processing) {			
			if (firstCube == null) {
				firstCube = cube;
				bc.gameObject.SetActive (true);
				bc.setBorder (cube.Me.colId, cube.Me.colId, cube.Me.rowId, cube.Me.rowId);
			} else {
				if (secondCube == null) {
					secondCube = cube;
					if (secondCube == firstCube) {
						firstCube = null;
						secondCube = null;
						bc.gameObject.SetActive (false);
					} else {
						collectSelectCube ();
					}
				} else {
					//Debug.Log ();
				}
			}				
		}

	}

	public void collectSelectCube()
	{
		int maxRow, maxCol;
		int minRow, minCol;
		if (firstCube.Me.rowId < secondCube.Me.rowId) {
			minRow = firstCube.Me.rowId;
			maxRow = secondCube.Me.rowId;
		}else{
			minRow = secondCube.Me.rowId;
			maxRow = firstCube.Me.rowId;
		}

		if (firstCube.Me.colId < secondCube.Me.colId) {
			minCol = firstCube.Me.colId;
			maxCol = secondCube.Me.colId;
		}else{
			minCol = secondCube.Me.colId;
			maxCol = firstCube.Me.colId;
		}

		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (r >= minRow && r <= maxRow && c <= maxCol && c >= minCol) {
					if (CUBES [r, c].GetComponent<CubeController> ()) {
						//CUBES [r, c].GetComponent<CubeController> ().OnSelect ();
						SELECTED_CUBE.Add (CUBES [r, c].GetComponent<CubeController> ());
					}
				}
			}
		}

		bc.setBorder (minCol, maxCol, maxRow, minRow);
		StartCoroutine (handleCubeSelected ());
	}

	IEnumerator handleCubeSelected()
	{
		yield return new WaitForSeconds (0.5f);
		bc.gameObject.SetActive (false);
		//yield return new WaitForSeconds (0.3f);
		checkHaveSame ();
	}

	void connectCube()
	{
		for (int i = 0; i < SELECTED_CUBE.Count; i++) {
			CubeController.Cube cube = SELECTED_CUBE [i].Me;
			SELECTED_CUBE [i].setRight (ArrayHelper.haveRightNeiboor (cube.rowId, cube.colId, CUBES, col));
			SELECTED_CUBE [i].setUp (ArrayHelper.haveUpNeiboor (cube.rowId, cube.colId, CUBES, row));
		}
	}

	void checkHaveSame()
	{
		waitCube = 0;
		curentWaitCube = 0;
		List<int> res = ArrayHelper.haveSameColor (SELECTED_CUBE);
		if (res.Count == 0) {
			int minusValue = ArrayHelper.getMinValue (SELECTED_CUBE);
			List<CubeController> bigCubes = ArrayHelper.getBigValueCube (SELECTED_CUBE);
			if (bigCubes.Count > 0) {
				for (int bc = 0; bc < bigCubes.Count; bc++) {	
					bigCubes [bc].isBomed = true;
					bigCubes [bc].onCorrect (minusValue);
				}
			} else {
				for (int i = 0; i < SELECTED_CUBE.Count; i++) {	
					SELECTED_CUBE [i].onCorrect (minusValue);
				}
			}

			StartCoroutine (waitToCorrectDone ());
		} else {
			for (int i = 0; i < SELECTED_CUBE.Count; i++) {
				if (res.Contains (i)) {
					waitCube++;
					SELECTED_CUBE [i].onFail ();
					if (getLeftCube (SELECTED_CUBE [i].Me.rowId, SELECTED_CUBE [i].Me.colId) != null) {
						getLeftCube (SELECTED_CUBE [i].Me.rowId, SELECTED_CUBE [i].Me.colId).setSameRight (false);
					}
					if (getBelowCube (SELECTED_CUBE [i].Me.rowId, SELECTED_CUBE [i].Me.colId) != null) {
						getBelowCube (SELECTED_CUBE [i].Me.rowId, SELECTED_CUBE [i].Me.colId).setSameUp (false);
					}
				}
			}
			StartCoroutine (waitToFailDone ());
		}

		ArrayHelper.refreshListCube (SELECTED_CUBE);
		SELECTED_CUBE = new List<CubeController> ();
		firstCube = null;
		secondCube = null;
	}

	public void increaWaitCube(){
		waitCube++;
		//Debug.Log ("waitCube  " + waitCube);

	}

	public void increaCurWaitCube(){
		curentWaitCube++;
		//Debug.Log ("curentWaitCube  " + curentWaitCube);

	}

	public void handleACubeCorrect(CubeController cube, int minusValue){
		//CubeController thisCC = SELECTED_CUBE [i];
		int r = cube.Me.rowId;
		int c = cube.Me.colId;
		List<GameObject> myGroup = collectSameStartFromCell(r, c, 99);
		if (myGroup.Count > 1) {
			if (cube.GetComponent<GroupSame> ()) {
				Destroy (cube.GetComponent<GroupSame> ());
			}
			for (int g = 0; g < myGroup.Count; g++) {
				if (myGroup [g].GetComponent<CubeController> () != cube) {
					myGroup [g].GetComponent<CubeController> ().merg (cube);
					waitCube++;
				}
			}
		} else {
			cube.onIncrea (-minusValue);
			if (cube.curNumberSame < 1) {
				cube.onCorrect (- minusValue);
				waitCube++;
			}
		}
		if (getLeftCube (cube.Me.rowId, cube.Me.colId) != null) {
			getLeftCube (cube.Me.rowId, cube.Me.colId).setSameRight (false);
		}
		if (getBelowCube (cube.Me.rowId, cube.Me.colId) != null) {
			getBelowCube (cube.Me.rowId, cube.Me.colId).setSameUp (false);
		}
	}

	Dictionary<int, List<GameObject>> curSame = new Dictionary<int, List<GameObject>>();

	IEnumerator waitToCorrectDone(){
		yield return new WaitUntil (() => waitCube == curentWaitCube);	
		startDrop ();
	}

	IEnumerator waitToFailDone(){
		yield return new WaitUntil (() => waitCube == curentWaitCube);
		yield return new WaitForSeconds (0.2f);
		connectSame ();
	}

	void collectSame()
	{
		int index = -1;
		curSame.Clear ();
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				if (!CUBES [i, j].GetComponent<GroupSame> ()) {
					List<GameObject> l = collectSameStartFromCell (i, j, index);
					if (l.Count > 1) {						
						curSame.Add (index, l);
						index++;
					}
				}
			}
		}
	}


	public List<GameObject> collectSameStartFromCell(int r, int c, int gId)
	{
		List<GameObject> res = new List<GameObject> ();
		res.Add (CUBES [r, c]);
		CUBES [r, c].AddComponent<GroupSame> ();
		CUBES [r, c].GetComponent<GroupSame> ().sameGroup = gId;
		int i = 0;
		int rr = r;
		int cc = c;
		for(int k = 0; k < row * col; k ++){
			if (i < res.Count) {
				List<GameObject> l = getSameAround (rr, cc, gId);
				if (l.Count > 0) {					
					res.AddRange (l);
				}
				i++;
				if (i >= res.Count) {
					break;
				}
				rr = res [i].GetComponent<CubeController> ().Me.rowId;
				cc = res [i].GetComponent<CubeController> ().Me.colId;
			} else {				
				break;
			}
		}
//		if (res.Count == 1) {
//			Destroy (CUBES [r, c].GetComponent<GroupSame> ());
//		} else {
//			
//		}
		for (int s = 0; s < res.Count; s++) {
			Destroy (res [s].GetComponent<GroupSame> ());
		}
		return res;
	}

	List<GameObject> getSameAround(int r, int c, int gId){
		List<GameObject> res = new List<GameObject> ();
		if (isValidCell (r + 1, c)) {
			if (ArrayHelper.isSameColor (CUBES [r, c], CUBES [r + 1, c])) {				
				if (!CUBES [r + 1, c].GetComponent<GroupSame> ()) {
					CUBES [r + 1, c].AddComponent<GroupSame> ();
					CUBES [r + 1, c].GetComponent<GroupSame> ().sameGroup = gId;
					res.Add (CUBES [r + 1, c]);	
				}
				//return true;
			}
		}

		if (isValidCell (r - 1, c)) {
			if (ArrayHelper.isSameColor (CUBES [r, c], CUBES [r - 1, c])) {
				if (!CUBES [r - 1, c].GetComponent<GroupSame> ()) {
					CUBES [r - 1, c].AddComponent<GroupSame> ();
					CUBES [r - 1, c].GetComponent<GroupSame> ().sameGroup = gId;
					res.Add (CUBES [r - 1, c]);	
				}
			}
		}
		if (isValidCell (r, c + 1)) {
			if (ArrayHelper.isSameColor (CUBES [r, c], CUBES [r, c+1])) {
				if (!CUBES [r, c + 1].GetComponent<GroupSame> ()) {
					CUBES [r, c + 1].AddComponent<GroupSame> ();
					CUBES [r, c + 1].GetComponent<GroupSame> ().sameGroup = gId;
					res.Add (CUBES [r, c + 1]);	
				}
			}
		}
		if (isValidCell (r, c - 1)) {
			if (ArrayHelper.isSameColor (CUBES [r, c], CUBES [r, c - 1])) {
				if (!CUBES [r, c - 1].GetComponent<GroupSame> ()) {
					CUBES [r, c - 1].AddComponent<GroupSame> ();
					CUBES [r, c - 1].GetComponent<GroupSame> ().sameGroup = gId;
					res.Add (CUBES [r, c - 1]);	
				}
			}
		}
		//Debug.Log (">>>>>>>>>>>>" + res.Count);
		return res;
	}

	bool isValidCell(int r, int c){
		if (r < 0 || c < 0 || r >= row || c >= col) {
			return false;
		} else {
			if (CUBES [r, c] != null) {
				return true;
			} else {
				return false;
			}
		}
	}

	void connectSame()
	{
		int cc = 0;
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				
				if (r == row - 1 && c == col - 1) {
					continue;
				} else {
					if (c != col - 1) {
						if(CUBES [r, c].GetComponent<CubeController> ()){
							CubeController.Cube thisCube = CUBES [r, c].GetComponent<CubeController> ().Me;
							if (thisCube.value == Define.bigValue) {
								continue;
							}
							if (CUBES [r, c + 1].GetComponent<CubeController> ()) {
								CubeController.Cube rightCube = CUBES [r, c + 1].GetComponent<CubeController> ().Me;
								if (thisCube.value == rightCube.value) {
									CUBES [r, c].GetComponent<CubeController> ().setSameRight (true);
								}
							}
						}



					}
					if (r != row - 1) {
						if (CUBES [r, c].GetComponent<CubeController> ()) {
							CubeController.Cube thisCube = CUBES [r, c].GetComponent<CubeController> ().Me;
							if (CUBES [r + 1, c].GetComponent<CubeController> ()) {
								CubeController.Cube upCube = CUBES [r + 1, c].GetComponent<CubeController> ().Me;
								if (thisCube.value == upCube.value) {
									CUBES [r, c].GetComponent<CubeController> ().setSameUp (true);
								}
							}
						}

					}
						
				}
			}

		}
	}
		

	void startDrop()
	{
		for (int i = 0; i < col; i++) {
			List<GameObject> cellInCollum = ArrayHelper.getCellsInCollumn (i, row, CUBES);
			if (cellInCollum.Count == row) {
				continue;
			} else {
				//List<GameObject> cellsWillDrop = ArrayHelper.getCellsWillDrop (i, row, CUBES);
				cellInCollum.AddRange (instantiateToFillCube (i, row - cellInCollum.Count));
				for(int j = 0; j < cellInCollum.Count; j ++){
					if (cellInCollum [j].GetComponent<CubeController> ()) {
						CubeController cc = cellInCollum [j].GetComponent<CubeController> ();
						int numberDrop = 0;
						if (isFullToHighestStatic (cc.Me.colId)) {
							numberDrop = getNumberDropWhithNull (cc.Me.rowId, cc.Me.colId);
						} else {
							numberDrop = getNumberDropWhithStatic (cc.Me.rowId, cc.Me.colId);
						}
						cc.drop (numberDrop);
					}
				}
				disableConnect ();
				StartCoroutine (waitToFill ());

			}
		}
	}

	bool isStatic(int r, int c){
		if (r >= 0) {
			return CUBES [r, c] != null && !CUBES [r, c].GetComponent<CubeController> ();
		} else {
			return false;
		}
	}

	int getNumberDropWhithStatic(int r, int c){
		int res = 0;
		if (r == 0) {
			return res;
		}
		for (int i = r - 1; i >=0; i -- ){			
			if (CUBES [i, c] == null || (isStatic(i, c) && i != 0)) {
				res++;
			}

		}
		return res;
	}

	int getNumberDropWhithNull(int r, int c){
		int res = 0;
		if (r == 0) {
			return res;
		}
		for (int i = r - 1; i >=0; i -- ){			
			if (CUBES [i, c] == null) {
				res++;
			}

		}
		return res;
	}

	bool isFullFromCell(int r, int c){
		for (int i = r - 1; i >= 0; i--) {
			if (CUBES [i, c] == null) {
				return false;
			}
		}
		return true;
	}

	bool isFullToHighestStatic(int c){
		int hight = 6;
		for (int i = row - 1; i >= 0; i--) {
			if (isStatic (i, c)) {
				hight = i;
				break;
			}
		}
		if (isFullFromCell (hight, c)) {
			return true;
		}
		return false;
	}

	List<GameObject> instantiateToFillCube(int c, int number)
	{
		List<GameObject> res = new List<GameObject> ();
		for (int i = 0; i < number; i++) {
			float x = (c - (col - 1) / 2) * Define.deltaPos;
			float y = ((row - 1) / 2) * Define.deltaPos + i * Define.deltaPos + Define.deltaPos;
			int id = Random.Range (0, cubesToInstantiate.Length);
			GameObject cube;
			if (getCube (id) != null) {
				cube = getCube (id);
				cube.transform.position = new Vector3 (x, y, 0);
			} else {
				cube = Instantiate (cubesToInstantiate [id], new Vector3 (x, y, 0), transform.rotation) as GameObject;
			}
			cube.GetComponent<CubeController> ().setRowCol (row + i, c);
			cube.GetComponent<CubeController> ().AC = this;
			CUBES [row + i, c] = cube;
			cube.SetActive (true);
			res.Add (cube);
		}
		return res;
	}

	IEnumerator waitToFill()
	{ 
		yield return new WaitUntil (() => ArrayHelper.filled (CUBES, row, col) == true);
		yield return new WaitForSeconds (0.2f);
		connectSame ();
		//collectSame ();
	}

	void disableConnect(){
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				if (CUBES [i, j] != null) {
					if (CUBES [i, j].GetComponent<CubeController> ()) {
						CUBES [i, j].GetComponent<CubeController> ().refresh ();
					}
				}
			}
		}
	}

	CubeController getLeftCube(int r, int c){
		CubeController res = null;
		if (c - 1 >= 0) {
			res = CUBES [r, c - 1].GetComponent<CubeController> ();
		} else {
			res = null;
		}
		return res;
	}

	CubeController getBelowCube(int r, int c){
		CubeController res = null;
		if (r - 1 >= 0) {
			res = CUBES [r - 1, c].GetComponent<CubeController> ();
		} else {
			res = null;
		}
		return res;
	}

	GameObject getCube(int value){
		for (int i = 0; i < pooling.Count; i++) {
			if (!pooling [i].activeSelf && pooling [i].GetComponent<CubeController> ().Me.value == value) {
				return pooling [i];
			}
		}
		return null;
			
	}
}
