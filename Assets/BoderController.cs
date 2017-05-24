using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoderController : MonoBehaviour {

	public LineRenderer top;
	public LineRenderer bot;
	public LineRenderer right;
	public LineRenderer left;
	public ArrayController ac;
	public void setBorder(int minCol, int maxCol, int maxRow, int minRow){
//		Debug.Log (minCol + "  " + maxCol + "  " + maxRow + "  " + minRow);
		Vector3 conner1 = ArrayHelper.getPos (maxRow, minCol, ac.row, ac.col) + new Vector3(-0.35f, 0.35f, 0);
		Vector3 conner2 = ArrayHelper.getPos (maxRow, maxCol, ac.row, ac.col) + new Vector3(0.35f, 0.35f, 0);
		Vector3 conner3 = ArrayHelper.getPos (minRow, maxCol, ac.row, ac.col) + new Vector3(0.35f, -0.35f, 0);
		Vector3 conner4 = ArrayHelper.getPos (minRow, minCol, ac.row, ac.col) + new Vector3(-0.35f, -0.35f, 0);

		top.SetPosition (0, conner1 + new Vector3 (-0.015f, 0, 0));
		top.SetPosition (1, conner2 + new Vector3 (0.015f, 0, 0));

		right.SetPosition (0, conner2);
		right.SetPosition (1, conner3);

		bot.SetPosition (0, conner3 + new Vector3 (0.015f, 0, 0));
		bot.SetPosition (1, conner4 + new Vector3 (-0.015f, 0, 0));

		left.SetPosition (0, conner4);
		left.SetPosition (1, conner1);

	}
}
