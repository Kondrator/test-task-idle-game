using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UIWindowManager{

	[CustomEditor(typeof(UICreateFromInspector))]
	public class UICreateFromInspectorEditor : Editor {


		protected void InspectorGUI(){


			// type event for show message box
			SerializedProperty propertyTypeEvent = serializedObject.FindProperty( "eventCreate" );
			EditorGUILayout.PropertyField( propertyTypeEvent );


			UIMessageBoxInspector.EventCreate typeEvent = (UIMessageBoxInspector.EventCreate)propertyTypeEvent.enumValueIndex;
			switch( typeEvent ) {

				case UIMessageBoxInspector.EventCreate.Button:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "buttonEvent" ) );
					break;

				case UIMessageBoxInspector.EventCreate.Timer:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "timerEvent" ) );
					break;

			}


			serializedObject.ApplyModifiedProperties();

		}


	}

}