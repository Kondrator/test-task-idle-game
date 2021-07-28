using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	public class UIWindowCursorBlocker : MonoBehaviour {

	
		private static UIWindowCursorBlocker singleton_ = null;
		public static UIWindowCursorBlocker singleton{
			get{
				if( singleton_ == null ){
					singleton_ = new GameObject( typeof(UIWindowCursorBlocker).Name ).AddComponent<UIWindowCursorBlocker>();
				}
				return singleton_;
			}
		}
		// for create
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnBeforeSceneLoadRuntimeMethod(){
			DontDestroyOnLoad( singleton );
		}



		private bool isBlocked = false;
		public static bool IsBlocked{
			get{
				return singleton.isBlocked;
			}
		}


		void Update(){
		
			// check block
			isBlocked = false;
			for( int i = 0; i < UIWindow.Windows.Count; i++ ){

				UIWindow window = UIWindow.Windows[i];

				if( window.isCursorBlock == true
					&& window.IsOpen == window.isCursorBlockWhenOpen
				){
					// have blocker
					isBlocked = true;
					break;
				}

			}

			// setting
			Cursor.visible = !isBlocked;
			Cursor.lockState = isBlocked ? CursorLockMode.Locked : CursorLockMode.None;

		}

	}

}