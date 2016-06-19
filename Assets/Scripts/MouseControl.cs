using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {

	private Editor editor;

	void Awake(){
		editor = GetComponent<Editor> ();

	}
		
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		//mouse hovering over the tiles and light up
		if (Physics.Raycast (ray, out hit)) {
			Tile tile = hit.transform.gameObject.GetComponent<Tile>();
			if(tile!=null){
				tile.mouseOver();
			}
		}
		//spawning and removing parts
		if(Input.GetMouseButtonUp(0)){
			if (Physics.Raycast (ray, out hit)) {
				Tile tile = hit.transform.gameObject.GetComponent<Tile>();
				if(tile!=null){
					if(tile.getOpen()){
						editor.spawnPart(tile);
					}
					else{
						editor.deletePart(tile);
					}
				}
			}
		}
		//setting the start tile
		else if(Input.GetMouseButtonUp(1)){
			if (Physics.Raycast (ray, out hit)) {
				Tile tile = hit.transform.gameObject.GetComponent<Tile>();
				if(tile!=null){
					if(!tile.getOpen()){
						editor.setStartTile(tile);
					}
				}
			}
		}
	}
}
