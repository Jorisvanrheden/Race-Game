using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadPart : MonoBehaviour {
	
	private Renderer renderer;
	
	private int ID;
	private int rotation;
	private Vector2 position;
	
	private bool isStartPart = false;
	private Color startColor;
	
	//every part needs to have two parts that it's attached to
	private List<RoadPart> connections = new List<RoadPart>();
	
	private Vector3 screenPos;
	
	void Awake(){
		renderer = GetComponent<Renderer> ();
		startColor = renderer.material.color;
		
		screenPos = Camera.main.WorldToScreenPoint (transform.position);
	}
	
	public void setColor(Color color){
		if(!isStartPart)renderer.material.color = color;
	}
	
	public void removeConnection(RoadPart part){
		connections.Remove (part);
		if (connections.Count == 2) {
			setColor (Color.green);
		} else
			setColor (Color.red);
	}
	public void addConnection(RoadPart part){
		connections.Add (part);
		if (connections.Count == 2) {
			setColor (Color.green);
		} else
			setColor (Color.red);
	}
	
	public void setStartPart(bool start){
		isStartPart = start;
		if (start) {
			renderer.material.color = Color.blue;
		}
		if (connections.Count == 2) {
			setColor (Color.green);
		} else
			setColor (Color.red);
	}
	
	public bool getStartPart(){
		return isStartPart;
	}
	
	public void setID(int _ID){
		ID = _ID;
	}
	
	public int getID(){
		return ID;
	}
	
	public void setRotation(int _rotation){
		rotation = _rotation;
	}
	
	public int getRotation(){
		return rotation;
	}
	
	//for loading the actual part we place the values from the playerprefs file in this variable
	//this so we can access it from the editor class again
	public void setPosition(int x, int y){
		position = new Vector2 (x, y);
	}
	
	public Vector2 getPosition(){
		return position;
	}

	public List<RoadPart> getConnections(){
		return connections;
	}
	
	void OnGUI(){
		if (connections.Count > 0) {
			//GUI.Box (new Rect (screenPos.x, Screen.height - screenPos.y, 60,20), connections[0].transform.position.x.ToString() + " / " + connections[0].transform.position.z.ToString());
			//GUI.Box (new Rect (screenPos.x, Screen.height - screenPos.y + 20, 60,20), connections[1].transform.position.x.ToString() + " / " + connections[1].transform.position.z.ToString());
		}
	}
}