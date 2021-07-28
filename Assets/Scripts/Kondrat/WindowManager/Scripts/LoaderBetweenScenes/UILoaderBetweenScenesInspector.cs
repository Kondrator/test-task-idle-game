using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	public class UILoaderBetweenScenesInspector : UICreateFromInspector {

		[SerializeField]
		public string sceneName = "";


		public override void Create(){
		
			UILoaderBetweenScenes.LoadScene( sceneName );

		}


	}

}