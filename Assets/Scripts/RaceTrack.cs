using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceTrack : MonoBehaviour {

	public int mapID;

	public GameObject flag;
	private GameObject flagHolder;

	public List<RoadPart> parts;
	private List<RoadPart> spawnedParts = new List<RoadPart>();

	private static int MAP_WIDTH = 10;
	private static int MAP_HEIGHT = 10;

	public CarController carRef;
	private List<CarController> cars = new List<CarController> ();

	private List<Vector3> waypoints;
	private int totalLaps = 3;
	
	private string RACE_STATE;

	private CameraController cameraControl;

	void Awake(){
		cameraControl = Camera.main.GetComponent<CameraController> ();

	}
	// Use this for initialization
	void Start () {

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

		flagHolder = (GameObject)Instantiate(flag, new Vector3 (tile_ref.x * 20, 10f, tile_ref.y * 20), Quaternion.Euler(new Vector3(0,tile_ref.z+90, 0)));

		Vector2 dir = new Vector2 (0, 0);
		if(tile_ref.z == 90)dir = new Vector2(1,0);
		else if(tile_ref.z == 180)dir = new Vector2(0,-1);
		else if(tile_ref.z == 270)dir = new Vector2(-1,0);
		else if(tile_ref.z == 0)dir = new Vector2(0,1);

		CarController car = (CarController)Instantiate (carRef, new Vector3 (tile_ref.x * 20, 2, tile_ref.y * 20), Quaternion.Euler(new Vector3(0,tile_ref.z, 0)));
		cars.Add (car);
		car.AI = false;
		car.setLaps (totalLaps);
		car.setWaypoints (waypoints);

		spawnCar (tile_ref.x - (0.15f*dir.y), tile_ref.y - (0.15f*dir.x), tile_ref.z, totalLaps);
		spawnCar (tile_ref.x + (0.15f*dir.y), tile_ref.y + (0.15f*dir.x), tile_ref.z, totalLaps);
		spawnCar (tile_ref.x - (0.30f*dir.y), tile_ref.y - (0.30f*dir.x), tile_ref.z, totalLaps);
		spawnCar (tile_ref.x + (0.30f*dir.y), tile_ref.y + (0.30f*dir.x), tile_ref.z, totalLaps);

		cameraControl.setTarget (car.transform);
	}

	private void spawnCar(float x, float z, float rot, int laps){
		CarController car = (CarController)Instantiate (carRef, new Vector3 (x * 20, 2, z * 20), Quaternion.Euler(new Vector3(0,rot, 0)));
		cars.Add (car);
		car.AI = true;
		car.setLaps (laps);
		car.setRandomValues ();
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
		Destroy (flagHolder.gameObject);
		Destroy (gameObject);
	}

	public void setState(string state){
		RACE_STATE = state;
	}

	public int getLatestStraightPoint (int current){
		Parser parser = new Parser ();

		if (current == 0)
			return current;

		if (parser.getSaveFile (mapID, (int)waypoints [current].x / 20, (int)waypoints [current].z / 20).ID == 1) {
			return getLatestStraightPoint(current-1);
		}
		else{
			return current;
		}
	}

	public Vector3 resetCarRotation(int x, int y){
		Parser parser = new Parser ();

		Vector3 rot = new Vector3 (0, 0, 0);
		rot.y = parser.getSaveFile (mapID, x, y).rotation;

		return rot;
	}

	public void calculateEndPosition(CarController _car){
		//identify what car on the field is the passed through reference car
		int carIndex = 0;
		for (int i=0; i<cars.Count; i++) {
			if(cars[i] == _car)carIndex = i;
		}

		//save all times to compare with and determine your position
		List<float> positions = new List<float> ();
		for(int i=0;i<cars.Count;i++){
			positions.Add(cars[i].getTotalTime());
		}

		//calculate end position
		int finalPosition = 1;
		for(int i=0;i<positions.Count;i++){
			if(i!=carIndex){
				if(positions[carIndex]>positions[i]){
					if(positions[i]!=0)finalPosition++;
				}
			}
		}
		//set the end position
		cars [carIndex].setEndPosition (finalPosition);
	}

	void OnGUI(){
		switch (RACE_STATE) {
		case "Racing":

			GUI.Box(new Rect(0,0, 200,100), "\n\nLAP " + (cars[0].lapsCompleted+1).ToString() + "/" + totalLaps.ToString());

			GUI.Box(new Rect(0,150, 200,50), "\nPress 'R' to reset your car!");

			bool finished = true;
			for(int i=0;i<cars.Count;i++){
				if(!cars[i].finished){
					finished = false;
				}
			}

			//make an exception for you own car if you're done already
			if(cars[0].finished)finished = true;

			if(finished){
				cars[0].AI = true;
				cameraControl.setState("EndRace");
				RACE_STATE = "Score";
			}

			break;
		case "Score":
			for(int i=0;i<cars.Count;i++){
				if(cars[i].getPosition()!=0)GUI.Box(new Rect(i*200, 0, 200, 30), "Position: " + cars[i].getPosition());

				GUI.Box(new Rect(i*200, 30, 200, 30), "Car " + (i+1).ToString() + " total: " + Mathf.Round(cars[i].getTotalTime() * 100)/100 + " seconds");

				for(int j=0;j<cars[i].getRoundTimes().Count;j++){
					GUI.Box(new Rect(i*200, 70 + j*30, 200, 30), "Round " + (j+1).ToString() + ": " + Mathf.Round(cars[i].getRoundTimes()[j] * 100)/100 +" seconds");
				}
			}

			GUI.Box(new Rect(Screen.width/2-150, Screen.height - 100, 300, 100), "\n\nYou finished in place " + cars[0].getPosition());

			break;
		}
	}
}
