using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWindowManager;


public class UIMessageBoxExampleDynamic : MonoBehaviour {

	[SerializeField]
	private Button	bShowMessageBox = null,
					bShowNotification = null;
	[SerializeField]
	private InputField	textNubmerMin = null,
						textNumberMax = null;


	void Awake(){
		
		if( bShowMessageBox != null ){
			bShowMessageBox.onClick.AddListener( ShowMessageBox );
		}

		if( bShowNotification != null ){
			bShowNotification.onClick.AddListener( ShowNotification );
		}

	}



	private void ShowMessageBox(){
		
		try{
			int min = System.Convert.ToInt32( textNubmerMin.text );
			int max = System.Convert.ToInt32( textNumberMax.text );
			string message = "Random number:\n" + Random.Range( min, max ) + "\nbetween " + min + " and " + max;

			UIMessageBox box = UIMessageBox.CreateShow(	message, UIMessageBoxButtons.YesNo, "Again", "Close" );
			box.OnClickYes.AddListener( ShowMessageBox );
		}catch{
			UIMessageBox.CreateShow( "Incorrect Numbers", UIMessageBoxButtons.Close, "OK" );
		}

	}

	private void ShowNotification(){
		
		try{
			int min = System.Convert.ToInt32( textNubmerMin.text );
			int max = System.Convert.ToInt32( textNumberMax.text );
			string message = Random.Range( min, max ) + " <== [" + min + "; " + max + "]";

			UINotification.CreateShow( message );
		}catch{
			UINotification.CreateShow( "Incorrect Numbers" );
		}

	}

}
