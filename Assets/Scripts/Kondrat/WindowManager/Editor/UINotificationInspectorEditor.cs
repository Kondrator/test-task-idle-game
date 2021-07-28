using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UIWindowManager{

	[CustomEditor(typeof(UINotificationInspector))]
	public class UINotificationInspectorEditor : UICreateFromInspectorEditor {



		public override void OnInspectorGUI(){
		

			EditorGUILayout.Space();

			// message
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "text" ) );

			// type hide notification
			SerializedProperty propertyTypeWait = serializedObject.FindProperty( "typeWait" );
			EditorGUILayout.PropertyField( propertyTypeWait );

			UINotification.TypeWait typeHide = (UINotification.TypeWait)propertyTypeWait.enumValueIndex;
			switch( typeHide ){
				case UINotification.TypeWait.TimeEnter:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "timeWait" ) );
					break;
			}

			MyOperationEditor.DrawSeparator( 5 );

		
			// type event for show message box
			InspectorGUI();


			EditorGUILayout.Space();


			serializedObject.ApplyModifiedProperties();

		}


	}

}