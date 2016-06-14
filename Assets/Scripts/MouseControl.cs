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
		if (Physics.Raycast (ray, out hit)) {
			Tile tile = hit.transform.gameObject.GetComponent<Tile>();
			if(tile!=null){
				tile.mouseOver();
			}
		}

		if(Input.GetMouseButtonUp(0)){
			if (Physics.Raycast (ray, out hit)) {
				Tile tile = hit.transform.gameObject.GetComponent<Tile>();
				if(tile.getOpen()){
					editor.spawnPart(tile);
				}
				else{
					editor.deletePart(tile);
				}
			}
		}
		else if(Input.GetMouseButtonUp(1)){
			if (Physics.Raycast (ray, out hit)) {
				Tile tile = hit.transform.gameObject.GetComponent<Tile>();
				if(!tile.getOpen()){
					editor.setStartTile(tile);
				}
			}
		}
	}
}
