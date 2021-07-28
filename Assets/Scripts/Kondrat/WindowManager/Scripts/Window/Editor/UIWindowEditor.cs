using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor.SceneManagement;



namespace UIWindowManager{

	[CustomEditor(typeof(UIWindow), true)]
	[DisallowMultipleComponent]
	public class UIWindowEditor : Editor{


		protected UIWindow window;
		private MonoScript script;
		private static string[] typesCursorBlock = new string[]{ "When Open", "When Close" };

		private bool	isVisibleEventsOpen = false,
						isVisibleEventsClose = false,
						isVisibleEventsOpenClose = false;

		private AnimationSet	animationSetOpen = null,
								animationSetClose = null;


		protected virtual void OnEnable(){
			window = target as UIWindow;
			script = MonoScript.FromMonoBehaviour( window );
		
			AnimationSet.SettingSets( window.AnimationSetOpen, window.AnimationSetClose );
			animationSetOpen = window.AnimationSetOpen;
			animationSetClose = window.AnimationSetClose;
		}

		protected virtual void OnDestroy(){
			if( Application.isEditor ){
				if( window == null ){
					if( animationSetOpen != null ){
						GameObject.DestroyImmediate( animationSetOpen.gameObject );
					}
					if( animationSetClose != null ){
						GameObject.DestroyImmediate( animationSetClose.gameObject );
					}
				}
			}
		}
		
		
		public override void OnInspectorGUI(){

			float labelWidth = EditorGUIUtility.labelWidth - 4f;
			if( EditorGUIUtility.currentViewWidth > 357 ){
				float size = EditorGUIUtility.currentViewWidth - 357;
				labelWidth -= Mathf.Clamp( size, size, 7f );
			}




			// save in other variable all fields
			Dictionary<string, SerializedProperty> fieldsGUI = new Dictionary<string, SerializedProperty>();
			SerializedProperty iterator = serializedObject.GetIterator();
			while( iterator.NextVisible( true ) ){
				fieldsGUI[iterator.name] = serializedObject.FindProperty( iterator.name );
			}
			fieldsGUI.Remove( "m_Script" );
			// after removing drawing fields in this method



			EditorGUILayout.ObjectField( "Script:", script, typeof(MonoScript), false );

			// button-editor open / close
			if( window.content != null
				&& window.gameObject.activeInHierarchy
			){
				bool isOpen = window.IsOpen;
				if( Application.isPlaying == false ){
					isOpen = window.content.activeSelf;
				}

				if( MyOperationEditor.DrawButtonMini( isOpen ? "CLOSE" : "OPEN" ) ){
					if( Application.isPlaying == true ){
						window.OpenClose();

					}else{
						window.IsOpen = !isOpen;
						window.content.gameObject.SetActive( window.IsOpen );
					}
				}
			}
		

			MyOperationEditor.DrawSeparator( 5 );


			// --- CONTENT
			window.content = MyOperationEditor.DrawDropArea<GameObject>( window.content, "Content" );
			fieldsGUI.Remove( "content" );
			if( window.content != null ){
				if( window.gameObject == window.content ){
					Debug.LogError( "<b>Need set content at children window</b>!" );
					window.content = null;

				}else{
					// check content at children window
					RectTransform[] childrens = window.GetComponentsInChildren<RectTransform>( true );
					for( int i = 0; i < childrens.Length; i++ ){
						if( childrens[i].gameObject == window.content ){
							childrens = null;
							break;
						}
					}
					if( childrens != null ){
						Debug.LogError( "<b>" + window.content + "</b> is not children in <b>" + window + "</b>!" );
						window.content = null;
					}
				}
			}

			if( window.content == null ){
				EditorGUILayout.LabelField( "Need set content of window for settings..." );
				return;
			}
		

			MyOperationEditor.DrawSeparator( 5 );


			// --- IDENTIFIER

			if( String.IsNullOrEmpty( window.id ) ){
				window.id = null;
				if( Application.isPlaying == false ){
					if( MyOperationEditor.DrawButtonMini( "create id" ) ){
						window.id = "custom id";
					}

				}else{
					EditorGUILayout.LabelField( "Not have ID" );
				}
			

			}else{
				if( Application.isPlaying == false ){
					EditorGUILayout.BeginHorizontal();
						// enter id
						window.id = EditorGUILayout.TextField( "ID", window.id );
						// check id equals in other id at scene
						if( MyOperationEditor.DrawButtonMini( "?", Color.yellow.Clone( 0.3f ), 18 ) ){
							string idFree = null;
							int index = 0;
							UIWindow[] windows = FindObjectsOfType<UIWindow>();
							for( int i = 0; i < windows.Length; i++ ){
								if( windows[i] != window
									&& window.id == windows[i].id
								){
									if( idFree == null ){
										idFree = "This id \"" + window.id + "\" contain in (on current scene):";
									}
									idFree += "\n" + ++index + ") " + windows[i].name;
								}
							}
							// show result equals
							if( idFree != null ){
								EditorUtility.DisplayDialog( "Check equals id with other windows.", idFree, "Close" );

							}else{
								EditorUtility.DisplayDialog( "Check equals id with other windows.", "This id \"" + window.id + "\" is free.", "Close" );
							}
						}
						// remove id
						if( MyOperationEditor.DrawButtonMini( "x", Color.red.Clone( 0.3f ), 18 ) ){
							window.id = null;
							GUI.FocusControl( "" );
						}
					EditorGUILayout.EndHorizontal();

				}else{
					EditorGUILayout.LabelField( "Window id: <b>" + window.id + "</b>", MyOperationEditor.StyleLabel );
				}
			}

			fieldsGUI.Remove( "id" );

			MyOperationEditor.DrawSeparator( 5 );




			// --- BUTTONS
			window.bClose = MyOperationEditor.DrawDropArea<Button>( window.bClose, "Button Close" );
			fieldsGUI.Remove( "bClose" );

			MyOperationEditor.DrawSeparator( 5 );


			// --- SOUND
			window.soundOpen = MyOperationEditor.DrawDropAudioClip( window.soundOpen, "Sound Open" );
			window.soundClose = MyOperationEditor.DrawDropAudioClip( window.soundClose, "Sound Close" );
			fieldsGUI.Remove( "soundOpen" );
			fieldsGUI.Remove( "soundClose" );
		

			MyOperationEditor.DrawSeparator( 5 );


			// --- BOOLEANS
			window.isStatic = EditorGUILayout.Toggle( "Static", window.isStatic );
			// hide at awake
			window.typeAwake = (UIWindow.AwakeType)EditorGUILayout.EnumPopup( "Awake Type", window.typeAwake );
			// resolution open/close when now animation
			window.isOpenCloseAtAnimation = EditorGUILayout.Toggle( "Open/Close When Animation", window.isOpenCloseAtAnimation );
			// close at escape
			window.isCloseEscape = EditorGUILayout.Toggle( "Close At Escape", window.isCloseEscape );


			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "To Top At", GUILayout.Width( labelWidth ) );
				// to top at show
				window.isToTopAtOpen = EditorGUILayout.Toggle( window.isToTopAtOpen, GUILayout.Width( 15 ) );
				GUILayout.Label( "Open", GUILayout.MinWidth( 25 ) );
				// to top at show
				window.isToTopAtTouch = EditorGUILayout.Toggle( window.isToTopAtTouch, GUILayout.Width( 15 ) );
				GUILayout.Label( "Touch", GUILayout.MinWidth( 25 ) );
			EditorGUILayout.EndHorizontal();

