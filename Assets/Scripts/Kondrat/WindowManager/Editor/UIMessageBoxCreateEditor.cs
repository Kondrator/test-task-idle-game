using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UIWindowManager{

	[CustomEditor(typeof(UIMessageBoxInspector))]
	public class UIMessageBoxCreateEditor : UICreateFromInspectorEditor {

		private static bool isShowEvents = false;


		public override void OnInspectorGUI(){
		

			EditorGUILayout.Space();


			// message
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "textMessage" ) );

			MyOperationEditor.DrawSeparator( 5 );

			// type
			SerializedProperty propertyType = serializedObject.FindProperty( "typeButtons" );
			EditorGUILayout.PropertyField( propertyType );

			// text at type
			UIMessageBoxButtons type = (UIMessageBoxButtons)propertyType.enumValueIndex;
			switch( type ){
				case UIMessageBoxButtons.Close:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "textButtonClose" ) );
					break;

				case UIMessageBoxButtons.YesNo:
				case UIMessageBoxButtons.YesNoCancel:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "textButtonYes" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "textButtonNo" ) );
					if( type == UIMessageBoxButtons.YesNoCancel ){
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "textButtonCancel" ) );
					}
					break;
			}



			MyOperationEditor.DrawSeparator( 5 );

		
			// type event for show message box
			InspectorGUI();


			MyOperationEditor.DrawSeparator( 5 );



			if( MyOperationEditor.DrawButtonMini( (isShowEvents == true ? "Hide" : "Show") + " Events" ) ){
				isShowEvents = !isShowEvents;
			}
			EditorGUILayout.Space();


			if( isShowEvents == true ){

				// events click at buttons
				switch( type ){
					case UIMessageBoxButtons.Close:
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "onClickClose" ) );
						break;

					case UIMessageBoxButtons.YesNo:
					case UIMessageBoxButtons.YesNoCancel:
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "onClickYes" ) );
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "onClickNo" ) );
						if( type == UIMessageBoxButtons.YesNoCancel ){
							EditorGUILayout.PropertyField( serializedObject.FindProperty( "onClickCancel" ) );
						}
						break;
				}

			}

			serializedObject.ApplyModifiedProperties();

		}


	}

}