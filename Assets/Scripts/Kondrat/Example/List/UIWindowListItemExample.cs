using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowListItemExample : MonoBehaviour {

	[SerializeField]
	private Text textItem = null;

	private DataExample data = null;


	public void Set( DataExample data ){
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

public class DataExample{
	public string text = "";
	public DataExample( string text ){
		this.text = text;
	}
}