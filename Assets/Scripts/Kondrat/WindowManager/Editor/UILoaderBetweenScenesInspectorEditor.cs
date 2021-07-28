using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;


namespace UIWindowManager{

	[CustomEditor(typeof(UILoaderBetweenScenesInspector))]
	public class UILoaderBetweenScenesInspectorEditor : UICreateFromInspectorEditor {


		private string[] scenesNames = null;
		private UILoaderBetweenScenesInspector loader;

	
		void OnEnable(){

			loader = base.target as UILoaderBetweenScenesInspector;
		
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
			scenesNames = new string[scenes.Length];

			for( int i = 0; i < scenesNames.Length; i++ ){
				EditorBuildSettingsScene scene = scenes[i];

				// get path to scene
				string sceneName = scene.path;

				// get name from path
				sceneName = scene.path.Split( '/' ).Last(); // "example_name.unity"
				// correct name
				sceneName = sceneName.Replace( ".unity", "" );

				// fix for disabled scene
				if( scene.enabled == false ){
					sceneName = sceneName + "  ---> (disabled)";
				}

				scenesNames[i] = sceneName;
			}

		}



		public override void OnInspectorGUI(){
		

			EditorGUILayout.Space();
		
			if( scenesNames.Length > 0 ){
				int index = Array.IndexOf( scenesNames, loader.sceneName );
				index = EditorGUILayout.Popup( "Scene Load", index, scenesNames );
				if( index >= 0 && index < scenesNames.Length ){
					loader.sceneName = scenesNames[index];

				}else{
					loader.sceneName = scenesNames[0];
				}

			}else{
				EditorGUILayout.HelpBox( "Add scenes in: File -> Build Settings...", MessageType.Warning );
			}

			MyOperationEditor.DrawSeparator( 5 );

		
			// type event for show message box
			InspectorGUI();


			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();

		}


	}

}