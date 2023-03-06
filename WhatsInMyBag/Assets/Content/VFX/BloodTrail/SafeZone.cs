using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    //[SerializeField] 
    private GameObject BloodTrail;
    
    private GameObject Corpse;


    private void Start()
    {
        //BloodTrail bloodTrail = BloodTrail.GetComponent<BloodTrail>();

        BloodTrail = FindObjectOfType<BloodTrail>().gameObject;
        BloodTrail bloodTrail = BloodTrail.GetComponent<BloodTrail>();


        if (bloodTrail.bloodSpawnPosition != null)
        {
            Corpse = bloodTrail.bloodSpawnPosition;
        }
        
        if (!Corpse.gameObject.CompareTag("Dead Body"))
        {
            Debug.Log("BloodTrail -> SafeZone: "+ Corpse.name + "'s Tag is not set to 'Dead Body'. This way, Corpse will also bleed in SafeZones.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Dead Body"))
        {
            BloodTrail.GetComponent<BloodTrail>().corpseInSafeZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Dead Body"))
        {
            BloodTrail.GetComponent<BloodTrail>().corpseInSafeZone = false;
        }
    }
}
