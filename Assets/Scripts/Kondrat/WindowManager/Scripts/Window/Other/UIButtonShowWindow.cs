using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace UIWindowManager {

	/// <summary>
	/// Show need window at button
	/// </summary>
	public class UIButtonShowWindow : MonoBehaviour {

		public enum WhenWindowOpenType {
			None,
			Hide,
			Disable,
		}




		// open window
		[SerializeField]
		[HideInInspector]
		private UIWindow window = null;

		// open window by id
		[HideInInspector]
		[SerializeField]
		private bool isByID = false;
		[SerializeField]
		[HideInInspector]
		private string windowID = null;

		// auto hide this button when window open
		[SerializeField]
		[HideInInspector]
		public WhenWindowOpenType whenWindowOpen = WhenWindowOpenType.None;
		// open window
		[HideInInspector]
		public bool isResolutionOpen = true;
		// close window
		[SerializeField]
		[HideInInspector]
		public bool isResolutionClose = true;




		private Button button;
		private UIAnimation anim;






		void Start() {

			if( isByID == true ) {
				window = UIWindow.GetAtID( windowID );
			}

			button = GetComponentInChildren<Button>();
			anim = GetComponentInChildren<UIAnimation>();

			if( button != null ) {
				button.onClick.AddListener( () => {
					if( window != null ) {
						if( window.IsOpen == false ) {
							if( isResolutionOpen == true ) {
								window.Open();
							}

						} else if( isResolutionClose == true ) {
							window.Close();
						}
					}
				} );
			}

			SetWindow( window );

		}

		void OnEnable() {
			if( window != null ) {
				CheckWindowOpen( window.IsOpen );
			}
		}


		public void SetWindow( UIWindow windowNew ) {

			if( window != null ) {
				window.OnOpen.RemoveListener( OnWindowOpen );
				window.OnCloseAnimationCompleted.RemoveListener( OnWindowClose );
			}

			window = windowNew;

			if( window != null ) {
				window.OnOpen.AddListener( OnWindowOpen );
				window.OnCloseAnimationCompleted.AddListener( OnWindowClose );
			}

			if( anim != null ) {
				anim.window = window;
			}

		}

		private void OnWindowOpen() {
			CheckWindowOpen( true );
		}
		private void OnWindowClose() {
			CheckWindowOpen( false );
		}




		private void CheckWindowOpen( bool isOpen ) {
			switch( whenWindowOpen ) {

				case WhenWindowOpenType.Hide:
					SetVisible( !isOpen );
					break;

				case WhenWindowOpenType.Disable:
					if( button != null ) {
						button.interactable = !isOpen;
					}
					break;

			}
		}





		/// <summary>
		/// Set visible button.
		/// </summary>
		public void SetVisible( bool isVisible ) {

			if( anim != null ) {
				anim.Visible( isVisible );

			} else {
				if( button != null
					&& button.targetGraphic != null
				) {
					button.targetGraphic.enabled = isVisible;
				}
			}

		}





#if UNITY_EDITOR
		[CustomEditor( typeof(UIButtonShowWindow) )]
		private class UIButtonShowWindowEditor : Editor<UIButtonShowWindow> {


			public override void OnInspectorGUI() {
				base.OnInspectorGUI();


				EditorGUILayout.Space( 10 );

				// window by id ?
				component.isByID = EditorGUILayout.Toggle( "Window By ID", component.isByID );

				if( component.isByID == true ) {
					// window id
					component.windowID = UIWindow.PopupID( component.windowID );

				} else {
					// window reference
					component.window = MyOperationEditor.DrawDropArea<UIWindow>( component.window, "Window" );
				}

				// other

				EditorGUILayout.Space( 10 );
				component.isResolutionOpen = EditorGUILayout.Toggle( "Resolution Open", component.isResolutionOpen );

				EditorGUILayout.Space( 10 );
				component.whenWindowOpen = (WhenWindowOpenType)EditorGUILayout.EnumPopup( "When Window Open", component.whenWindowOpen );
				component.isResolutionClose = EditorGUILayout.Toggle( "Resolution Close", component.isResolutionClose );



				if( GUI.changed ) {
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty( component );
				}

			}

		}
#endif


	}

}