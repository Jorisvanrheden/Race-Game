using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	private Renderer renderer;

	private bool open = true;

	void Awake(){
		renderer = GetComponent<Renderer> ();

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void mouseOver(){
		renderer.material.color = Color.blue;
	}
	void OnMouseExit(){
		renderer.material.color = Color.white;
	}

	public void setOpen(bool _open){
		open = _open;
	}

	public bool getOpen(){
		return open;
	}


}
