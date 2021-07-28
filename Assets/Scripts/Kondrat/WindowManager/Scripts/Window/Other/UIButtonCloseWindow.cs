using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace UIWindowManager{

	/// <summary>
	/// Close need window at button.
	/// </summary>
	public class UIButtonCloseWindow : MonoBehaviour {

		// close window
		[SerializeField]
		private UIWindow window = null;

		private Button button;

		void Start(){
		
			button = GetComponentInChildren<Button>();
		
			button.AddListenerOnClick( () => {
				if( window != null ){
					window.Close();
				}
			} );

		}

	}

}