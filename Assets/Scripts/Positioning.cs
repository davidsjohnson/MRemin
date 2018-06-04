using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Positioning : MonoBehaviour {

    public float distance2Theremin = 0.2286f;  // 9 inches

    private bool positionSet = false;

    public void Awake()
    {
        LoadPositionState();
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

            //Save State
            SavePositionState();
        }
    }

    private void SavePositionState()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/thereminInfo.dat", FileMode.OpenOrCreate);

        PositionData positionData = new PositionData();
        positionData.Position = transform.position;
        positionData.Rotation = transform.eulerAngles;
        positionData.Scale = transform.localScale;

        bf.Serialize(file, positionData);
        file.Close();
    }

    private void LoadPositionState()
    {
        if (File.Exists(Application.persistentDataPath + "/thereminInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/thereminInfo.dat", FileMode.Open);
            PositionData positionData = (PositionData)bf.Deserialize(file);
            file.Close();

            transform.position = positionData.Position;
            transform.eulerAngles = positionData.Rotation;
            transform.localScale = positionData.Scale;
        }
    }
}

[Serializable]
class PositionData
{
    private float[] position = new float[3];
    private float[] rotation = new float[3];
    private float[] scale = new float[3];

    public Vector3 Position
    {
        set
        {
            position[0] = value.x;
            position[1] = value.y;
            position[2] = value.z;
        }

        get
        {
            return new Vector3(position[0], position[1], position[2]);
        }
    }

    public Vector3 Rotation
    {
        set
        {
            rotation[0] = value.x;
            rotation[1] = value.y;
            rotation[2] = value.z;
        }

        get
        {
            return new Vector3(rotation[0], rotation[1], rotation[2]);
        }
    }

    public Vector3 Scale
    {
        set
        {
            scale[0] = value.x;
            scale[1] = value.y;
            scale[2] = value.z;
        }

        get
        {
            return new Vector3(scale[0], scale[1], scale[2]);
        }
    }
}