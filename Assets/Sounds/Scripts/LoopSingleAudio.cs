 using UnityEngine;
 using System.Collections;
 
 public class LoopSingleAudio : MonoBehaviour {
 
     public AudioSource soundSource;
     public float minDur;
     public float maxDur;
 
     // Use this for initialization
     void Start () {
				
		// Create an array
         CallAudio ();
     }
 
 
     private void CallAudio()
     {
         Invoke ("PlayRandomSound", Random.Range(minDur,maxDur));

     }
 
     private void PlayRandomSound()
     {
         soundSource.Play();
         CallAudio ();
     }
 }