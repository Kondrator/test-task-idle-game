using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UIWindowManager{

	public class UIWindowContext : UIWindow {


		public bool isNextOpenedIsBlockCloseAtTouch = false;

		public override void CopyFieldsFrom( UIWindow window ){
			base.CopyFieldsFrom( window );

			if( window is UIWindowContext ){
				isNextOpenedIsBlockCloseAtTouch = ((UIWindowContext)window).isNextOpenedIsBlockCloseAtTouch;
			}
		}


		protected override void OnAwake(){
			base.OnAwake();

			OnClose.AddListener( () =>{
				isNextOpenedIsBlockCloseAtTouch = false;
			} );
		}


		protected virtual void Update(){
			if( IsOpen == false ){
				return;
			}

			if( isNextOpenedIsBlockCloseAtTouch == false
				&& Input.GetMouseButtonDown( 0 ) 
				&& MyOperationUI.IsCursorOverUI( content ) == false
			){
				Close();
			}
		}

	}

}