using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour {

    public GameObject rig;
    public float delta = 0.7f;
    public float maxAngle = 20;

    private Rigidbody rb;
    private Quaternion rotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //float input = Input.GetAxisRaw("Horizontal");

        if (rb.velocity.magnitude > 10)
        {
            //Debug.Log(rb.angularVelocity.magnitude);
            if (rb.angularVelocity.magnitude < 0.4f)
            {
                rotation = Quaternion.LookRotation(transform.forward);
                rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, rotation, Time.deltaTime * delta * 4);
            }
            else
            {
                Vector3 point = Vector3.Reflect(-rb.velocity.normalized, transform.forward);
                rotation = Quaternion.LookRotation(point);

                rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, rotation, Time.deltaTime * delta);

                float y = rig.transform.localRotation.eulerAngles.y;
                y = (y > 180) ? y - 360 : y;
                if (y > maxAngle)
                {
                    Vector3 newRotation = rig.transform.localEulerAngles;
                    newRotation.y = maxAngle;
                    rig.transform.localEulerAngles = newRotation;
                }
                else if (y < -maxAngle)
                {
                    Vector3 newRotation = rig.transform.localEulerAngles;
                    newRotation.y = -maxAngle;
                    rig.transform.localEulerAngles = newRotation;
                }
            }
        }
        else
        {
            rotation = Quaternion.LookRotation(transform.forward);
            rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, rotation, Time.deltaTime * delta);
        }
    }
}
