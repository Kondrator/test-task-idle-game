using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWindowManager;

public class UIWindowListTEST_GENERATE : UIWindowListArray<UIWindowListTEST_GENERATEItem, DATA_CUSTOM_TEST> {

	[SerializeField]
	private int countItemsExample = 5; // for example!

	// only two method:
	// 1) setting each element
	// 2) get data

	// setting at item
	protected override void SettingItemExtra( UIWindowListTEST_GENERATEItem container, DATA_CUSTOM_TEST item, int index ){
		container.Set( item );
	}


	// this method executed at open window or only one time
	protected override DATA_CUSTOM_TEST[] GetItems(){

		print( "<b>" + this.name + "</b>: get items (count=" + countItemsExample + ")" );

		// load data (example)
		DATA_CUSTOM_TEST[] array = new DATA_CUSTOM_TEST[countItemsExample];
		for( int i = 0; i < array.Length; i++ ){
			array[i] = new DATA_CUSTOM_TEST( "Item #" + (i + 1) + " (random=" + Random.Range( 5, 20 ) + ")" );
		}

		// return data
		return array;
	}

}