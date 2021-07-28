using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


namespace UIWindowManager{

	[RequireComponent(typeof(UIDragHandler))]
	public class UIDragHandlerLongTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {


		private UIDragHandler drag;

		void Awake(){
			drag = GetComponent<UIDragHandler>();
			drag.enabled = false;

			drag.OnDragEndEvent += OnDragEnd;
		}

		void OnDragEnd(){
			drag.enabled = false;
		}




#region DragAndDrop At LongTouch

		// wait time long touch
		private float timeLongTouch = 500;
		private Vector2? posDown;

		// down
		public void OnPointerDown( PointerEventData eventData ){
			MyOperation.StopCoroutine( this, ref coroutineWaitLongTouch );
			coroutineWaitLongTouch = CoroutineWaitLongTouch();
			StartCoroutine( coroutineWaitLongTouch );
		}

		// up
		public void OnPointerUp( PointerEventData eventData ){
			posDown = null;
		}


		private IEnumerator coroutineWaitLongTouch;
		private IEnumerator CoroutineWaitLongTouch(){
			// check offset
			posDown = Input.mousePosition;

			yield return new WaitForSeconds( timeLongTouch / 1000 );

			// not have offset after wait
			if( posDown.HasValue == true
				&& Vector2.Distance( posDown.Value, Input.mousePosition ) < 5
			){
				OnLongTouch();
			
			}else{
				drag.enabled = false;
			}
		}

		/// <summary>
		/// Detected long touch.
		/// </summary>
		protected void OnLongTouch(){
			if( drag != null ){
				// enable DragAndDrop
				drag.enabled = true;
				// start DragAndDrop
				drag.DragBeginProgrammacly();
			}

		}

#endregion

	}

}