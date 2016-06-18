using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private string CAMERA_STATE = "Menu";

	private Vector3 startPosition;
	private Quaternion startRotation;

	private Transform target;

	void Awake(){
		startPosition = transform.position;
		startRotation = transform.rotation;
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

				Vector3 velocity = Vector3.zero;
				Vector3 forward = target.forward * 10.0f;

				Vector3 pos = target.position;
				//pos.z += 10;
				//pos.x += 5;
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
				
				Vector3 velocity = Vector3.zero;
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
			transform.position = startPosition;
			transform.rotation = startRotation;

		}
		CAMERA_STATE = state;
	}

	public void setTarget(Transform _target){
		target = _target;
	}
}
