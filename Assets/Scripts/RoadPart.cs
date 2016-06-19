using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadPart : MonoBehaviour {
	
	private Renderer renderer;
	
	private int ID;
	private int rotation;
	private Vector2 position;
	
	private bool isStartPart = false;

	//every part needs to have two parts that it's attached to
	private List<RoadPart> connections = new List<RoadPart>();

	void Awake(){
		renderer = GetComponent<Renderer> ();
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

	public List<RoadPart> getConnections(){
		return connections;
	}
	
	void OnGUI(){
		if (connections.Count > 0) {
			//GUI.Box (new Rect (screenPos.x, Screen.height - screenPos.y, 60,20), connections.Count.ToString());
			//GUI.Box (new Rect (screenPos.x, Screen.height - screenPos.y + 20, 60,20), connections[1].transform.position.x.ToString() + " / " + connections[1].transform.position.z.ToString());
		}
	}
}