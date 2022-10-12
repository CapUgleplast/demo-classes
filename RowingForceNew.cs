using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowingForceNew : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 tmpPoint;
    private Vector3 impulseVector;
    public Rigidbody raftRig;
    public float raftDrag;
    public int Multiplier;
    public GameObject HandL;
    public GameObject HandR;
    public float freq;
    public float amp;
    private Vector3 startRaftPoint;
    private ConstantForce drag;
    public bool dragOn = true;

    // Start is called before the first frame update
    void Start() {
        if (LobbyManager.VR) {
            HandR = GameObject.FindGameObjectWithTag("HandR");
            HandL = GameObject.FindGameObjectWithTag("HandL");
        }
        startRaftPoint = raftRig.transform.position;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Paddle") {
            startPoint = other.transform.position;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag == "Paddle") {
            tmpPoint = other.transform.position;
            impulseVector = (startPoint - tmpPoint);
            raftRig.AddForce(impulseVector*Multiplier, ForceMode.Impulse);
            torqueForce();
            
            startPoint = tmpPoint;
            if (HandR.GetComponent<Autohand.Demo.XRHandControllerLink>().grabbingCheck == true) {
                OVRInput.SetControllerVibration(freq, amp, OVRInput.Controller.RTouch);

            }

            if (HandL.GetComponent<Autohand.Demo.XRHandControllerLink>().grabbingCheck == true) {
                OVRInput.SetControllerVibration(freq, amp, OVRInput.Controller.LTouch);

            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Paddle") {
            dragOn = true;
            impulseVector = Vector3.zero;
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }
    }
            // Update is called once per frame
        void Update() {

        if (dragOn == true) {
            
            if (Vector3.Distance(raftRig.transform.position, new Vector3(raftRig.transform.position.x, raftRig.transform.position.y, startRaftPoint.z)) > 0.01f) {
                if (raftRig.transform.position.z - startRaftPoint.z > 0) {
                    raftRig.velocity += new Vector3(0, 0, -raftDrag * 0.001f);
                }
                else if (raftRig.transform.position.z - startRaftPoint.z < 0) {
                    raftRig.velocity += new Vector3(0, 0, raftDrag * 0.001f);
                }
            }
            else {
                raftRig.velocity += new Vector3(0, 0, 0);
            }
        }
        if (raftRig.transform.rotation.y <= -0.08f) {
            raftRig.freezeRotation = true;
            raftRig.angularVelocity = Vector3.zero;
            raftRig.transform.Rotate(new Vector3(raftRig.transform.rotation.x, 0.1f, raftRig.transform.rotation.z));
        }
        else if (raftRig.transform.rotation.y >= 0.08f) {
            raftRig.freezeRotation = true;
            raftRig.angularVelocity = Vector3.zero;
            raftRig.transform.Rotate(new Vector3(raftRig.transform.rotation.x, -0.1f, raftRig.transform.rotation.z));
        }
        }
    void torqueForce() {
        if (raftRig.transform.rotation.y < 0.08f && raftRig.transform.rotation.y > -0.08f) {
           
            if (impulseVector.x < 0 || (raftRig.transform.position.x - impulseVector.x) > 0.1) {
                raftRig.AddTorque(new Vector3(0, -0.2f, 0), ForceMode.Impulse);
                raftRig.constraints &= ~RigidbodyConstraints.FreezeRotationY;

            }
            else
            if (impulseVector.x > 0 || (raftRig.transform.position.x - impulseVector.x) < 0.1) {
                raftRig.AddTorque(new Vector3(0, 0.2f, 0), ForceMode.Impulse);
                raftRig.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            }
        }
       



    }

}
