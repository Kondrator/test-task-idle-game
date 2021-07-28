using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWindowManager;

public class UISimulateLoadSceneExample : MonoBehaviour, IListenerLoaderSceneProgress, IListenerLoaderSceneCompleted {

	[SerializeField, Range( 1f, 10f )]
	private float secondsWait = 4f;

	// loading item in scene
	IEnumerator IListenerLoaderSceneProgress.SceneLoading(){
		Debug.Log( "<b>" + this.name + ":</b> load start (wait seconds = " + secondsWait + ")" );
		yield return new WaitForSeconds( secondsWait ); // do something
		Debug.Log( "<b>" + this.name + ":</b> load end" );
	}

	// completed loading scene
	void IListenerLoaderSceneCompleted.OnLoadSceneCompleted(){
		Debug.Log( "<b>" + this.name + ":</b> I received an alert at load completed" );
	}

}
