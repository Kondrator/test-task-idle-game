using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;



namespace UIWindowManager{

	[CustomEditor(typeof(UIWindowTabControlSimple), true)]
	[CanEditMultipleObjects]
	public class UIWindowTabControlSimpleEditor : UIWindowEditor{

		protected UIWindowTabControlSimple windowTC;

		protected override void OnEnable(){
			base.OnEnable();

			windowTC = window as UIWindowTabControlSimple;
		}


		protected override void OnInspectorGUI( ref Dictionary<string, SerializedProperty> fieldsGUI ){
		
			string[] pages = GetPages( windowTC );


			// DRAW FIELDS

			windowTC.tabContents = MyOperationEditor.DrawDropArea<Transform>( windowTC.tabContents, "Contents - Tabs" );
			windowTC.tabButtons = MyOperationEditor.DrawDropArea<Transform>( windowTC.tabButtons, "Buttons - Switch" );
			windowTC.tabDropdown = MyOperationEditor.DrawDropArea<Dropdown>( windowTC.tabDropdown, "Dropdown - Switch" );
			
			windowTC.toPrevTabAtEscape = EditorGUILayout.Toggle( "To Prev Tab At Escape", windowTC.toPrevTabAtEscape );
			windowTC.isShowTabAtOpen = EditorGUILayout.Toggle( "Show Tab At Open", windowTC.isShowTabAtOpen );
			if( windowTC.isShowTabAtOpen == true ){
				windowTC.indexTabAtOpen = EditorGUILayout.Popup( "Tab At Open", windowTC.indexTabAtOpen, pages );
			}

			fieldsGUI.Remove( "tabContents" );
			fieldsGUI.Remove( "tabButtons" );
			fieldsGUI.Remove( "tabDropdown" );
			fieldsGUI.Remove( "toPrevTabAtEscape" );
			fieldsGUI.Remove( "isShowTabAtOpen" );
			fieldsGUI.Remove( "indexTabAtOpen" );


			if( windowTC.tabContents != null ){
				for( int i = 0; i < windowTC.tabContents.childCount; i++ ){
					UIAnimation animTab = windowTC.tabContents.GetChild( i ).GetComponent<UIAnimation>();
					if( animTab != null ){
						animTab.controlled = windowTC;
					}
				}
			}




			// FIX NAMES
			// buttons titles
			if( windowTC.tabButtons != null ){
				for( int i = 0; i < windowTC.tabButtons.childCount; i++ ){
					windowTC.tabButtons.GetChild( i ).name = "Page " + i + " - Title (Button)";
				}
			}
			// contents
			if( windowTC.tabContents != null ){
				for( int i = 0; i < windowTC.tabContents.childCount; i++ ){
					windowTC.tabContents.GetChild( i ).name = "Page " + i + " - Content";
				}
			}




			// SELECT PAGE IN INSPECTOR
			// count first correct pages (with title and page)
			if( Application.isPlaying == false
				&& windowTC.gameObject.activeInHierarchy
			){
				int count = 0;
				if( windowTC.tabContents != null ){
					count = windowTC.tabContents.childCount;
					int index = 0;
					for( int i = 0; i < pages.Length; i++ ){
						if( windowTC.tabContents.GetChild( i ).gameObject.activeSelf == true ){
							index = i;
						}
					}
					index = EditorGUILayout.Popup( "Page Visible", index, pages );
					windowTC.IndexSelectTab = index;
				}
			}

		}

		/// <summary>
		/// Generate array with names of GameObjects contets.
		/// </summary>
		public static string[] GetPages( UIWindowTabControlSimple target ){
			if( target != null
				&& target.tabContents != null
			){
				int count = target.tabContents.childCount;
				string[] pages = new string[count];
				for( int i = 0; i < pages.Length; i++ ){
					Transform page = target.tabContents.GetChild( i );
					pages[i] = page.name;
				}
				return pages;
			}
			return new string[0];
		}

	}

}