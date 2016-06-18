using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceTrack : MonoBehaviour {

	public int mapID;

	public List<RoadPart> parts;
	private List<RoadPart> spawnedParts = new List<RoadPart>();

	private static int MAP_WIDTH = 10;
	private static int MAP_HEIGHT = 10;

	public CarController carRef;
	private List<CarController> cars = new List<CarController> ();

	private List<Vector3> waypoints;
	private int totalLaps = 3;

	private int index = 0;

	private string RACE_STATE;

	private CameraController cameraControl;

	void Awake(){
		cameraControl = Camera.main.GetComponent<CameraController> ();

	}
	// Use this for initialization
	void Start () {
		//loadTrack ();
	}

	void Update(){

	}
	
	public void loadTrack(int _mapID){
		mapID = _mapID;
		//get save state from the parser
		Parser parser = new Parser ();
		for (int i=0; i<MAP_WIDTH; i++) {
			for (int j=0; j<MAP_HEIGHT; j++) {
				if(parser.containsData(mapID,i,j)){
					SaveData saveFile = parser.getSaveFile(mapID,i,j);
					spawnLoadedPart(i,j, saveFile.ID, saveFile.rotation);
				}
			}
		}

		waypoints = parser.getWaypoints (mapID, MAP_WIDTH, MAP_HEIGHT);

		Vector3 tile_ref = parser.getStartPart (mapID);

		CarController car = (CarController)Instantiate (carRef, new Vector3 (tile_ref.x * 20, 2, tile_ref.y * 20), Quaternion.Euler(new Vector3(0,tile_ref.z, 0)));
		cars.Add (car);
		car.AI = false;
		car.setLaps (totalLaps);
		car.setWaypoints (waypoints);

		spawnCar (tile_ref.x - 0.1f, tile_ref.y - 0.1f, tile_ref.z, totalLaps);
		spawnCar (tile_ref.x + 0.1f, tile_ref.y - 0.1f, tile_ref.z, totalLaps);
		spawnCar (tile_ref.x - 0.1f, tile_ref.y - 0.5f, tile_ref.z, totalLaps);
		spawnCar (tile_ref.x + 0.1f, tile_ref.y - 0.5f, tile_ref.z, totalLaps);

		cameraControl.setTarget (car.transform);
	}

	private void spawnCar(float x, float z, float rot, int laps){
		CarController car = (CarController)Instantiate (carRef, new Vector3 (x * 20, 2, z * 20), Quaternion.Euler(new Vector3(0,rot, 0)));
		cars.Add (car);
		car.AI = true;
		car.setLaps (laps);
		car.setRandomTorque ();
		car.setWaypoints (waypoints);
	}

	private void spawnLoadedPart(int x, int z, int ID, int rot){
		RoadPart newPart = (RoadPart)Instantiate (parts [ID], new Vector3 (x*20,1,z*20), Quaternion.Euler (0, rot, 0));
		spawnedParts.Add (newPart);
	}

	public void Destroy(){

		foreach (RoadPart p in spawnedParts) {
			Destroy(p.gameObject);
		}
		foreach (CarController car in cars) {
			Destroy (car.gameObject);
		}
		Destroy (gameObject);
	}

	public void setState(string state){
		RACE_STATE = state;
	}

	void OnGUI(){
		switch (RACE_STATE) {
		case "Racing":
			bool finished = true;
			for(int i=0;i<cars.Count;i++){
				GUI.Box(new Rect(0,i*30, 100,30), "Car " + i.ToString() + " - LAP " + cars[i].lapsCompleted.ToString() + "/3");
				if(!cars[i].finished){
					finished = false;
				}
			}
			if(finished){
				cars[0].AI = true;
				cameraControl.setState("EndRace");
				RACE_STATE = "Score";
			}

			break;
		case "Score":
			for(int i=0;i<cars.Count;i++){
				GUI.Box(new Rect(i*200, 0, 200, 30), "Car " + i.ToString() + " total: " + Mathf.Round(cars[i].getTotalTime() * 100f / 100f) + " seconds");
				for(int j=0;j<cars[i].getRoundTimes().Count;j++){
					GUI.Box(new Rect(i*200, 50 + j*30, 200, 30), "Round " + j.ToString() + ": " + Mathf.Round(cars[i].getRoundTimes()[j] * 100f / 100f).ToString()+" seconds");
				}
			}
			break;
		}
	}
}
