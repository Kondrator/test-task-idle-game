using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWindowManager;

public class UILoaderBetweenScenesExampleEnum : UILoaderBetweenScenesEnum<LoaderSceneExample> {

	// simple variant load scene
	void TestSimple(){

		// at index build scene
		UILoaderBetweenScenes.LoadScene( 1 );

		// at name scene
		UILoaderBetweenScenes.LoadScene( "SceneNameExample" );

	}

	// example load scene from enum (for code)
	void TestEnum(){
		
		// from this custom class
		UILoaderBetweenScenesExampleEnum.LoadScene( LoaderSceneExample.Arena );

		// OR without this class (WITH ONLY ENUM)
		UILoaderBetweenScenesEnum<LoaderSceneExample>.LoadScene( LoaderSceneExample.Main );

	}

}

// NAME_ITEM = INDEX_IN_BUILD
public enum LoaderSceneExample{
	Main = 0,
	Level = 1,
	Arena = 2
}