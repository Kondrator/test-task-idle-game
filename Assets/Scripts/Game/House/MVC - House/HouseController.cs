using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HouseController : Controller<HouseModel, HouseView> {

	public class NOTIFY {

		public class TOUCH {

			public const string NAME = "house.touch";

		}

	}






	protected override void PreInitiate() { }

	protected override void Initiate() {



	}



	private void OnMouseDown() {

		if( MyOperationUI.IsCursorOverUI() == true ) {
			return;
		}

		Notify( NOTIFY.TOUCH.NAME );
	}

}
