using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Preview : MonoBehaviour {

	private List<RoadPart> spawnedParts = new List<RoadPart> ();
	public List<RoadPart> parts = new List<RoadPart>();
	
	void Update(){
		transform.Rotate (Vector3.up*Time.deltaTime*5);
	}

	public void loadTrack(int mapID){
		//get save state from the parser
		Parser parser = new Parser ();
		for (int i=0; i<10; i++) {
			for (int j=0; j<10; j++) {
				if(parser.containsData(mapID,i,j)){
					SaveData saveFile = parser.getSaveFile(mapID,i,j);
					spawnLoadedPart(i,j, saveFile.ID, saveFile.rotation);
				}
			}
		}

		transform.position = new Vector3 (-97, 8.5f, 146.5f);
	}

	private void spawnLoadedPart(int x, int z, int ID, int rot){
		RoadPart newPart = (RoadPart)Instantiate (parts [ID], new Vector3 (x-5.5f,1,z-5.5f), Quaternion.Euler (0, rot, 0));
		newPart.transform.parent = transform;
		spawnedParts.Add (newPart);
	}

	public void Destroy(){
		foreach (RoadPart p in spawnedParts) {
			Destroy(p.gameObject);
		}
	}
}
