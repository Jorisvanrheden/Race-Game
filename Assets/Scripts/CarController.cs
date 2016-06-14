using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {

	public List<WheelCollider> wheels;
	public List<GameObject> physicalWheels;

	private Rigidbody rg;

	void Start(){

		rg = GetComponent<Rigidbody> ();
		Vector3 centerMass = new Vector3 (0, -0.8f, 0);
		rg.centerOfMass = centerMass;

	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){

		int maxTorque = 2500;
		float maxSteering = 9 + (4 - (rg.velocity.magnitude/7));

		float torque = Input.GetAxis ("Vertical") * maxTorque;
		float steering = Input.GetAxis ("Horizontal") * maxSteering;


		wheels[0].motorTorque = torque;
		wheels[1].motorTorque = torque;

		wheels[0].steerAngle = steering;
		wheels[1].steerAngle = steering;

		if (Input.GetKey (KeyCode.Space)) {
			wheels [2].brakeTorque = 2000;
			wheels [3].brakeTorque = 2000;
		} 
		else {
			wheels[2].brakeTorque = 0;
			wheels[3].brakeTorque = 0;
		}


		foreach (GameObject pw in physicalWheels) {
		
			pw.transform.Rotate (new Vector3 (-(torque/500 + rg.velocity.magnitude), 0, 0));
		}


	}
}
