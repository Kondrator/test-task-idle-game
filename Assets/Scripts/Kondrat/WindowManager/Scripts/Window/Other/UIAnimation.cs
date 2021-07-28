using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


namespace UIWindowManager{

	public class UIAnimation : MonoBehaviour, IListenerLoaderSceneCompleted, IListenerLoaderSceneClosed {

		public enum EventType{
			WindowShowClose = 0,
			WindowShowCloseAnimation = 1,
			EnableDisable = 2,
			Programmacly = 3,
			LoadSceneCompleted = 4,
			LoadSceneClosed = 5
		}

		public enum CycleType{
			Switch = 0,
			RepeatShow = 1,
			RepeatHide = 2,
			ShowLast = 3,
			HideLast = 4
		}



		[SerializeField]
		public EventType eventType = EventType.WindowShowClose;

		// можно указать окно в инспекторе
		// если окно не указано, оно будет считано автоматически, поиском первого найденного в родителях
		[SerializeField]
		public UIWindow window;

		// cycle params
		[SerializeField]
		public bool isAutoCycle = false;
		[SerializeField]
		public float	waitCycleShow = 0, // before show
						waitCycleHide = 0; // before hide
		[SerializeField]
		public CycleType cycleType = CycleType.Switch;
		// TRUE - hide at awake; FALSE - show at awake (without animations)
		[SerializeField]
		public bool isHideAtAwake = true;


		
		public bool IsLastShowed{ get{ return animationSetLast == AnimationSetShow; } }
		public bool IsLastHided{ get{ return animationSetLast == AnimationSetHide; } }
		public bool IsVisible{ get{ return Canvas.alpha > 0; } }

	
		private RectTransform rect;
		private CanvasGroup canvas;
		public CanvasGroup Canvas{
			get{
				if( canvas == null ){
					canvas = this.AddComponent<CanvasGroup>();
				}
				return canvas;
			}
		}


		[SerializeField]
		protected AnimationSet animationSetShow = null;
		public AnimationSet AnimationSetShow{ 
			get{
				if( animationSetShow == null ){
					animationSetShow = AnimationSet.CreateFor( this.transform, "Show" );
				}
				return animationSetShow;
			}
		}
		[SerializeField]
		protected AnimationSet animationSetHide = null;
		public AnimationSet AnimationSetHide{
			get{
				if( animationSetHide == null ){
					animationSetHide = AnimationSet.CreateFor( this.transform, "Hide" );
				}
				return animationSetHide;
			}
		}
		private AnimationSet animationSetLast = null;

		
		[SerializeField]
		public bool isBlockRaycastWhenAnimation = true;


		// for UIAnimationQueueEditor
		public Component controlled;

		private TimerExecutor.Item timerWaitCycle = null;


		protected virtual void Awake(){

			rect = GetComponent<RectTransform>();
			if( rect == null ){
				Destroy( this );
				return;
			}

			canvas = this.AddComponent<CanvasGroup>();

			if( window == null ){
				window = GetComponentInParent<UIWindow>();
			}
		
			if( window != null ){
				window.OnOpen.AddListener( Window_OnShow );
				window.OnOpenAnimationCompleted.AddListener( Window_OnShowAnimationCompleted );
				window.OnClose.AddListener( Window_OnClose );
				window.OnCloseAnimationCompleted.AddListener( Window_OnCloseAnimationCompleted );
			}

			
			if( isHideAtAwake == true ){
				animationSetLast = AnimationSetHide;
				Stop();

			}else{
				animationSetLast = AnimationSetShow;
				Show( false, true );
			}

		}





		void Window_OnShow(){
			if( eventType == EventType.WindowShowClose ){
				Show();
			}
		}
		void Window_OnShowAnimationCompleted(){
			if( eventType == EventType.WindowShowCloseAnimation ){
				Show();
			}
		}

		void Window_OnClose(){
			if( eventType == EventType.WindowShowClose ){
				Hide();
			}
		}
		void Window_OnCloseAnimationCompleted(){
			if( eventType == EventType.WindowShowCloseAnimation ){
				Hide();
			}
		}



		void OnDestroy(){
			if( window != null ){
				window.OnOpen.RemoveListener( Window_OnShow );
				window.OnOpenAnimationCompleted.RemoveListener( Window_OnShowAnimationCompleted );
				window.OnClose.RemoveListener( Window_OnClose );
				window.OnCloseAnimationCompleted.RemoveListener( Window_OnCloseAnimationCompleted );
			}
		}


		// ON
		protected virtual void OnEnable(){

			StopWaitCycle();

			switch( eventType ){
				case EventType.WindowShowClose:
					if( window != null
						&& window.IsAwaked == true
						&& window.IsOpen == true
					){
						ActivateForcibly();
						Show();
					}
					break;

				case EventType.WindowShowCloseAnimation:
					if( window != null
						&& window.IsAwaked == true
						&& window.IsAnimationOpenCompleted == true
					){
						ActivateForcibly();
						Show();
					}
					break;

				case EventType.EnableDisable:
					ActivateForcibly();
					Show();
					break;
			}
		
		}
		// OFF
		protected virtual void OnDisable(){

			StopWaitCycle();

			if( eventType != EventType.Programmacly ){
				Stop();
			}

			switch( eventType ){
				case EventType.WindowShowClose:
					if( window != null
						&& window.IsAwaked == true
						&& window.IsOpen == true
					){
						ActivateForcibly();
						Hide();
					}
					break;

				case EventType.WindowShowCloseAnimation:
					if( window != null
						&& window.IsAwaked == true
						&& window.IsAnimationOpenCompleted == true
					){
						ActivateForcibly();
						Hide();
					}
					break;

				case EventType.EnableDisable:
					ActivateForcibly();
					Hide();
					break;
			}
		}