			// cursor block
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Cursor Block", GUILayout.Width( labelWidth ) );
				window.isCursorBlock = EditorGUILayout.Toggle( window.isCursorBlock, GUILayout.Width( 15f ) );
				if( window.isCursorBlock == true ){
					window.isCursorBlockWhenOpen = EditorGUILayout.Popup( window.isCursorBlockWhenOpen == true ? 0 : 1, typesCursorBlock ) == 0;
				}
			EditorGUILayout.EndHorizontal();

			// open at key
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Open At Key", GUILayout.Width( labelWidth ) );
				window.isOpenAtKey = EditorGUILayout.Toggle( window.isOpenAtKey, GUILayout.Width( 15f ) );
				if( window.isOpenAtKey == true ){
					window.keyTarget = (KeyCode)EditorGUILayout.EnumPopup( window.keyTarget );
					EditorGUILayout.LabelField( "+ close", GUILayout.Width( 45f ) );
					window.isCloseAtKey = EditorGUILayout.Toggle( window.isCloseAtKey, GUILayout.Width( 15f ) );
				}
			EditorGUILayout.EndHorizontal();



			fieldsGUI.Remove( "isStatic" );
			fieldsGUI.Remove( "typeAwake" );
			fieldsGUI.Remove( "isOpenCloseAtAnimation" );
			fieldsGUI.Remove( "isCloseEscape" );
			fieldsGUI.Remove( "isToTopAtOpen" );
			fieldsGUI.Remove( "isToTopAtTouch" );
			fieldsGUI.Remove( "isOpenAtKey" );
			fieldsGUI.Remove( "isCloseAtKey" );
			fieldsGUI.Remove( "isCursorBlock" );
			fieldsGUI.Remove( "isCursorBlockWhenOpen" );
			fieldsGUI.Remove( "keyTarget" );
		
		
			AnimationSet.DrawGUI( window.AnimationSetOpen, window.content.transform as RectTransform );
			AnimationSet.DrawGUI( window.AnimationSetClose, window.content.transform as RectTransform );

