using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace UIWindowManager{

	public class UIHandlerLongTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		
		// wait time long touch
		[SerializeField, Range(250, 1500)]
		private float timeLongTouchMilliSeconds = 500;
		private Vector2? posDown = null;

		public UnityEvent OnLongTouch = new UnityEvent();

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

			yield return new WaitForSeconds( timeLongTouchMilliSeconds / 1000f );

			// not have offset after wait
			if( posDown.HasValue == true
				&& Vector2.Distance( posDown.Value, Input.mousePosition ) < Screen.width * 0.05f
			){
				OnLongTouch.Invoke();
			}
		}

	}

}