using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parser {

	public SaveData getSaveFile(int mapID, int x, int z){

		SaveData saveFile = new SaveData ();

		string data = PlayerPrefs.GetString (mapID.ToString()+"/"+x.ToString () + "/" + z.ToString ());
		string newIDString = "";
		string newRotationString = "";

		for (int i=0; i<data.Length; i++) {
			if(data[i].ToString() == "/"){
				newIDString = data.Substring(0,i);
				//continue parsing the second half from this point as well
				//start from i+1 in order to avoid the "/". Length - 1 for last item, minus i gives you the remaining chars
				newRotationString = data.Substring(i+1,data.Length-i-1);
				
				break;
			}
		}
		saveFile.ID = int.Parse (newIDString);
		saveFile.rotation = int.Parse (newRotationString);
		
		return saveFile;
		
	}

	public Vector2 getStartPart(int mapID){
		Vector2 tile_ref = new Vector2 ();
		string data = PlayerPrefs.GetString ("Start" + mapID.ToString());

		for (int i=0; i<data.Length; i++) {
			if(data[i].ToString() == "/"){
				tile_ref.x = int.Parse(data.Substring(0,i));
				tile_ref.y = int.Parse(data.Substring(i+1,data.Length-1-i));
				break;
			}
		}
		return tile_ref;
	}

	public List<Vector3> getWaypoints(int mapID, int width, int height){
		List<Vector3> path = new List<Vector3> ();
		int maxPossibleParts = width * height;

		string data = "";

		for (int i=0; i<maxPossibleParts; i++) {
			//don't parse values that don't have any data in them
			if(PlayerPrefs.GetString("Path" + mapID.ToString() + "/" + i.ToString()).Length>0){
				Vector3 waypoint = new Vector3();
				data = PlayerPrefs.GetString("Path" + mapID.ToString() + "/" + i.ToString());

				for (int j=0; j<data.Length; j++) {
					if(data[j].ToString() == "/"){
						waypoint.x = int.Parse(data.Substring(0,j))*20;
						waypoint.y = 1;
						waypoint.z = int.Parse(data.Substring(j+1,data.Length-1-j))*20;

						path.Add(waypoint);

						break;
					}
				}
			}
		}
		return path;
	}
	
	public bool containsData(int mapID, int x, int z){

		if (PlayerPrefs.GetString (mapID.ToString()+"/"+x.ToString () + "/" + z.ToString ()).Length > 0) {
			return true;
		}
		return false;
	}
}
