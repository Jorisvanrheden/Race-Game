using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Editor : MonoBehaviour {

	public Tile tile;

	private static int MAP_WIDTH = 10;
	private static int MAP_HEIGHT = 10;

	private Tile[,] map = new Tile[MAP_WIDTH,MAP_HEIGHT];
	public int mapID;

	public List<RoadPart> parts;
	private List<RoadPart> spawnedParts = new List<RoadPart>();

	private int selectedPart = 0;
	private RoadPart selectedPreview;
	private int previewRotation = 0;

	private RoadPart startPart;
	
	// Use this for initialization
	void Start () {

		spawnTiles ();

		updateSelectedPart ();

	}

	void Update(){
		handleRotations ();
	}

	public void loadTrack(){
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
		Vector2 tile_ref  = parser.getStartPart (mapID);
		setStartTile (map[(int)tile_ref.x,(int)tile_ref.y]);
	}

	private void spawnTiles(){
		for (int i=0; i<MAP_WIDTH; i++) {
			for (int j=0; j<MAP_HEIGHT; j++) {
				map[i,j] = (Tile)(Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity));
			}
		}
	}

	private void spawnLoadedPart(int x, int z, int ID, int rot){
		RoadPart newPart = (RoadPart)Instantiate (parts [ID], new Vector3 (x,1,z), Quaternion.Euler (0, rot, 0));
		spawnedParts.Add (newPart);
		//set some information that is needed to save the made track
		newPart.setID (ID);
		newPart.setRotation (rot);
		
		map[x,z].setOpen(false);
		
	}

	public void spawnPart(Tile origin){

		RoadPart newPart = (RoadPart)Instantiate (parts [selectedPart], new Vector3 (origin.transform.position.x, 1, origin.transform.position.z), Quaternion.Euler (0, previewRotation, 0));
		spawnedParts.Add (newPart);
		//set some information that is needed to save the made track
		newPart.setID (selectedPart);
		newPart.setRotation (previewRotation);

		origin.setOpen(false);


	}

	public void setStartTile(Tile tile){
		foreach (RoadPart p in spawnedParts) {
			if (p.transform.position.x == tile.transform.position.x) {
				if (p.transform.position.z == tile.transform.position.z) {

					//check if the selected part is the current start part
					//if not, unset the previous one and set the new one
					if(startPart == p){
						//no need to do anything, since it's already set as a start part
					}
					else{
						if(startPart!=null){
							//unset the previous part
							startPart.setStartPart(false);
						}
						//set the new part
						startPart = p;
						p.setStartPart (true);
					}
					break;
				}
			}
		}
	}

	public void deletePart(Tile origin){
		foreach (RoadPart p in spawnedParts) {
			if(p.transform.position.x == origin.transform.position.x){
				if(p.transform.position.z == origin.transform.position.z){

					//if its a start part, make sure to remove that as well
					if(startPart = p) startPart = null;

					spawnedParts.Remove(p);
					Destroy(p.gameObject);

					origin.setOpen(true);

					break;
				}
			}
		}
	}

	private void updateSelectedPart(){
		if (selectedPreview != null) {
			Destroy (selectedPreview.gameObject);
		}

		selectedPreview = (RoadPart)Instantiate (parts [selectedPart], new Vector3 (-1.1f, 2.34f, 1.7f), Quaternion.Euler (0, previewRotation, 0));
	}

	private void handleRotations(){
		if (Input.GetKeyDown (KeyCode.E)) {
			previewRotation += 90;
			if(previewRotation >= 360)previewRotation -=360;
			updateSelectedPart();
		}
		
		else if (Input.GetKeyDown (KeyCode.Q)) {
			previewRotation -= 90;
			if(previewRotation <= -360)previewRotation +=360;
			updateSelectedPart();
		}
	}

	void OnGUI(){
		if (GUI.Button (new Rect (0, 0, 50, 30), "Straight")) {
			selectedPart = 0;
			updateSelectedPart();
		}
		else if (GUI.Button (new Rect (0, 30, 50, 30), "Corner")) {
			selectedPart = 1;
			updateSelectedPart();
		}

		else if (GUI.Button (new Rect (0, 150, 50, 30), "SAVE")) {
			Save();
		}
		else if (GUI.Button (new Rect (0, 200, 50, 30), "Load")) {
			loadTrack();
		}
		else if (GUI.Button (new Rect (0, 250, 50, 30), "Clear all saves")) {
			PlayerPrefs.DeleteAll();
		}
	}

	public void setMapID(int ID){
		mapID = ID;
	}

	private void Save(){
		//make sure the file is empty first, so you can fill the file with new data, without keeping any of the older state
		for (int i=0; i<MAP_WIDTH; i++) {
			for (int j=0; j<MAP_HEIGHT; j++) {
				PlayerPrefs.DeleteKey(mapID.ToString()+"/"+i.ToString()+"/"+j.ToString());
			}
		}

		//save the actual data
		foreach (RoadPart p in spawnedParts) {
			//data structure will be: KEY = mapID/X/Z (position) VALUE =  ID of part / Rotation of part
			PlayerPrefs.SetString(mapID.ToString()+"/"+p.transform.position.x.ToString()+"/"+p.transform.position.z.ToString(), p.getID().ToString()+"/"+p.getRotation().ToString());
			PlayerPrefs.SetString("Start" + mapID.ToString(), startPart.transform.position.x.ToString() + "/" + startPart.transform.position.z.ToString());
		}
	}

	public void Destroy(){
		for (int i=0; i<MAP_WIDTH; i++) {
			for (int j=0; j<MAP_HEIGHT; j++) {
				Destroy(map[i,j].gameObject);
			}
		}
		foreach (RoadPart p in spawnedParts) {
			Destroy(p.gameObject);
		}
		if (selectedPreview != null) {
			Destroy(selectedPreview.gameObject);
		}
		Destroy (gameObject);
	}
}
