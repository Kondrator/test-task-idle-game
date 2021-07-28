using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace UIWindowManager{

	public class UIButtonMenu : MonoBehaviour {
	
		// auto hide this button when window open
		[SerializeField]
		public UIButtonShowWindow.WhenWindowOpenType whenWindowOpen = UIButtonShowWindow.WhenWindowOpenType.None;

		void Awake(){

			UIButtonShowWindow button = this.AddComponent<UIButtonShowWindow>();
			button.SetWindow( UIWindowMenu.singleton );
			button.whenWindowOpen = whenWindowOpen;

			Destroy( this );
		
		}

	}

}