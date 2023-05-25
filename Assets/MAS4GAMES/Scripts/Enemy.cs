using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody[] _ragdollRigidbodies;
    private bool isRagdoll = true;
    
    void Awake()
    {
        //Get every ragdoll component in the model
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            if(isRagdoll){
                DisableRagdoll();
            }else{
                EnableRagdoll();
            }
            isRagdoll = !isRagdoll;
        }
    }

    private void DisableRagdoll(){
        foreach (var rigidBody in _ragdollRigidbodies){
            //Stop physics affecting the rigidbody
            rigidBody.isKinematic = true;
        }
    }

    private void EnableRagdoll(){
        foreach (var rigidBody in _ragdollRigidbodies){
            //Stop physics affecting the rigidbody
            rigidBody.isKinematic = false;
        }
    }
}
