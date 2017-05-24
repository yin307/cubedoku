using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CUBE_STATUS {IDLE, SELECTED, DROPING};

public class CubeController : MonoBehaviour {
	[System.Serializable]
	public class Cube{
		public int rowId;
		public int colId;
		public int value;
		public CUBE_STATUS status;

		public Cube (int r, int c, int value){
			this.rowId = r;
			this.colId = c;
			this.value = value;
			status = CUBE_STATUS.IDLE;
		}

		public bool isSelected()
		{
			return status == CUBE_STATUS.SELECTED;
		}
	}
	public Cube Me;
	public ArrayController AC;
	public GameObject rightConnect;
	public GameObject upConnect;
	public GameObject middle;

	public GameObject sameRight;
	public GameObject sameUp;

	public Vector3 nextPos;
	public Animator ani;

	public Text numberSame;
	public int curNumberSame;
	int nd;

	public SpriteRenderer sp;
	public bool isBomed;
	Color bigCubeColor = new Color (49f / 255f, 105f / 255f, 148f / 255f, 1);

	public GameObject bullet;
	public Color defaultColor;
	void OnEnable()
	{
		nextPos = transform.position;
		if (ani == null) {
			ani = gameObject.GetComponent<Animator> ();
		}
		numberSame.enabled = false;
		curNumberSame = 1;
		playAnimation ("Idle");
		isBomed = false;
		sp.color = defaultColor;
		Me.status = CUBE_STATUS.IDLE;

	}

	void OnDisable()
	{
		refresh ();
		AC.removeCell (Me.rowId, Me.colId);
		AC.pooling.Add (this.gameObject);
		if (gameObject.GetComponent<GroupSame> ()) {
			Destroy (gameObject.GetComponent<GroupSame> ());
		}
	}

	public void setRowCol(int r, int c){
		Me.rowId = r;
		Me.colId = c;
	}
	string moveType;
	public void drop(int numberDrop){
		nd = numberDrop;
		AC.removeCell (Me.rowId, Me.colId);
		Me.rowId -= numberDrop;
		nextPos = nextPos + new Vector3 (0, -Define.deltaPos * numberDrop, 0);
		AC.fillCell (Me.rowId, Me.colId, this.gameObject);
		StartCoroutine (moveToNextPos ());
	}

	public void merg(CubeController cc){
		refresh ();
		if (GetComponent<GroupSame> ()) {
			Destroy (GetComponent<GroupSame> ());
		}
		StartCoroutine (mergToTarget (cc));
	}

	IEnumerator mergToTarget(CubeController cc){
		Vector3 target = cc.gameObject.transform.position;
		float t = 0;
		while (transform.position != target) {
			t += Time.deltaTime / 0.2f;
			transform.position = Vector3.Lerp (transform.position, target, t);
			yield return new WaitForSeconds (Time.deltaTime);
		}
		gameObject.SetActive (false);
		AC.curentWaitCube++;
		cc.onIncrea (curNumberSame);
	}

	IEnumerator moveToNextPos(){
		float t = 0;
		while (transform.position != nextPos) {
			t += Time.deltaTime / 0.2f;
			transform.position = Vector3.Lerp (transform.position, nextPos, t);
			yield return new WaitForSeconds (Time.deltaTime);
		} 


	}

	void OnMouseDown(){
		AC.onClickCube (this);

		//middle.SetActive (!middle.activeSelf);
	}

	public void OnSelect()
	{
		middle.SetActive(true);
		Me.status = CUBE_STATUS.SELECTED;
	}

	public void refresh()
	{
		numberSame.enabled = false;
		middle.SetActive (false);
		rightConnect.SetActive (false);
		upConnect.SetActive (false);
		sameUp.SetActive (false);
		sameRight.SetActive (false);
		Me.status = CUBE_STATUS.IDLE;
	}



	public void setRight(bool active)
	{
		rightConnect.SetActive (active);
	}
	public void setUp(bool active)
	{
		upConnect.SetActive (active);
	}

