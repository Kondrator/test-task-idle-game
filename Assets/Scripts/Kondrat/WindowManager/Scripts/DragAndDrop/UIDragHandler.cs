using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;



namespace UIWindowManager{

	//[RequireComponent(typeof(CanvasGroup))]
	public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{

		/// <summary>
		/// Type begin Drag&Drop.
		/// </summary>
		public enum UIDragHandlerBeginType{
			/// <summary>
			/// Simple drag.
			/// </summary>
			Simple,
			/// <summary>
			/// Drag after long touch.
			/// </summary>
			LongTouch
		}





		[SerializeField]
		private UIDragHandlerBeginType typeBeginDrag = UIDragHandlerBeginType.Simple;
		// true - dragged copy object
		[SerializeField]
		private bool isDragClone = false;
		// multiply scale dragged object
		[SerializeField]
		private float kSizeDrag = 0.9f;
		[SerializeField, Tooltip( "True - destroy at end drop. False - deactivate." )]
		private bool isDestroyAtDrop = true;



		// position item
		private RectTransform rectTransform;
		// for DROP
		private CanvasGroup canvasGroup;




		// event begin drag
		public System.Action OnDragStartEvent;
		// event drag
		public System.Action OnDragEvent;
		// event end drag
		public System.Action OnDragEndEvent;




		public bool IsDragClone{
			get{ return isDragClone; }
			set{ isDragClone = value; }
		}


		/// <summary>
		/// Clear all events.
		/// </summary>
		public void ClearEvents(){
			OnDragStartEvent = null;
			OnDragEvent = null;
			OnDragEndEvent = null;
		}




		void Awake(){

			rectTransform = GetComponent<RectTransform>();
			canvasGroup = GetComponent<CanvasGroup>();
			if( canvasGroup != null ){
				canvasGroup.blocksRaycasts = true;
			}

			if( typeBeginDrag == UIDragHandlerBeginType.LongTouch ){
				this.AddComponent<UIDragHandlerLongTouch>();
				this.enabled = false;
			}
		
		}

		void OnDisable(){
			// now dragg object
			if( itemDraggedSource == this ){
				OnEndDrag( null );
			}
		}

		private void SetScroll( bool isEnable ){
			ScrollRect scroll = GetComponentInParent<ScrollRect>();
			if( scroll != null ) scroll.enabled = isEnable;
		}





		// programmacly drag
		bool isDragProgrammacly = false;
		public bool IsDragProgrammacly{ get{ return isDragProgrammacly; } }
		/// <summary>
		/// Start DragAndDrop.
		/// </summary>
		public void DragBeginProgrammacly(){

			OnDisable();

			isDragProgrammacly = true;
			OnBeginDrag( null );
		}
		void Update(){

			if( isDragProgrammacly == true ){
			
				// end of drag
				if( itemDragged == null 
					|| Input.GetMouseButtonUp( 0 ) == true
				){
					OnEndDrag( null );
					isDragProgrammacly = false;

				}else{
					OnDrag( null );
				}

			}

		}






		// for drag UI element
		/// <summary>
		/// Have drag now?
		/// </summary>
		public static bool IsDrag{
			get{
				return itemDragged != null;
			}
		}

		/// <summary>
		/// Dragged objct (this check id is drag).
		/// </summary>
		public static UIDragHandler itemDragged;
		/// <summary>
		/// Dragged from this object (if dragged clone - this is from clone).
		/// </summary>
		public static UIDragHandler itemDraggedSource;
		/// <summary>
		/// Parent at dragged object (before begin).
		/// </summary>
		public static Transform parent;
		/// <summary>
		/// Parent itemDraggedSource.
		/// </summary>
		public static Transform parentSource;

	
		private static int siblingIndex;
		/// <summary>
		/// Sibling index from dragging.
		/// </summary>
		public static int SiblingIndex{
			get{ return siblingIndex; }
		}



		// begin params
		private Vector2 positionStart, sizeBegin, anchorMin, anchorMax, pivot;





		public static void DestroyDrag( bool isSendEventDrop = false ){

			if( itemDragged != null ){

				if( isSendEventDrop == false ){
					// if not NULL - send event in OnEndDrag
					itemDraggedSource = null;
				}
			
				// fix item dragged
				FixDropItemDragged();

				// save in variable
				UIDragHandler itemDraggedTemp = itemDragged;

				// clear for not usability in OnEndDrag
				itemDragged = null;

		
				if( itemDraggedTemp.isDestroyAtDrop == true ){
					// destroy
					Destroy( itemDraggedTemp.gameObject );

				}else{
					// deactivate
					itemDraggedTemp.transform.SetParent( null );
					itemDraggedTemp.gameObject.SetActive( false );
				}

			}

		}

	
	

		// begin
		#region IBeginDragHandler implementation
		public void OnBeginDrag( PointerEventData eventData ){

			SetScroll( true );

			// end current
			//if( itemDragged != null ) OnEndDrag( eventData );


			// clone
			if( isDragClone == true ){
				// create clone and save
				itemDragged = MyOperation.InstantiateClone<UIDragHandler>( this.gameObject );

			}else{
				// save current
				itemDragged = this;

			}

			// save source
			itemDraggedSource = this;
			// save sibling index 
			siblingIndex = itemDraggedSource.transform.GetSiblingIndex();

			SetScroll( false );


			// save parent
			parent = itemDragged.transform.parent;
			parentSource = this.transform.parent;


			// save begin params
			anchorMin = itemDragged.rectTransform.anchorMin;
			anchorMax = itemDragged.rectTransform.anchorMax;
			pivot = itemDragged.rectTransform.pivot;
			positionStart = itemDragged.rectTransform.anchoredPosition;
			sizeBegin = itemDragged.rectTransform.sizeDelta;



			// scale param
			itemDragged.rectTransform.sizeDelta = new Vector2( 
				itemDraggedSource.rectTransform.rect.width * kSizeDrag, 
				itemDraggedSource.rectTransform.rect.height * kSizeDrag 
			);

			// need params
			itemDragged.rectTransform.anchorMin = new Vector2( 0, 1 );
			itemDragged.rectTransform.anchorMax = new Vector2( 0, 1 );

			try{
			
				// reset parent
				itemDragged.transform.SetParent( GameObject.FindGameObjectWithTag( "UICanvasMain" ).transform );

			}catch{

				Debug.LogError( "Не назначен тег \"UICanvasMain\" для корневого Canvas." );

			}


			// from DROP
			itemDragged.canvasGroup.blocksRaycasts = false;



			// observer
			if( itemDraggedSource != null 
				&& itemDraggedSource.OnDragStartEvent != null )
				itemDraggedSource.OnDragStartEvent();

		}
		#endregion


		// drag
		#region IDragHandler implementation
		public void OnDrag( PointerEventData eventData ){
		
			// cursor position
			Vector3 posMouse = Input.mousePosition;
			posMouse.z = 0;

			itemDragged.transform.position = posMouse;

			// observer
			if( itemDraggedSource != null 
				&& itemDraggedSource.OnDragEvent != null
			){
				itemDraggedSource.OnDragEvent();
			}

		}
		#endregion


		// end
		#region IEndDragHandler implementation
		public void OnEndDrag( PointerEventData eventData ){
		
			// observer
			if( itemDraggedSource != null 
				&& itemDraggedSource.OnDragEndEvent != null
			){
				itemDraggedSource.OnDragEndEvent();
			}



			// not have drop
			if( itemDragged != null
				&& ( 
					itemDragged.transform.parent == null
					|| itemDragged.transform.parent.tag == "UICanvasMain"
				)
			){
			
				// dragged original
				if( isDragClone == false ){

					// return back
					itemDragged.transform.SetParent( parent );
					itemDragged.transform.SetSiblingIndex( siblingIndex );
				
				}
			}


			if( itemDragged != null ){
			
				FixDropItemDragged();

				if( isDragClone == true ){

					DestroyDrag();

				}

			}



			// reset params
			itemDragged = null;
			itemDraggedSource = null;
			parent = null;
			parentSource = null;
			siblingIndex = 0;

			isDragProgrammacly = false;


			SetScroll( true );
		
		}

		private static void FixDropItemDragged(){
		
			if( itemDragged != null ){
			
				// restore params
				itemDragged.rectTransform.anchorMin = itemDragged.anchorMin;
				itemDragged.rectTransform.anchorMax = itemDragged.anchorMax;
				itemDragged.rectTransform.pivot = itemDragged.pivot;
				itemDragged.rectTransform.anchoredPosition = itemDragged.positionStart;
				itemDragged.rectTransform.sizeDelta = itemDragged.sizeBegin;

				// from DRAG
				itemDragged.canvasGroup.blocksRaycasts = true;

			}

		}

		#endregion



	}

}