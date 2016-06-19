using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private string CAMERA_STATE = "Menu";

	private Vector3 menuPosition;
	private Vector3 menuRotation;

	private Vector3 editorPosition = new Vector3(3.9f, 9.8f, 4.46f);
	private Vector3 editorRotation = new Vector3 (90, 0, 0);

	private Transform target;

	void Awake(){
		menuPosition = Camera.main.transform.position;
		menuRotation = Camera.main.transform.rotation.eulerAngles;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		switch (CAMERA_STATE) {

		case "Menu":

			break;

		case "Follow":
			if(target!=null){

				Vector3 forward = target.forward * 10.0f;

				Vector3 pos = target.position;
				pos -= forward;
				pos.y += 5;

				transform.position = pos;
				transform.LookAt (target);

				//transform.position = Vector3.SmoothDamp(transform.position, pos, 
				   //                                     ref velocity,0.05f);


			}
			break;

		case "EndRace":
			if(target!=null){
				
				Vector3 forward = target.forward * 10.0f;
				
				Vector3 pos = target.position;
				pos.z += 15;
				pos.x += 10;
				pos -= forward;
				pos.y += 10;
				
				transform.position = pos;
				transform.LookAt (target);
				
				//transform.position = Vector3.SmoothDamp(transform.position, pos, 
				//                                     ref velocity,0.05f);
				
				
			}
			break;

		case "Editor":

			break;
		
		}
	}

	public void setState(string state){
		if (state == "Editor") {
			transform.position = editorPosition;
			transform.eulerAngles = editorRotation;

		}
		else if (state == "Menu") {
			transform.position = menuPosition;
			transform.eulerAngles = menuRotation;	
		}
		else if (state == "RaceTrack") {
			
			
		}
		CAMERA_STATE = state;
	}

	public void setTarget(Transform _target){
		target = _target;
	}
}
