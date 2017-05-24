using UnityEngine;
using System.Collections;

public class GUITest : MonoBehaviour {

	public Texture EN;
	public Texture ES;
	public Texture FR;

	void Start () {
		LocalizationManager.instance.SetLang("ES");
	}

	void OnGUI () {
		GUI.Label(new Rect(Screen.width/2 - 64, Screen.height/2 - 16, 128, 32), LocalizationManager.instance.GetWord("Hello"));

		if(GUI.Button(new Rect(8, 8, 96, 64), EN))
		{
			LocalizationManager.instance.SetLang("EN");
		}

		if(GUI.Button(new Rect(8, 80, 96, 64), ES))
		{
			LocalizationManager.instance.SetLang("ES");
		}

		if(GUI.Button(new Rect(8, 152, 96, 64), FR))
		{
			LocalizationManager.instance.SetLang("FR");
		}
	}
}
