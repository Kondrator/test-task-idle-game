using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;



namespace UIWindowManager{

	public class UIMessageBox : UIWindowTabControlSimple {

	
		[SerializeField]
		private Text info = null;

		[SerializeField]
		private Button bClose2, bYes = null, bNo = null, bCancel = null;

		// events
		public UnityEvent OnClickClose, OnClickYes, OnClickNo, OnClickCancel;





		private UIMessageBox(){}


	
		/// <summary>
		/// Create and show MessageBox.
		/// </summary>
		/// <param name="text">Text message.</param>
		/// <param name="buttons">Type buttons..</param>
		/// <param name="textButtons">Text buttons respectively.</param>
		/// <param name="isShow">Show before return?</param>
		/// <returns>New MessageBox.</returns>
		public static UIMessageBox CreateShow(	string text, 
												UIMessageBoxButtons buttons = UIMessageBoxButtons.Close,
												params string[] textButtons
											){
		
			// get prefab
			UIMessageBox boxLoad = UIData.MessageBoxPregab;
		
			// setting
			if( boxLoad != null ){
				// create new message box
				UIMessageBox boxCreate = MyOperation.InstantiateInCanvas<UIMessageBox>( boxLoad.gameObject, true, true );
				RectTransform rect = boxCreate.GetComponent<RectTransform>();
				rect.anchoredPosition = Vector2.zero;
				rect.sizeDelta = Vector2.zero;
			
				// event close
				boxCreate.OnCloseAnimationCompleted.AddListener( () =>{
					// destroy
					Destroy( boxCreate.gameObject );
				} );

			
				boxCreate.Setting( text, buttons, textButtons );
				boxCreate.Open();

				return boxCreate;
			}

			return null;
		}

		private void Reset(){
			OnClickClose = null;
			OnClickYes = null;
			OnClickNo = null;
			OnClickCancel = null;
			ClearAllEvents();
		
			SetButtonText( bClose, "Close" );
			SetButtonText( bYes, "Yes" );
			SetButtonText( bNo, "No" );
			SetButtonText( bCancel, "Cancel" );
		}


		/// <summary>
		/// Setting window.
		/// </summary>
		/// <param name="text">Text message.</param>
		/// <param name="buttons">Type buttons..</param>
		/// <param name="textButtons">Text buttons respectively.</param>
		public void Setting( string text, UIMessageBoxButtons buttons = UIMessageBoxButtons.Close, params string[] textButtons ){

			if( info != null ) info.text = text;

			SetButtons( buttons, textButtons );

		}



		/// <summary>
		/// Set buttons.
		/// </summary>
		public void SetButtons( UIMessageBoxButtons buttons, params string[] textButtons ){

			switch( buttons ){
				case UIMessageBoxButtons.Close:
					indexTabAtOpen = 0;
					SetButtonText( bClose2, textButtons, 0 );
					bClose2.onClick.AddListener( () => { if( OnClickClose != null ) OnClickClose.Invoke(); Close(); } );
					break;
				
				case UIMessageBoxButtons.YesNo: case UIMessageBoxButtons.YesNoCancel:
					indexTabAtOpen = 1;
					SetButtonText( bYes, textButtons, 0 );
					bYes.onClick.AddListener( () => { if( OnClickYes != null ) OnClickYes.Invoke(); Close(); } );
					SetButtonText( bNo, textButtons, 1 );
					bNo.onClick.AddListener( () => { if( OnClickNo != null ) OnClickNo.Invoke(); Close(); } );

					// проверка на отображение кнопки отмены
					bool isCancel = buttons == UIMessageBoxButtons.YesNoCancel;
					bCancel.gameObject.SetActive( isCancel );
					if( isCancel == true ){
						SetButtonText( bCancel, textButtons, 2 );
						bCancel.onClick.AddListener( () => { if( OnClickCancel != null ) OnClickCancel.Invoke(); Close(); } );
					}
					break;
			}

			isShowTabAtOpen = true;
			ShowAt( indexTabAtOpen );

		}



	
		/// <summary>
		/// Set text to button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="textButtons">Array texts.</param>
		/// <param name="index">Index of need text from array.</param>
		private void SetButtonText( Button button, string[] textButtons, int index ){
			if( button == null || textButtons == null || index < 0 || index >= textButtons.Length ) return;
			
			SetEnableLocalization( button, false );
			button.GetComponentInChildren<Text>().text = textButtons[index];
		}

		/// <summary>
		/// Set text to button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="text">Text.</param>
		private void SetButtonText( Button button, string text ){
			button.GetComponentInChildren<Text>().text = text;
			SetEnableLocalization( button, true );
		}

		private void SetEnableLocalization( Button button, bool isEnable ){
			/*
			GameToolkit.Localization.LocalizedTextBehaviour localization = button.GetComponentInChildren<GameToolkit.Localization.LocalizedTextBehaviour>();
			if( localization != null ){
				localization.enabled = isEnable;
			}
			*/
		}


	}



	/// <summary>
	/// Type buttons.
	/// </summary>
	public enum UIMessageBoxButtons{
		/// <summary>
		/// Button "Close".
		/// </summary>
		Close,
		/// <summary>
		/// Two buttons: "Yes" и "No".
		/// </summary>
		YesNo,
		/// <summary>
		/// Three buttons: "Yes", "No" и "Cancel".
		/// </summary>
		YesNoCancel
	}

}