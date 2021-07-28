using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


namespace UIWindowManager{

	public class UIMessageBoxInspector : UICreateFromInspector {

		[SerializeField]
		private UIMessageBoxButtons typeButtons = UIMessageBoxButtons.Close;

		[SerializeField, TextArea(1, 10)]
		private string textMessage = "Message";

		[SerializeField]
		private string	textButtonClose = "Close",
						textButtonYes = "Yes",
						textButtonNo = "No",
						textButtonCancel = "Cancel";

	

		[SerializeField]
		private Button.ButtonClickedEvent onClickClose = null;
		[SerializeField]
		private Button.ButtonClickedEvent onClickYes = null;
		[SerializeField]
		private Button.ButtonClickedEvent onClickNo = null;
		[SerializeField]
		private Button.ButtonClickedEvent onClickCancel = null;






		/// <summary>
		/// Create and show message box with need params.
		/// </summary>
		public override void Create(){
		
			// prepare texts for need buttons
			string[] buttonsText = new string[0];
			switch( typeButtons ){
				case UIMessageBoxButtons.Close:
					buttonsText = new string[]{ textButtonClose };
					break;

				case UIMessageBoxButtons.YesNo:
					buttonsText = new string[]{ textButtonYes, textButtonNo };
					break;

				case UIMessageBoxButtons.YesNoCancel:
					buttonsText = new string[]{ textButtonYes, textButtonNo, textButtonCancel };
					break;
			}

			// create box
			UIMessageBox box = UIMessageBox.CreateShow( textMessage, typeButtons, buttonsText );
			// setting events
			box.OnClickClose.AddListener( () =>{ ObserverClick( onClickClose ); } );
			box.OnClickYes.AddListener( () =>{ ObserverClick( onClickYes ); } );
			box.OnClickNo.AddListener( () =>{ ObserverClick( onClickNo ); } );
			box.OnClickCancel.AddListener( () =>{ ObserverClick( onClickCancel ); } );

		}

		private void ObserverClick( Button.ButtonClickedEvent onClick ){
			if( onClick != null ){
				onClick.Invoke();
			}
		}

	}

}