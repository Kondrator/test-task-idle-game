using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowListTEST_GENERATEItem : MonoBehaviour {

	[SerializeField]
	private Text textItem = null;

	private DATA_CUSTOM_TEST data = null;


	public void Set( DATA_CUSTOM_TEST data ){
		this.data = data;
	}


	// this method for example!
	void Update(){
		if( textItem != null
			&& data != null
		){
			textItem.text = data.text;
		}
	}

}

public class DATA_CUSTOM_TEST{
	public string text = "";

	public DATA_CUSTOM_TEST( string text ){
		this.text = text;
	}
}