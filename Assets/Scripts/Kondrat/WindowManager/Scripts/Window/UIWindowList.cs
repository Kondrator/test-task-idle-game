using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace UIWindowManager{

	/// <summary>
	/// This is auto generate items Monobehaviors.
	/// </summary>
	/// <typeparam name="T">Type of generate item (GameObject UI).</typeparam>
	public abstract class UIWindowList<T> : UIWindow, IMyManagerMonoBehaviour where T : MonoBehaviour {

		// container for items
		[SerializeField]
		public Transform list = null;
		[SerializeField]
		private Transform showAtEmptyList = null;
		[SerializeField]
		private ScrollRect scroll = null;
		[SerializeField]
		private T itemList = null;

		[Header( "Load" )]
		[SerializeField]
		private bool reloadListAtOpen = false;
		[SerializeField]
		private bool reloadListAtEnable = false;
		[SerializeField]
		[Range( 1, 10 )]
		private int loadItemsByFrame = 1;


		private IEnumerator coroutineReload = null;
		private CanvasGroup canvasGroupList = null;
		private T[] itemsUILast = null;

		protected bool isFixScrollAlways = false;


		protected override void OnAwake(){
			base.OnAwake();

			if( list != null ){
				canvasGroupList = list.AddComponent<CanvasGroup>();
			}

			OnOpen.AddListener( () =>{
				FixScroll();
				if( reloadListAtOpen == true ){
					Reload();
				}

				MyManagerMonoBehaviour.Add( this );
			} );

			OnClose.AddListener( () => {
				MyManagerMonoBehaviour.Remove( this );
			} );
		}

		public override void CopyFieldsFrom( UIWindow window ){
			base.CopyFieldsFrom( window );

			if( window is UIWindowList<T> ){
				UIWindowList<T> windowList = (UIWindowList<T>)window;
				this.list = windowList.list;
				this.showAtEmptyList = windowList.showAtEmptyList;
				this.scroll = windowList.scroll;
				this.itemList = windowList.itemList;
				this.reloadListAtOpen = windowList.reloadListAtOpen;
			}
		}


		protected override void OnEnable() {
			base.OnEnable();

			if( reloadListAtEnable == true ) {
				Reload();
			}
		}


		public void UpdateMe( float timeDelta, MyManagerMonoBehaviourType type ) {
			if( list != null ) {
				bool isEmpty = list.childCount == 0;

				if( isEmpty != GetEmptyVisible() ) {
					SetEmptyVisible( isEmpty );
				}
			}
		}



		/// <summary>
		/// Clear items.
		/// </summary>
		public virtual void Clear(){
		
			if( list != null ){
				MyOperation.DeactivateAllChild( list );
			}

		}


		public T GetItemUI( int index ){
			if( itemsUILast == null ){
				return null;
			}

			return itemsUILast[index];
		}



		protected virtual void FixScrollAfter() {}
		protected void FixScroll(){
			if( scroll != null ){
				if( isFixScrollAlways == false
					&& IsAnimationOpenCompleted == true
				){
					return;
				}

				if( scroll.vertical == true ){
					scroll.verticalNormalizedPosition = 1f;

				}else if( scroll.horizontal == true ){
					scroll.horizontalNormalizedPosition = 0;
				}
			}
		}
		protected void ScrollTo( int index ){
			if( scroll != null
				&& index >= 0 && index < GetCount()
			){
				if( scroll.vertical == true ) {
					scroll.verticalNormalizedPosition = (float)index / (float)GetCount();

				} else if( scroll.horizontal == true ) {
					scroll.horizontalNormalizedPosition = (float)index / (float)GetCount();
				}

				/*
				T itemUI = GetItemUI( index );
				if( itemUI == null ) {
					return;
				}

				T itemFirstUI = GetItemUI( 0 );
				if( itemFirstUI == null ) {
					return;
				}

				if( scroll.vertical == true ) {
					float itemFirstPosition = itemFirstUI.transform.localPosition.y;
					float itemPosition = Mathf.Abs( itemUI.transform.localPosition.y - itemFirstPosition );
					float contentHeight = ((RectTransform)scroll.content.transform).rect.height + itemFirstPosition + itemFirstPosition * ((float)index / (float)GetCount());
					scroll.verticalNormalizedPosition = 1f - ( itemPosition / contentHeight );

				}else if( scroll.horizontal == true ) {
					float itemFirstPosition = itemFirstUI.transform.localPosition.x;
					float itemPosition = Mathf.Abs( itemUI.transform.localPosition.x - itemFirstPosition );
					float contentWidth = ((RectTransform)scroll.content.transform).rect.width + itemFirstPosition + itemFirstPosition * ((float)index / (float)GetCount());
					scroll.horizontalNormalizedPosition = 1f - ( itemPosition / contentWidth);
				}
				*/
			}
		}
	




		/// <summary>
		/// Reload items.
		/// </summary>
		public void Reload(){
			if( isActiveAndEnabled == true ){
				MyOperation.StopCoroutine( this, ref coroutineReload );
				coroutineReload = CoroutineReload();
				StartCoroutine( coroutineReload );
			}
		}

		/// <summary>
		/// Coroutine reload items.
		/// </summary>
		protected IEnumerator CoroutineReload(){

			T item = itemList;
			if( item == null ){
				yield break;

			}else if( item.gameObject.activeInHierarchy == true ){
				item.gameObject.Deactivate();
			}

			FixScroll();

			BeforeGenerate();
			int count = GetCount();

			// read current items
			T[] itemsCur = MyOperation.GetComponentsInChildren<T>( list );
			itemsUILast = new T[count];

			for( int i = 0; i < count; i++ ){

				// setting current
				if( i < itemsCur.Length ){
					SettingItem( itemsCur[i], i );
					itemsUILast[i] = itemsCur[i];
					continue;
				}

				if( i % loadItemsByFrame == 0 ) {
					yield return new WaitForEndOfFrame();
				}
				
				// create new
				T containerNew = PoolGameObject.Get<T>( item.gameObject, list );
				containerNew.gameObject.FixScale();
				containerNew.gameObject.Activate();
				SettingItem( containerNew, i );

				itemsUILast[i] = containerNew;

			}

			// clear excess
			MyOperation.DeactivateAllChild( list, count );


			AfterGenerate();
			
			yield return new WaitForEndOfFrame();
			FixScroll();
			FixScrollAfter();


			SetEmptyVisible( count == 0 );

		}


		private void SetEmptyVisible( bool isVisible ) {

			if( showAtEmptyList != null ) {

				if( canvasGroupList != null ) {
					canvasGroupList.alpha = isVisible ? 0 : 1f;

				} else {
					list.gameObject.SetActive( !isVisible );
				}

				showAtEmptyList.gameObject.SetActive( isVisible );
			}

		}

		private bool GetEmptyVisible() {

			if( showAtEmptyList != null ) {
				return showAtEmptyList.gameObject.activeSelf;
			}

			return false;
		}



		/// <summary>
		/// Count need generated items.
		/// </summary>
		public abstract int GetCount();
		/// <summary>
		/// Setting item in generate.
		/// </summary>
		/// <param name="item">This item need Setting?</param>
		/// <param name="index">Index in list.</param>
		protected abstract void SettingItem( T item, int index );

		/// <summary>
		/// Callback before generate items.
		/// </summary>
		protected virtual void BeforeGenerate(){}
		/// <summary>
		/// Callback after generate items.
		/// </summary>
		protected virtual void AfterGenerate(){}

	}

}