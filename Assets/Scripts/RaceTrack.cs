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
	private CarController car;

	// Use this for initialization
	void Start () {

		//loadTrack ();

		CameraController cameraControl = Camera.main.GetComponent<CameraController> ();
		cameraControl.setTarget (car.transform);
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

		Vector2 tile_ref = parser.getStartPart (mapID);
		car = (CarController)Instantiate (carRef, new Vector3 (tile_ref.x * 20, 2, tile_ref.y * 20), Quaternion.identity);
	}

	private void spawnLoadedPart(int x, int z, int ID, int rot){
		RoadPart newPart = (RoadPart)Instantiate (parts [ID], new Vector3 (x*20,1,z*20), Quaternion.Euler (0, rot, 0));
		spawnedParts.Add (newPart);
	}

	public void Destroy(){

		foreach (RoadPart p in spawnedParts) {
			Destroy(p.gameObject);
		}
		Destroy (car.gameObject);
		Destroy (gameObject);
	}
}