			fieldsGUI.Remove( "animationSetOpen" );
			fieldsGUI.Remove( "animationSetClose" );





			// --- EVENTS
			MyOperationEditor.DrawSeparator( 5 );
			DrawEvents( "Events - Open", ref isVisibleEventsOpen, fieldsGUI, "OnOpen", "OnOpenAnimationCompleted", "OnCloseOnEscape" );

			MyOperationEditor.DrawSeparator( 5 );
			DrawEvents( "Events - Close", ref isVisibleEventsClose, fieldsGUI, "OnClose", "OnCloseAnimationCompleted" );

			MyOperationEditor.DrawSeparator( 5 );
			DrawEvents( "Events - Open & Close", ref isVisibleEventsOpenClose, fieldsGUI, "OnOpenAnimationCompletedType" );






			// --- OTHER FIELDS
			if( fieldsGUI.Count > 0 ){

				MyOperationEditor.DrawSeparator( 5 );


				if( GetType() != typeof(UIWindowEditor) ){
					OnInspectorGUI( ref fieldsGUI );
					MyOperationEditor.DrawSeparator( 5 );
				}


				// draw other custom fields
				MyOperationEditor.DrawFields( fieldsGUI, serializedObject );

			}



			// --- EXRA
			window.OnInspectorGUI();



			if( GUI.changed
				&& Application.isPlaying == false
			){
				EditorUtility.SetDirty( window );
				serializedObject.ApplyModifiedProperties();
				EditorSceneManager.MarkAllScenesDirty();
			}

		}


		/// <summary>
		/// Draw info for animation Custom.
		/// </summary>
		/// <param name="target">Target object.</param>
		/// <param name="key1">Need animation clip in Animation.</param>
		/// <param name="key2">Need animation clip in Animation.</param>
		/// <param name="speed">Multiply speed (normal speed is 1).</param>
		public static void DrawAnimationCustom( MonoBehaviour target, string key1, string key2, ref float speed ){
		
			Animation animation = target.AddComponent<Animation>();
			animation.playAutomatically = false;

			speed = EditorGUILayout.Slider( "Speed Multiply", speed, 0.1f, 4f );

			EditorGUILayout.HelpBox(	String.Format( "{0}\n1) {1}\n2) {2}", "Animation must contain two animations with names:", key1, key2 ), 
										MessageType.Warning 
									);

			EditorGUILayout.HelpBox( "Advice: not animate the root.", MessageType.Info );

		}

		public static void FixSwitchAnimationFromCustom( MonoBehaviour target, bool isCustom ){
		
			if( isCustom == false ){
				Animation animation = target.GetComponent<Animation>();
				if( animation != null ){
					DestroyImmediate( animation );
				}
			}

		}


	

		private void DrawEvents( string title, ref bool isVisibleNow, Dictionary<string, SerializedProperty> fieldsGUI, params string[] namesFields ){
		
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( title );
				if( MyOperationEditor.DrawButtonMini( isVisibleNow == true ? "hide" : "show", 50 ) ){
					isVisibleNow = !isVisibleNow;
				}
			EditorGUILayout.EndHorizontal();


			for( int i = 0; i < namesFields.Length; i++ ){
				if( isVisibleNow == true ){
					EditorGUILayout.PropertyField( serializedObject.FindProperty( namesFields[i] ) );
				}
				fieldsGUI.Remove( namesFields[i] );
			}

		}



		/// <summary>
		/// Draw fields.
		/// </summary>
		/// <param name="fieldsGUI">Remove fields after drawing!</param>
		protected virtual void OnInspectorGUI( ref Dictionary<string, SerializedProperty> fieldsGUI ){}



	}

}