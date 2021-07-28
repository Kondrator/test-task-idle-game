using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;



namespace UIWindowManager{

	public class UIWindowCap : MonoBehaviour, IBeginDragHandler, IDragHandler {


		RectTransform windowRect;

		void Start () {

			UIWindow window = GetComponentInParent<UIWindow>();
			if( window != null ){
				windowRect = window.GetComponent<RectTransform>();
			}

		}






		// draw window

		Vector3 offset;


		// begin
		#region IBeginDragHandler implementation
		public void OnBeginDrag( PointerEventData eventData ){

			if( windowRect == null ){
				return;
			}

			offset = Input.mousePosition;
			offset -= windowRect.position;

		}
		#endregion


		// move
		#region IDragHandler implementation
		public void OnDrag( PointerEventData eventData ){

			if( windowRect == null ){
				return;
			}

			Vector3 posMouse = Input.mousePosition - offset;
			posMouse.z = 0;

			windowRect.transform.position = posMouse;

		}
		#endregion

	}

}