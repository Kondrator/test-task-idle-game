using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;



namespace UIWindowManager{

	[CustomEditor(typeof(UIWindowTabControlSimpleButtonSwitch), true)]
	public class UIWindowTabControlSimpleButtonSwitchEditor : Editor {

		private UIWindowTabControlSimpleButtonSwitch switcher;

		private void OnEnable(){
			switcher = target as UIWindowTabControlSimpleButtonSwitch;
		}

	
		public override void OnInspectorGUI(){
		
			switcher.ReFindTarget();
			string[] pages = UIWindowTabControlSimpleEditor.GetPages( switcher.Target );

			switcher.Type = (UIWindowTabControlSimpleButtonSwitch.SwitchType)EditorGUILayout.EnumPopup( "Type", switcher.Type );

			switch( switcher.Type ){

				case UIWindowTabControlSimpleButtonSwitch.SwitchType.ToTarget: 
					switcher.IndexToSwitch = Mathf.Clamp( switcher.IndexToSwitch, 0, pages.Length - 1 );
					switcher.IndexToSwitch = EditorGUILayout.Popup( "Switch To", switcher.IndexToSwitch, pages );
					break;

			}
		
			bool haveButton = switcher.GetComponentInChildren<Button>();
			if( haveButton == false ){
				EditorGUILayout.HelpBox( "Not have a BUTTON in childrens!", MessageType.Warning );
			}

		}


	}

}