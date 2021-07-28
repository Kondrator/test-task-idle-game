using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonAudio : MonoBehaviour {

	[SerializeField]
	private AudioClip clipClick = null;

	void Awake(){
		GetComponent<Button>().AddListenerOnClick( () =>{
			//AudioManager.PlayEffect( clipClick );
		} );
	}


}
