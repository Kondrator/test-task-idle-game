using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWindowManager;

public class UIWindowExampleIdentifier : MonoBehaviour {

	[SerializeField]
	private InputField inputID = null;
	[SerializeField]
	private Button	bShow = null,
					bHide = null;



	void Awake(){
		
		if( bShow != null ){
			bShow.onClick.AddListener( Show );
		}
		
		if( bHide != null ){
			bHide.onClick.AddListener( Hide );
		}

	}



	private void Show(){
		
		if( inputID == null ){
			return;
		}

		UIWindow window = UIWindow.GetAtID( inputID.text );
		if( window != null ){
			window.Open();

		}else{
			UINotification.CreateShow( "Not have window with id <b>" + inputID.text + "</b>", UINotification.TypeWait.TimeEnter, 1f );
		}

	}

	private void Hide(){
		
		if( inputID == null ){
			return;
		}
		
		UIWindow window = UIWindow.GetAtID( inputID.text );
		if( window != null ){
			window.Close();

		}else{
			UINotification.CreateShow( "Not have window with id <b>" + inputID.text + "</b>" );
		}

	}

}
