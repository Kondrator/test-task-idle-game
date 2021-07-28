using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;



public class MenuScenes : MonoBehaviour {


	private static string pathScenes = "Assets/Scenes/";

	public static void OpenScene( string pathScene ){

		if( Application.isPlaying == true ){
			EditorUtility.DisplayDialog( "Load scene in Editor", "Scene can not be loaded in game mode.", "Close" );
			return;
		}
		
		if( EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() ){
			EditorSceneManager.OpenScene( pathScene );
		}

	}




	[MenuItem( "Scenes/Data/Level", false, 0 )]
	static void MenuItemDataLevel() {
		OpenScene( pathScenes + "Data/Level.unity" );
	}


	[MenuItem( "Scenes/Game", false, 100 )]
	static void MenuItemGame() {
		OpenScene( pathScenes + "Game.unity" );
	}

}
