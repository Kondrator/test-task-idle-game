using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWindowManager;


public class GameObjectActiveByLoaderScene : MonoBehaviour, IListenerLoaderSceneProgress {

	public IEnumerator SceneLoading(){
		yield return new WaitForEndOfFrame();
		this.gameObject.Activate();
		print( this.name + ": Activate" );
	}

}
