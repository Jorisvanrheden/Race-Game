using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private CameraController cameraControl;

	public Editor editorRef;
	private Editor editor;

	public RaceTrack raceTrackRef;
	private RaceTrack raceTrack;

	private string GAME_STATE = "Menu";

	private int totalMaps = 3;

	// Use this for initialization
	void Start () {
		cameraControl = Camera.main.GetComponent<CameraController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

		switch (GAME_STATE) {
		case "Menu":
			if(GUI.Button(new Rect(0,0,100,50), "Editor")){
				GAME_STATE = "ChooseEditorMap";
			}
			else if(GUI.Button(new Rect(0,70,100,50), "Racetrack")){
				GAME_STATE = "ChooseRacetrack";
			}
			break;

		case "Editor":
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				editor.Destroy();
				
				GAME_STATE = "Menu";
			}
			break;

		case "ChooseEditorMap":
			for(int i=0;i<totalMaps;i++){

				string content = "";
				if(PlayerPrefs.HasKey("Start"+i.ToString())){
					content = "Edit Map: ";
				}
				else content = "EMPTY: ";

				if(GUI.Button(new Rect(0,0 + (100*i),100,50), content + i.ToString())){
					editor = (Editor)Instantiate(editorRef, Vector3.zero, Quaternion.identity);

					editor.setMapID(i);

					GAME_STATE = "Editor";
					cameraControl.setState("Editor");
				}
			}
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				
				GAME_STATE = "Menu";
			}

			break;

		case "ChooseRacetrack":
			for(int i=0;i<totalMaps;i++){
				if(PlayerPrefs.HasKey("Start"+i.ToString())){
					if(GUI.Button(new Rect(0,0 + (100*i),100,50), "Track:" + i.ToString())){
						raceTrack = (RaceTrack)Instantiate(raceTrackRef, Vector3.zero, Quaternion.identity);
						raceTrack.loadTrack(i);
						GAME_STATE = "RaceTrack";
						cameraControl.setState("Follow");
					}
				}
			}
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				
				GAME_STATE = "Menu";
			}
			
			break;
		
		case "RaceTrack":
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				raceTrack.Destroy();
				
				GAME_STATE = "Menu";
			}
			break;
		}
	}
}
