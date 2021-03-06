﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {

	private RaceTrack track;
	private AudioSource source;

	public List<WheelCollider> wheels;
	public List<GameObject> physicalWheels;

	private float maxTorque = 2500;

	private Rigidbody rg;

	private List<Vector3> waypoints;
	private int currentWaypoint = 0;

	private float dir;
	private float dir2;
	private float dir3;

	private float resetTimer = 0;

	public bool AI = true;
	public bool finished = false;

	public int lapsCompleted = 0;
	private int totalLaps;
	private int endPosition = 0;

	private List<float> lapTimes = new List<float>();
	private float roundTime = 0;
	private float totalTime = 0;

	private float waypointDistance = 15;

	void Awake(){
		track = GameObject.Find ("RaceTrack(Clone)").GetComponent<RaceTrack> ();
		source = GetComponent<AudioSource> ();
	}

	void Start(){

		rg = GetComponent<Rigidbody> ();
		Vector3 centerMass = new Vector3 (0, -0.8f, 0);
		rg.centerOfMass = centerMass;
	}

	// Update is called once per frame
	void Update () {
		if (waypoints != null) {

			updateWaypoints();


			if(AI){
				carResetter();
			}
			else if(Input.GetKeyDown(KeyCode.R)){
				resetCar();
			}

			soundHandler();

			roundTime+=Time.deltaTime;


		}
	}

	void FixedUpdate(){

		float maxSteering = 9 + (4 - (rg.velocity.magnitude/7));

		float torque = 0;
		float steering = 0;

		if (AI) {
			//steer towards the correct point
			Quaternion rotation = Quaternion.LookRotation(waypoints[currentWaypoint] - transform.position);
			
			dir = rotation.eulerAngles.y - transform.eulerAngles.y;
			dir2 = dir + 360;
			dir3 = dir - 360;
			float endDir = 0;

			//there are multiple options to take a turn, but to get the most efficient one, we make sure to check out all the angles and see which 
			//one is the shortest
			float max = 9999;
			if(Mathf.Abs(dir)<max){
				max = Mathf.Abs(dir);
				endDir = dir;
			}
			if(Mathf.Abs(dir2)<max){
				max = Mathf.Abs(dir2);
				endDir = dir2;
			}
			if(Mathf.Abs(dir3)<max){
				max = Mathf.Abs(dir3);
				endDir = dir3;
			}

			//set speed for AI
			torque = 1*maxTorque;

			//set a certain marge so the car doesn't correct every single frame
			if(endDir>5){
				steering = 2f*maxSteering;
			}
			else if(endDir<-5){
				steering = -2f*maxSteering;
			}
		} else {
			torque = Input.GetAxis ("Vertical") * maxTorque;
			steering = Input.GetAxis ("Horizontal") * maxSteering;
		}

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

	private void carResetter(){
		if(rg.velocity.magnitude<5){
			resetTimer += Time.deltaTime;
			if(resetTimer>3){
				resetCar();
				
				resetTimer = 0;
			}
		}
		else resetTimer = 0;
	}

	private void resetCar(){
		//reset speed
		rg.velocity = new Vector3(0,0,0);

		//set position
		int straightWaypoint = track.getLatestStraightPoint (currentWaypoint);
		if (straightWaypoint == 0) {
			straightWaypoint = waypoints.Count-1;
		}
		Vector3 resetPos = new Vector3();
		resetPos = waypoints [straightWaypoint];
		resetPos.y = 4;

		transform.position = resetPos;

		//set rotation
		Vector3 rot = track.resetCarRotation ((int)(resetPos.x/20), (int)(resetPos.z/20));
		transform.eulerAngles = rot;

	}

	private void soundHandler(){
		float newPitch = 1 + (rg.velocity.magnitude/40);
		if(newPitch>2)newPitch = 2;
		source.pitch = newPitch;
	}

	private void updateWaypoints(){
		if(Vector3.Distance(transform.position, waypoints[currentWaypoint])<waypointDistance){
			if(currentWaypoint < waypoints.Count-1){
				currentWaypoint ++;
			}
			else{
				currentWaypoint = 0;
				
				if(lapsCompleted<totalLaps){
					lapsCompleted++;
					
					//save laptime
					lapTimes.Add(roundTime);
					roundTime = 0;
					
					if(lapsCompleted == totalLaps){
						//calculate average time and end position
						foreach(float time in lapTimes)totalTime+=time;
						track.calculateEndPosition(this);
						finished = true;
					}
				}
			}
		}
	}

	public void setRandomValues(){
		maxTorque = 2000 + Random.value * 1000;
		waypointDistance = 11 + Random.value * 6;
	}

	public void setLaps(int laps){
		totalLaps = laps;
	}

	public void setEndPosition(int position){
		endPosition = position;
	}
	public int getPosition(){
		return endPosition;
	}

	public void setWaypoints(List<Vector3> _waypoints){
		waypoints = _waypoints;
	}

	public List<float> getRoundTimes(){
		return lapTimes;
	}

	public float getTotalTime(){
		return totalTime;
	}
}