	public void setSameRight(bool active)
	{
		sameRight.SetActive (active);
	}
	public void setSameUp(bool active)
	{
		sameUp.SetActive (active);
	}
	public void onFail(){
		playAnimation ("Fail");
	}
	public void onCorrect(int minusValue){
		refresh ();

		if (isBomed) {
			sp.color = bigCubeColor;
			playAnimation ("Correct");
			AC.increaWaitCube ();
		} else {
			List<GameObject> myGroup = AC.collectSameStartFromCell(Me.rowId, Me.colId, 99);
			if (myGroup.Count > 1) {
				if (this.GetComponent<GroupSame> ()) {
					Destroy (this.GetComponent<GroupSame> ());
				}
				for (int g = 0; g < myGroup.Count; g++) {
					if (myGroup [g].GetComponent<CubeController> () != this) {
						myGroup [g].GetComponent<CubeController> ().merg (this);
						AC.increaWaitCube ();
					}
				}
			} else {
				onIncrea (-minusValue);
				if (curNumberSame < 1) {		
					playAnimation ("Correct");
					AC.increaWaitCube ();
				}
			}
		}

		if (Me.value == Define.bigValue) {
			onBom ();
		}
			
		CubeController leftCube = ArrayHelper.getLeftCube (AC.CUBES, Me.rowId, Me.colId);
		if (leftCube) {
			leftCube.setSameRight (false);
		}
		CubeController belowCube = ArrayHelper.getBelowCube(AC.CUBES, Me.rowId, Me.colId);
		if (belowCube) {
			belowCube.setSameUp (false);
		}
	}

	public void onBom(){
		List<GameObject> aroundCubes = ArrayHelper.getAroundACell (AC.CUBES, Me.rowId, Me.colId, AC.row, AC.col);
		for (int ac = 0; ac < aroundCubes.Count; ac++) {
			CubeController acItem = aroundCubes [ac].GetComponent<CubeController> ();
			if (acItem != null && !acItem.isBomed) {
				acItem.isBomed = true;
				acItem.onCorrect (0);
			}
		}
	}

	public void disable(){
		gameObject.SetActive (false);
		AC.increaCurWaitCube ();
	}

	public void onIncrea(int am){
		curNumberSame += am;
//		playAnimation ("Increa");
//		numberSame.text = curNumberSame.ToString ();
		if (curNumberSame < Define.bigValue) {
			playAnimation ("Increa");
			numberSame.text = curNumberSame.ToString ();
		} else {
			curNumberSame = Define.bigValue;
			numberSame.text = curNumberSame.ToString ();
			numberSame.color = Color.white;
			sp.color = bigCubeColor;
			Me.value = Define.bigValue;
		}
			
	}

	public void checkNumberSame(){
		if (curNumberSame > 1) {			
			playAnimation ("Owned");
		} else {
			playAnimation ("Idle");
		}
	}		

	public void playAnimation(string name){
		switch (name) {
		case "Idle":
			ani.SetBool ("isIdle", true);
			ani.SetBool ("isFail", false);
			ani.SetBool ("isCorrect", false);
			ani.SetBool ("isOwned", false);
			ani.SetBool ("isIncrea", false);
			break;
		case "Fail":
			ani.SetBool ("isIdle", false);
			ani.SetBool ("isFail", true);
			ani.SetBool ("isCorrect", false);
			ani.SetBool ("isOwned", false);
			ani.SetBool ("isIncrea", false);
			break;
		case "Correct":
			ani.SetBool ("isIdle", false);
			ani.SetBool ("isFail", false);
			ani.SetBool ("isCorrect", true);
			ani.SetBool ("isOwned", false);
			ani.SetBool ("isIncrea", false);
			break;
		case "Owned":
			ani.SetBool ("isIdle", false);
			ani.SetBool ("isFail", false);
			ani.SetBool ("isCorrect", false);
			ani.SetBool ("isOwned", true);
			ani.SetBool ("isIncrea", false);
			break;
		case "Increa":
			ani.SetBool ("isIdle", false);
			ani.SetBool ("isFail", false);
			ani.SetBool ("isCorrect", false);
			ani.SetBool ("isOwned", false);
			ani.SetBool ("isIncrea", true);
			break;
		}
	}

	public void onDoneFailAnimation(){
		AC.increaCurWaitCube ();
		if (curNumberSame > 1) {
			playAnimation ("Owned");
		} else {
			playAnimation ("Idle");
		}
	}

	public void fire(List<GameObject> targets){
		for (int i = 0; i < targets.Count; i++) {
			GameObject bull = Instantiate(bullet, transform.position, transform.rotation) as GameObject;
			bull.GetComponent<Bullet> ().target = targets [i].GetComponent<CubeController>();
			//bull.transform.LookAt (targets [i].transform);
			bull.GetComponent<Bullet> ().move = true;
			bull.GetComponent<Bullet> ().enabled = true;
		}
	}

	public void onDamaged(){
		//AC.handleACubeCorrect (this, 1);
	}
}

