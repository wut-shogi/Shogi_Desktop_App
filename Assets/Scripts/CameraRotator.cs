using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fullAngle > curAngle)
        {
            float angle = (speed*Time.deltaTime);
            curAngle += angle;
            GetComponent<Camera>().transform.RotateAround(new Vector3(0,0,0),new Vector3(0,1,0),angle);
        }
    }
    private float fullAngle = 0;
    private float curAngle = 0;
    private int speed = 180;
    
    public void rotate(float angle)
    {
        GetComponent<Camera>().transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), angle);
        fullAngle = angle;
        curAngle = angle;

    }
}
