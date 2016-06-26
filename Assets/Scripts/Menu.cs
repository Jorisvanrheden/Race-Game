using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private CameraController cameraControl;
	private AudioController audio;

	public Editor editorRef;
	private Editor editor;

	public RaceTrack raceTrackRef;
	private RaceTrack raceTrack;

	public Preview previewRef;
	private Preview preview;

	private string GAME_STATE = "Menu";

	private int totalMaps = 5;
	private int trackSelectID = 99;

	public GameObject menuAccesories;

	private Vector3 startPos;

	public GUIStyle style;

	// Use this for initialization
	void Start () {
		cameraControl = Camera.main.GetComponent<CameraController> ();
		audio = Camera.main.GetComponent<AudioController> ();

		startPos = menuAccesories.transform.position;

		audio.playClip(0);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

		switch (GAME_STATE) {
		case "Menu":
			GUI.Box(new Rect(Screen.width/2 - 150, 50, 300, 100), "Racing Game", style);

			if(GUI.Button(new Rect(Screen.width - 250,450,200,100), "\nEditor")){
				GAME_STATE = "ChooseEditorMap";
				audio.playEffect(0);
			}
			else if(GUI.Button(new Rect(Screen.width - 250,300,200,100), "\nRacetrack")){
				GAME_STATE = "ChooseRacetrack";
				audio.playEffect(0);
			}
			else if(GUI.Button(new Rect(Screen.width - 250,650,200,100), "\nQuit game")){
				Application.Quit();
			}

			break;

		case "Editor":

			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				editor.Destroy();
				
				goToMenu();

				audio.playClip(0);
				audio.playEffect(0);
			}

			else if (GUI.Button (new Rect (Screen.width-100, Screen.height-50, 100,50), "Delete map")) {
				editor.clearMap ();
				editor.Destroy();

				GAME_STATE = "ChooseEditorMap";
				cameraControl.setState("Menu");

				audio.playClip(0);
				audio.playEffect(0);
			}

			break;

		case "ChooseEditorMap":
			GUI.Box(new Rect(Screen.width/2 - 150, 50, 300, 100), "Editor", style);
			for(int i=0;i<totalMaps;i++){

				string content = "";
				if(PlayerPrefs.HasKey("Start"+i.ToString())){
					content = "Edit Map: ";
				}
				else content = "EMPTY: ";

				if(GUI.Button(new Rect(0 + (230*i),180 ,200,100), content + (i+1).ToString())){
					editor = (Editor)Instantiate(editorRef, Vector3.zero, Quaternion.identity);

					editor.setMapID(i);

					GAME_STATE = "Editor";
					cameraControl.setState("Editor");

					audio.playClip(1);
					audio.playEffect(0);
				}
			}
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				
				goToMenu();
				audio.playEffect(0);
			}

			break;

		case "ChooseRacetrack":
			GUI.Box(new Rect(Screen.width/2 - 150, 50, 300, 100), "Tracks", style);

			for(int i=0;i<totalMaps;i++){
				if(PlayerPrefs.HasKey("Start"+i.ToString())){
					if(GUI.Button(new Rect(0 + (230*i),180 ,200,100), "Track:" + (i+1).ToString())){
						trackSelectID = i;

						if(preview!=null){
							//remove the old one
							Destroy(preview.gameObject);
						}
						preview = (Preview)Instantiate (previewRef, new Vector3(0,0,0), Quaternion.identity);
						preview.loadTrack (i);

						audio.playEffect(0);
					}
				}
			}
			if(trackSelectID!=99){
				if(GUI.Button(new Rect(Screen.width-550,0,250,150), "Race!")){
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

					audio.stopClip();
					audio.playEffect(0);
				}
			}
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				
				goToMenu();

				audio.playEffect(0);
			}
			
			break;
		
		case "RaceTrack":
			if(GUI.Button(new Rect(Screen.width-100,0,100,50), "Back to Menu")){
				raceTrack.Destroy();

				goToMenu();

				audio.playClip(0);
				audio.playEffect(0);
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
