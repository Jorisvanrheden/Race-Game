using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Editor : MonoBehaviour {

	public Tile tile;

	private static int MAP_WIDTH = 10;
	private static int MAP_HEIGHT = 10;

	private Tile[,] map = new Tile[MAP_WIDTH,MAP_HEIGHT];
	public int mapID;

	private string EDITOR_STATE = "Building";

	public GUIStyle style;

	public List<RoadPart> parts;
	private List<RoadPart> spawnedParts = new List<RoadPart>();

	private int selectedPart = 0;
	private RoadPart selectedPreview;
	private int previewRotation = 0;

	public Texture2D p0;
	public Texture2D p1;
	public Texture2D t0;
	public Texture2D t1;

	private RoadPart startPart;

	private bool ready = false;
	private AudioController audio;

	void Awake(){
		audio = Camera.main.GetComponent<AudioController> ();
	}
	
	// Use this for initialization
	void Start () {

		spawnTiles ();

		updateSelectedPart ();

	}

	void Update(){
		handleInput ();
	}

	private void loadTrack(){
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

		if (PlayerPrefs.GetString ("Start" + mapID.ToString ()).Length > 0) {
			Vector3 tile_ref  = parser.getStartPart (mapID);
			setStartTile (map[(int)tile_ref.x,(int)tile_ref.y]);
		}
	}

	private void spawnTiles(){
		for (int i=0; i<MAP_WIDTH; i++) {
			for (int j=0; j<MAP_HEIGHT; j++) {
				map[i,j] = (Tile)(Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity));
			}
		}

		loadTrack ();
	}

	private void spawnLoadedPart(int x, int z, int ID, int rot){
		RoadPart newPart = (RoadPart)Instantiate (parts [ID], new Vector3 (x,0.3f,z), Quaternion.Euler (0, rot, 0));
		spawnedParts.Add (newPart);
		//set some information that is needed to save the made track
		newPart.setID (ID);
		newPart.setRotation (rot);

		//find and save the neighbouring parts to be able to determine if the track is connected completely
		foreach (RoadPart connection in findConnectingParts(newPart)) {
			newPart.addConnection(connection);
			connection.addConnection(newPart);
		}
		
		map[x,z].setOpen(false);

		checkCompleteTrack ();
		
	}

	public void spawnPart(Tile origin){

		RoadPart newPart = (RoadPart)Instantiate (parts [selectedPart], new Vector3 (origin.transform.position.x, 0.3f, origin.transform.position.z), Quaternion.Euler (0, previewRotation, 0));
		spawnedParts.Add (newPart);
		//set some information that is needed to save the made track
		newPart.setID (selectedPart);
		newPart.setRotation (previewRotation);

		//find and save the neighbouring parts to be able to determine if the track is connected completely
		foreach (RoadPart connection in findConnectingParts(newPart)) {
			newPart.addConnection(connection);
			connection.addConnection(newPart);
		}

		origin.setOpen(false);

		checkCompleteTrack ();

		audio.playEffect (1);

	}

	private List<RoadPart> findConnectingParts(RoadPart part){
		Vector3 pos = part.transform.position;
		List<RoadPart> connectingParts = new List<RoadPart> ();
		List<Vector3> posRef = new List<Vector3> ();
		int rot = part.getRotation ();
		//straight part
		if (part.getID () == 0) {
			if(rot == 0 || rot == 180){
				//add one tile higher
				posRef.Add(new Vector3(pos.x, pos.y, pos.z + 1));
				//and one tile lower
				posRef.Add(new Vector3(pos.x, pos.y, pos.z - 1));
			}
			else if(rot == 90 || rot == 270){
				//add one tile to the right
				posRef.Add(new Vector3(pos.x + 1, pos.y, pos.z));
				//and one tile to the left
				posRef.Add(new Vector3(pos.x - 1, pos.y, pos.z));
			}
		}
		//corner part
		else if (part.getID () == 1) {
			//bottom, right
			if(rot == 0){
				posRef.Add(new Vector3(pos.x, pos.y, pos.z - 1));
				posRef.Add(new Vector3(pos.x + 1, pos.y, pos.z));
			}
			//bottom,left
			else if(rot == 90){
				posRef.Add(new Vector3(pos.x, pos.y, pos.z - 1));
				posRef.Add(new Vector3(pos.x - 1, pos.y, pos.z));
			}
			//left, top
			else if(rot == 180){
				posRef.Add(new Vector3(pos.x, pos.y, pos.z + 1));
				posRef.Add(new Vector3(pos.x - 1, pos.y, pos.z));
			}
			//top, right
			else if(rot == 270){
				posRef.Add(new Vector3(pos.x, pos.y, pos.z + 1));
				posRef.Add(new Vector3(pos.x + 1, pos.y, pos.z));
			}
		}
		//check if there are actually parts on these positions
		foreach (RoadPart p in spawnedParts) {
			foreach(Vector3 v in posRef){
				if (p.transform.position.x == v.x) {
					if (p.transform.position.z == v.z) {
						connectingParts.Add(p);
					}
				}
			}
		}
		return connectingParts;
	}

	public void setStartTile(Tile tile){
		foreach (RoadPart p in spawnedParts) {
			if (p.transform.position.x == tile.transform.position.x) {
				if (p.transform.position.z == tile.transform.position.z) {
					//only for straight parts
					if(p.getID() == 0){
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
	}

	public void deletePart(Tile origin){
		foreach (RoadPart p in spawnedParts) {
			if(p.transform.position.x == origin.transform.position.x){
				if(p.transform.position.z == origin.transform.position.z){

					//remove the internal connection between the two parts
					foreach(RoadPart part in findConnectingParts(p)){
						part.removeConnection(p);
					}

					//if its a start part, make sure to remove that as well
					if(startPart!=null){
						if(startPart.transform.position.x == p.transform.position.x){
							if(startPart.transform.position.z == p.transform.position.z){
								startPart = null;
							}
						}
					}
						
					spawnedParts.Remove(p);
					Destroy(p.gameObject);

					origin.setOpen(true);

					audio.playEffect (2);

					break;
				}
			}
		}

		checkCompleteTrack ();
	}

	private void updateSelectedPart(){
		if (selectedPreview != null) {
			Destroy (selectedPreview.gameObject);
		}

		selectedPreview = (RoadPart)Instantiate (parts [selectedPart], new Vector3 (0.7f, 5.44f, 2.74f), Quaternion.Euler (0, previewRotation, 0));
	}

	private void handleInput(){
		if (Input.GetKeyDown (KeyCode.E)) {
			previewRotation += 90;
			if(previewRotation >= 360)previewRotation -=360;
			updateSelectedPart();
		}
		
		else if (Input.GetKeyDown (KeyCode.Q)) {
			previewRotation -= 90;
			if(previewRotation < 0)previewRotation +=360;
			updateSelectedPart();
		}

		else if (Input.GetKeyDown (KeyCode.Alpha1)) {
			selectedPart = 0;
			updateSelectedPart();
		}

		else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			selectedPart = 1;
			updateSelectedPart();
		}

		else if (Input.GetKeyDown (KeyCode.Tab)) {
			if(selectedPart==0)selectedPart=1;
			else if(selectedPart == 1)selectedPart=0;
			updateSelectedPart();
		}
	}

	private void checkCompleteTrack(){
		foreach(RoadPart p in spawnedParts){
			if(p.getConnections().Count >= 2){
				ready = true;
			}
			else{
				ready = false;
				break;
			}
		}
	}

	void OnGUI(){
		switch (EDITOR_STATE) {
		case "Building":

			GUI.Box(new Rect(20, 0, 230, 40), "Available parts:");
			if (GUI.Button (new Rect (20, 50, 100, 100), p0)) {
				selectedPart = 0;
				updateSelectedPart ();
			} else if (GUI.Button (new Rect (150, 50, 100, 100), p1)) {
				selectedPart = 1;
				updateSelectedPart ();
			} 

			if (GUI.Button (new Rect (20, Screen.height - 600, 100, 100), t0)) {
				previewRotation -= 90;
				if(previewRotation < 0)previewRotation +=360;
				updateSelectedPart();
			}
			else if(GUI.Button (new Rect (150, Screen.height - 600, 100, 100), t1)) {
				previewRotation += 90;
				if(previewRotation >= 360)previewRotation -=360;
				updateSelectedPart();
			}

			GUI.Box(new Rect(20, 170, 230, 80), "\n\nPress TAB to switch part");
			GUI.Box(new Rect(20, Screen.height - 480, 230, 80), "\n\nPress E/Q to rotate the part");
			GUI.Box(new Rect(20, Screen.height - 370, 400, 80), "\n\nCurrent: ");


			string toDoBeforeSave = "TO DO before saving:";
			if (ready && startPart!=null){
				if (GUI.Button (new Rect (Screen.width-300, 500, 300,150), "SAVE")) {
					Save ();
				}
				toDoBeforeSave = "TO DO before save:" + "\nAll ready!";
			}
			GUI.Box(new Rect(Screen.width-300, 100, 300, 50), toDoBeforeSave);
			if(startPart == null)GUI.Box(new Rect(Screen.width-300, 260, 300, 60), "\nRight-mouseclick on a straight part to \nselect the starting position");
			if(!ready)GUI.Box(new Rect(Screen.width-300, 200, 300, 60), "\nMake the track completely circular!");

			break;
		}
	}

	public void setMapID(int ID){
		mapID = ID;
	}

	public void clearMap(){
		//make sure the file is empty first, so you can fill the file with new data, without keeping any of the older state
		for (int i=0; i<MAP_WIDTH; i++) {
			for (int j=0; j<MAP_HEIGHT; j++) {
				PlayerPrefs.DeleteKey(mapID.ToString()+"/"+i.ToString()+"/"+j.ToString());
			}
		}
		for (int i=0; i<MAP_WIDTH*MAP_HEIGHT; i++) {
			PlayerPrefs.DeleteKey("Path" + mapID.ToString() + "/" + i.ToString());
		}
		PlayerPrefs.DeleteKey ("Start" + mapID.ToString ());
	}

	private void Save(){
		clearMap ();

		//save the actual data
		foreach (RoadPart p in spawnedParts) {
			//data structure will be: KEY = mapID/X/Z (position) VALUE =  ID of part / Rotation of part
			PlayerPrefs.SetString(mapID.ToString()+"/"+p.transform.position.x.ToString()+"/"+p.transform.position.z.ToString(), p.getID().ToString()+"/"+p.getRotation().ToString());
		}
		PlayerPrefs.SetString("Start" + mapID.ToString(), startPart.transform.position.x.ToString() + "/" + startPart.transform.position.z.ToString()+"/"+startPart.getRotation().ToString());

		int partIndex = 0;
		foreach(RoadPart part in determinePath()){
			PlayerPrefs.SetString("Path" + mapID.ToString() + "/" + partIndex.ToString(), part.transform.position.x.ToString() + "/" + part.transform.position.z.ToString());
			partIndex++;
		}
	}

	private List<RoadPart> determinePath(){
		List<RoadPart> route = new List<RoadPart> ();

		RoadPart oldPart = startPart;
		RoadPart newPart = null;

		int rot = startPart.getRotation ();
		Vector3 dir = new Vector3 (0, 0, 0);
		if (rot == 0) {
			dir = new Vector3(0,0,1);
		}
		else if (rot == 90) {
			dir = new Vector3(1,0,0);
		}
		else if (rot == 180) {
			dir = new Vector3(0,0,-1);
		}
		else if (rot == 270) {
			dir = new Vector3(-1,0,0);
		}

		foreach(RoadPart p in startPart.getConnections()){
			if(p.transform.position.x == (startPart.transform.position+dir).x){
				if(p.transform.position.z == (startPart.transform.position+dir).z){
					newPart = p;
				}
			}
		}

		route.Add(newPart);

		for (int i=0; i<spawnedParts.Count-1; i++) {
			foreach(RoadPart p in newPart.getConnections()){
				if(p!=oldPart){
					route.Add(p);

					oldPart = newPart;
					newPart = p;

					break;
				}
			}
		}
		return route;
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