		public void OnLoadSceneCompleted() {
			switch( eventType ) {
				case EventType.LoadSceneCompleted:
					ActivateForcibly();
					Show();
					break;
			}
		}
		public void OnLoadSceneClosed() {
			switch( eventType ) {
				case EventType.LoadSceneClosed:
					ActivateForcibly();
					Show();
					break;
			}
		}




		private void ActivateForcibly(){
			if( AnimationSetShow != null ){
				AnimationSetShow.gameObject.ActivateForcibly();
			}
			if( AnimationSetHide != null ){
				AnimationSetHide.gameObject.ActivateForcibly();
			}
		}

	
		/// <summary>
		/// Stopped animation and set setting off animation.
		/// </summary>
		public virtual void Stop(){
			if( AnimationSetHide != null ){
				AnimationSetHide.Stop();
			}
			if( AnimationSetShow != null ){
				AnimationSetShow.Stop();
			}
			SetGameObjectActive( false );
		}


		/// <summary>
		/// Show / hide object animation.
		/// </summary>
		/// <param name="isVisible">True - show. False - hide.</param>
		public void Visible( bool isVisible, bool isFast ){
			if( isVisible == true ){
				Show( true, isFast );
		
			}else{
				Hide( true, isFast );
			}
		}

		/// <summary>
		/// Show / hide object animation.
		/// </summary>
		public void Visible( bool isVisible ) {
			Visible( isVisible, false );
		}

		/// <summary>
		/// Show / hide object animation with check for not show if showed and not hide if hide.
		/// </summary>
		/// <param name="isVisible">True - show. False - hide.</param>
		public void VisibleCheck( bool isVisible, bool isFast = false ){
			if( isVisible == true ){
				if( IsLastShowed == false ){
					Show( true, isFast );
				}
		
			}else{
				if( IsLastHided == false ){
					Hide( true, isFast );
				}
			}
		}


		/// <summary>
		/// Show.
		/// </summary>
		public virtual void Show( bool isCycle = true, bool isFast = false ){
			SetGameObjectActive( true );
			if( AnimationSetShow == null
				|| AnimationSetShow.isActiveAndEnabled == false
			){
				return;
			}

			System.Action dComplete = () =>{
				SetGameObjectActive( true );
				if( isCycle == true ){
					Cycle( true );
				}
			};

			if( isBlockRaycastWhenAnimation == true ){
				Canvas.blocksRaycasts = false;
			}
			
			StopWaitCycle();
			AnimationSetShow.Run( ref animationSetLast, dComplete, isFast );
			animationSetLast = AnimationSetShow;

		}

		/// <summary>
		/// Hide.
		/// </summary>
		public virtual void Hide( bool isCycle = true, bool isFast = false ){
			if( AnimationSetHide == null
				|| AnimationSetHide.isActiveAndEnabled == false
			){
				SetGameObjectActive( false );
				return;
			}

			System.Action dComplete = () =>{ 
				SetGameObjectActive( false );
				if( isCycle == true ){
					Cycle( false );
				}
			};

			SetGameObjectActive( true );

			if( isBlockRaycastWhenAnimation == true ){
				Canvas.blocksRaycasts = false;
			}

			StopWaitCycle();
			AnimationSetHide.Run( ref animationSetLast, dComplete, isFast );
			animationSetLast = AnimationSetHide;

		}


		/// <summary>
		/// Check start cycle animation.
		/// </summary>
		/// <param name="isAfter">Type animation end, when call this method.</param>
		protected virtual void Cycle( bool isAfter ){
			if( isAutoCycle == true ){

				// check type cycle
				bool isShow = isAfter;
				switch( cycleType ){

					case CycleType.Switch:
						isShow = !isAfter;
						break;


					case CycleType.RepeatShow:
						if( isShow == false ){
							return;
						}
						break;
					case CycleType.RepeatHide:
						if( isShow == true ){
							return;
						}
						break;


					case CycleType.ShowLast:
						isShow = !isAfter;
						if( isShow == false ){
							return;
						}
						break;
					case CycleType.HideLast:
						isShow = !isAfter;
						if( isShow == true ){
							return;
						}
						break;

				}

				StopWaitCycle();
				// check enable cycle
				if( isShow == true ){
					timerWaitCycle = TimerExecutor.Add( waitCycleShow, waitCycleShow + 1, ( float progress ) =>{}, () =>{
						Show( true );
					} );
				
				}else{
					timerWaitCycle = TimerExecutor.Add( waitCycleHide, waitCycleHide + 1, ( float progress ) =>{}, () =>{
						Hide( true );
					} );
				}
			}
		}
		private void StopWaitCycle(){
			if( timerWaitCycle != null ){
				timerWaitCycle.ForceComplete( false );
				timerWaitCycle = null;
			}
		}
	

		public virtual void SetGameObjectActive( bool isActive ){

			Canvas.Visible( isActive );
		
		}

		protected MonoBehaviour GetMonoBehaviour(){
			if( window != null ){
				return window;
			}
			return this;
		}

	}

}