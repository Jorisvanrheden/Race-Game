using UnityEngine;
using System.Collections;

public class RoadPart : MonoBehaviour {

	private Renderer renderer;

	private int ID;
	private int rotation;
	private Vector2 position;

	private bool isStartPart = false;

	void Awake(){
		renderer = GetComponent<Renderer> ();
		renderer.material.color = Color.yellow;
	}

	public void setColor(Color color){
		renderer.material.color = color;
	}

	public void setStartPart(bool start){
		isStartPart = start;
		if (start) {
			setColor (Color.green);
		}
		else setColor (Color.yellow);
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
}