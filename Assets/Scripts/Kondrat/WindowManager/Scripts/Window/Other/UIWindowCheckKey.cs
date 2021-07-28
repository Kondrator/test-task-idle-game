using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	// optimize method "Update" (only one "Update" for all windows)
	public class UIWindowCheckKey : MonoBehaviour {

		private static UIWindowCheckKey singleton;

		// for create
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void OnBeforeSceneLoadRuntimeMethod(){
			if( singleton == null ){
				singleton = new GameObject( typeof(UIWindowCheckKey).Name ).AddComponent<UIWindowCheckKey>();
				DontDestroyOnLoad( singleton );
			}
		}


		void Update(){
		
			for( int i = 0; i < UIWindow.Windows.Count; i++ ){
			
				UIWindow.Windows[i].CheckKey();

			}

			if( Input.GetMouseButtonDown( 0 ) ){
				// find window and if finded - to top sibling index
				GameObject goFirst = MyOperationUI.GetFirstUIUnderCursor();
				if( goFirst != null ){
					UIWindow window = goFirst.GetComponentInParent<UIWindow>( true );
					if( window != null
						&& window.isToTopAtTouch == true
					){
						window.transform.SetAsLastSibling();
					}
				}
			}

		}

	}

}