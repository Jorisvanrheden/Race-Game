using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private CameraController cameraControl;

	public Editor editorRef;
	private Editor editor;

	public RaceTrack raceTrackRef;
	private RaceTrack raceTrack;

	public Preview previewRef;
	private Preview preview;

	private string GAME_STATE = "Menu";

	private int totalMaps = 3;
	private int trackSelectID = 99;

	public GameObject menuAccesories;

	private Vector3 startPos;

	public GUIStyle style;

	// Use this for initialization
	void Start () {
		cameraControl = Camera.main.GetComponent<CameraController> ();

		startPos = menuAccesories.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

		switch (GAME_STATE) {
		case "Menu":
			GUI.Box(new Rect(Screen.width/2 - 150, 50, 300, 100), "Racing Game", style);


			if(GUI.Button(new Rect(Screen.width - 250,300,200,100), "\nEditor")){
				GAME_STATE = "ChooseEditorMap";
			}
			else if(GUI.Button(new Rect(Screen.width - 250,450,200,100), "\nRacetrack")){
				GAME_STATE = "ChooseRacetrack";
			}
			break;

		case "Editor":
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				editor.Destroy();
				
				goToMenu();
			}

			else if (GUI.Button (new Rect (0, 300, 200,150), "Clear map")) {
				editor.clearMap ();
				editor.Destroy();

				GAME_STATE = "ChooseEditorMap";
				cameraControl.setState("Menu");
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
				
				goToMenu();
			}

			break;

		case "ChooseRacetrack":
			for(int i=0;i<totalMaps;i++){
				if(PlayerPrefs.HasKey("Start"+i.ToString())){
					if(GUI.Button(new Rect(0,0 + (100*i),100,50), "Track:" + i.ToString())){
						trackSelectID = i;

						if(preview!=null){
							//remove the old one
							Destroy(preview.gameObject);
						}
						preview = (Preview)Instantiate (previewRef, new Vector3(0,0,0), Quaternion.identity);
						preview.loadTrack (i);
					}
				}
			}
			if(trackSelectID!=99){
				if(GUI.Button(new Rect(Screen.width-300,0,150,100), "Race!")){
					//hide the menu items
					menuAccesories.transform.position = new Vector3(0,-10,0);
					if(preview!=null){
						//remove the old one
						Destroy(preview.gameObject);
					}

					raceTrack = (RaceTrack)Instantiate(raceTrackRef, Vector3.zero, Quaternion.identity);
					raceTrack.loadTrack(trackSelectID);
					raceTrack.setState("Racing");
					
					GAME_STATE = "RaceTrack";
					cameraControl.setState("Follow");
				}
			}
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				
				goToMenu();
			}
			
			break;
		
		case "RaceTrack":
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				raceTrack.Destroy();

				goToMenu();
			}
			break;
		}
	}

	private void goToMenu(){
		GAME_STATE = "Menu";
		cameraControl.setState("Menu");

		if (preview != null) {
			Destroy(preview.gameObject);
		}
		trackSelectID = 99;
		
		menuAccesories.transform.position = startPos;
	}
}
