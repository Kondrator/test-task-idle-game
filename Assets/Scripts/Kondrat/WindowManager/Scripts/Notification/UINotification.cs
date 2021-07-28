using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace UIWindowManager{

	public class UINotification : UIWindow {

		public enum TypeWait{
			/// <summary>
			/// Wait need time.
			/// </summary>
			TimeEnter,
			/// <summary>
			/// Wait time at length text.
			/// </summary>
			TimeAuto
		}


		[SerializeField]
		private Text text;
		// example need wait extra animations
		[SerializeField, Range(0, 2)]
		private float timeWaitExtra = 0;

		private float timeWait = 1f;


		protected override void OnAwake(){
			base.OnAwake();
		
			OnOpenAnimationCompleted.AddListener( () =>{
				MyOperation.ExecuteAtNextUpdate( this, () =>{ Close(); }, timeWait + timeWaitExtra );
			} );

			OnCloseAnimationCompleted.AddListener( () =>{
				if( IsFirstOnShow == false ){
					// for pooling
					this.gameObject.Deactivate();
				}
			} );

		}

		public override void CopyFieldsFrom( UIWindow window ){
			base.CopyFieldsFrom( window );
		
			UINotification notification = window as UINotification;
			if( notification != null ){
				text = notification.text;
			}
		}


		/// <summary>
		/// Show notification at screen.
		/// </summary>
		/// <param name="text">Text.</param>
		public void Show( string text, TypeWait typeWait = TypeWait.TimeAuto, float timeWait = 2f ){
		
			if( this.text == null/* || uiAnimation == null */){
				return;
			}

			this.text.text = text;
			this.timeWait = timeWait;

			if( typeWait == TypeWait.TimeAuto ){
				this.timeWait = text.Length / 15f;
			}

			
			//AnimationSetClose.ReAwakeStart();
			//AnimationSetOpen.ReAwakeStart();
			Open();

		}



	
		/// <summary>
		/// Create and show notification at screen.
		/// </summary>
		/// <param name="text">Text.</param>
		public static UINotification CreateShow( string text, TypeWait typeWait = TypeWait.TimeAuto, float timeWait = 2f ){
		
			UINotification notification = UIData.NotificationPrefab;

			if( notification != null ){
				notification = PoolGameObject.Get<UINotification>( notification.gameObject, MyOperationUI.FindCanvas().transform );
				notification.gameObject.FixSize();

				notification.Show( text, typeWait, timeWait );

				return notification;
			}

			return null;
		}

		
		/// <summary>
		/// Create and show notification at screen above loader scene.
		/// </summary>
		/// <param name="text">Text.</param>
		public static UINotification CreateShowAboveLoader( string text, TypeWait typeWait = TypeWait.TimeAuto, float timeWait = 2f ){

			// setting notification
			UINotification notification = UINotification.CreateShow( text, typeWait, timeWait );

			// to top (above loader screen)
			Canvas canvas = notification.AddComponent<Canvas>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = 10000;
			GraphicRaycaster graphicRaycaster = notification.AddComponent<GraphicRaycaster>();

			UnityAction listenerClose = null;
			listenerClose = () =>{
				notification.OnCloseAnimationCompleted.RemoveListener( listenerClose );
				Destroy( canvas );
				Destroy( graphicRaycaster );
			};
			notification.OnCloseAnimationCompleted.AddListener( listenerClose );

			return notification;
		}


	}

}