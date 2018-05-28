using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positioning : MonoBehaviour {

    public float distance2Theremin = 0.2286f;  // 9 inches

    private bool positionSet = false;

	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update ()
    {

        // Addding Here for now since I'm not sure when exactly controllers are initialized and doesn't work in Awake or Start
        // Will eventually add a UI element to control this
        //if (!positionSet)
        //{
        //    positionSet = PositionTheremin();
        //}

    }


    public void PositionTheremin(GameObject motionControllers)
    {
        //1.Get motion controller positions(x, z); we can ignore y for now
        Transform leftController = motionControllers.transform.Find("LeftController");
        Transform rightController = motionControllers.transform.Find("RightController");

        if (leftController != null && rightController != null)
        {
            //2.Calculate center point(x, z) point
            Vector3 centerPos = leftController.transform.position + rightController.transform.position;
            centerPos /= 2;

            //UnityEngine.Debug.Log("Left" + leftController.transform.position);
            //UnityEngine.Debug.Log("Right" + rightController.transform.position);
            //UnityEngine.Debug.Log("Center" + centerPos);

            //3.Calculate(x, z) point X inches away from center point
            Vector2 rightPt = new Vector2(rightController.transform.position.x, rightController.transform.position.z);
            Vector2 centerPt = new Vector2(centerPos.x, centerPos.z);
            float angle = Vector2.SignedAngle(centerPt - rightPt, Vector2.right);

            float xDelta = Mathf.Sin(Mathf.Deg2Rad * angle) * distance2Theremin;
            float zDelta = Mathf.Cos(Mathf.Deg2Rad * angle) * distance2Theremin;

            float newX = centerPt.x + xDelta;
            float newZ = centerPt.y + zDelta;

            //4.Update Theremin position to this point
            Vector3 pos = transform.localPosition;
            pos.x = newX;
            pos.z = newZ;
            transform.localPosition = pos;

            Vector3 angles = transform.localEulerAngles;
            angles.y = 180 + angle;
            transform.localEulerAngles = angles;
        }
    }
}
