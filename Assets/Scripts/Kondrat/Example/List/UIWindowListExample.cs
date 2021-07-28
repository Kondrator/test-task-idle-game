using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWindowManager;

public class UIWindowListExample : UIWindowListArray<UIWindowListItemExample, DataExample> {

	[SerializeField]
	private int countItemsExample = 5; // for example!

	// only two method:
	// 1) setting each element
	// 2) get data

	// setting at item
	protected override void SettingItemExtra( UIWindowListItemExample container, DataExample item, int index ){
		container.Set( item );
	}


	// this method executed at open window or only one time
	protected override DataExample[] GetItems(){

		print( "<b>" + this.name + "</b>: get items (count=" + countItemsExample + ")" );

		// load data (example)
		DataExample[] array = new DataExample[countItemsExample];
		for( int i = 0; i < array.Length; i++ ){
			array[i] = new DataExample( "Item #" + (i + 1) + " (random=" + Random.Range( 5, 20 ) + ")" );
		}

		// return data
		return array;
	}

}

