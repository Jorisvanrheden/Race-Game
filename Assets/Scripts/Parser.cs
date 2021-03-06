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

	public Vector3 getStartPart(int mapID){
		Vector3 tile_ref = new Vector3 ();
		string data = PlayerPrefs.GetString ("Start" + mapID.ToString());

		int dash_1 = 99;
		int dash_2 = 99;

		for (int i=0; i<data.Length; i++) {
			if(data[i].ToString() == "/"){
				if(dash_1 == 99){
					dash_1 = i;
				}
				else dash_2 = i;
			}
		}
		tile_ref.x = int.Parse(data.Substring(0,dash_1));
		tile_ref.y = int.Parse(data.Substring(dash_1+1,dash_2-1 - dash_1));
		tile_ref.z = int.Parse (data.Substring (dash_2 + 1, data.Length - 1 - dash_2));

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
						waypoint.y = 2.3f;
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
