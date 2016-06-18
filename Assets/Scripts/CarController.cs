using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {

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

	private List<float> lapTimes = new List<float>();
	private float roundTime = 0;
	private float totalTime = 0;

	void Start(){

		rg = GetComponent<Rigidbody> ();
		Vector3 centerMass = new Vector3 (0, -0.8f, 0);
		rg.centerOfMass = centerMass;
	}

	// Update is called once per frame
	void Update () {
		if (waypoints != null) {

			if(Vector3.Distance(transform.position, waypoints[currentWaypoint])<15){
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
							finished = true;
						}
					}
				}
			}

			roundTime+=Time.deltaTime;

			if(rg.velocity.magnitude<5){
				resetTimer += Time.deltaTime;
				if(resetTimer>3){
					Vector3 resetPos = new Vector3();
					if(currentWaypoint == 0)resetPos = waypoints[currentWaypoint];
					else resetPos = waypoints[currentWaypoint-1];
					resetPos.y = 4;
					transform.position = resetPos;

					resetTimer = 0;
				}
			}
			else resetTimer = 0;
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

	public void setRandomTorque(){
		maxTorque = 2000 + Random.value * 1000;
	}

	public void setLaps(int laps){
		totalLaps = laps;
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

	void OnGUI(){
		//GUI.Box(new Rect(0,0,200,200), dir.ToString() + "\n" +dir2.ToString() + "\n" + dir3.ToString());
		//GUI.Box (new Rect (0, 200, 150,20), waypoints [currentWaypoint].ToString ());
	}
}
