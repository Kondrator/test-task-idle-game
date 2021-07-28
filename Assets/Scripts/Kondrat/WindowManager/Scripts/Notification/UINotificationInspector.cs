using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	public class UINotificationInspector : UICreateFromInspector {


		[SerializeField, TextArea(1, 10)]
		private string text = "Text in notification";

	
		[SerializeField]
		private UINotification.TypeWait typeWait = UINotification.TypeWait.TimeAuto;
		[SerializeField, Range( 0.1f, 30f )]
		private float timeWait = 2f;
	

		public override void Create(){

			UINotification.CreateShow( text, typeWait, timeWait );

		}



	}

}