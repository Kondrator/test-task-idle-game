using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace UIWindowManager {

	[RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
	public class UIWindow : Kondrat.MVC.Element, IListenerLoaderSceneCompleted {

		private static List<UIWindow> windows = null;
		public static List<UIWindow> Windows{
			get{
				if( windows == null ){
					windows = new List<UIWindow>();
				}
				return windows;
			}
		}
		private static Dictionary<string, UIWindow> windowsIDS = null;
		private static Dictionary<string, UIWindow> WindowsIDS{
			get{
				if( windowsIDS == null ){
					windowsIDS = new Dictionary<string, UIWindow>();
				}
				return windowsIDS;
			}
		}
		/// <summary>
		/// Count opened windows in scene.
		/// </summary>
		public static int CountWindowsOpened{
			get{
				int count = 0;
				for( int i = 0; i < Windows.Count; i++ ){
					if( Windows[i].IsOpen == true
						&& Windows[i].isStatic == false
						&& Windows[i].gameObject.activeInHierarchy == true
					){
						count++;
					}
				}
				return count;
			}
		}
		public static bool HasOpened{ get{ return CountWindowsOpened > 0; } }

		/// <summary>
		/// Get window in scene at id.
		/// </summary>
		/// <param name="id">Id of need window.</param>
		public static UIWindow GetAtID( string id ){
			if( string.IsNullOrEmpty( id ) ){
				return null;
			}
			if( WindowsIDS.ContainsKey( id ) ){
				return WindowsIDS[id];
			}
			return null;
		}


#if UNITY_EDITOR
		public static string[] FindIDs() {

			List<string> result = new List<string>();

			string[] pathes = AssetDatabase.FindAssets( "t:Prefab" );
			for( int i = 0; i < pathes.Length; i++ ) {
				GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>( AssetDatabase.GUIDToAssetPath( pathes[i] ) );
				if( go == null ) {
					continue;
				}

				UIWindow[] windows = go.GetComponentsInChildren<UIWindow>();
				for( int w = 0; w < windows.Length; w++ ) {
					if( string.IsNullOrEmpty( windows[w].id ) == false ) {
						result.Add( windows[w].id );
					}
				}
			}

			return result.ToArray();
		}


		private static string[] windowIDs = null;
		private static string[] WindowsIDs { get { return windowIDs = windowIDs ?? FindIDs().InsertItem( 0, "find" ); } }

		public static string PopupID( string id ) {

			// window id
			using( new EditorGUILayout.HorizontalScope() ) {
				id = EditorGUILayout.TextField( "Window ID", id );

				int iSelectID = EditorGUILayout.Popup( GUIContent.none, 0, WindowsIDs, GUILayout.Width( 45 ) );
				if( iSelectID > 0 ) {
					id = WindowsIDs[iSelectID];
				}
			}

			return id;
		}
#endif







		// content at window
		[SerializeField]
		public GameObject content;

		// id at window
		[SerializeField]
		public string id = null;

		[SerializeField]
		public Button bClose;

		// sound at open/close window
		[SerializeField]
		public AudioClip soundOpen, soundClose;



		public enum AwakeType{
			// close window at awake without animation
			HideFastOnAwake = 0,
			// open window at awake without animation
			OpenFastOnAwake = 1,
			// open window on loaded scene with animation
			OpenOnLoadedScene = 2
		}
		[SerializeField]
		public AwakeType typeAwake = AwakeType.HideFastOnAwake;
		// close window at key ESCAPE ?
		[SerializeField]
		public bool isCloseEscape = true;
		// true - window set as last sibling at show
		[SerializeField]
		public bool isToTopAtOpen = true;
		// true - window set as last sibling at touch
		[SerializeField]
		public bool isToTopAtTouch = true;


		// block cursror
		[SerializeField]
		public bool isCursorBlock = false;
		/// <summary>
		/// True - block cursor, when window is open.
		/// False - block cursor, when window is close.
		/// </summary>
		[SerializeField]
		public bool isCursorBlockWhenOpen = false;



		// open at target key
		[SerializeField]
		public bool isOpenAtKey = false;
		// close at target key
		[SerializeField]
		public bool isCloseAtKey = true;
		[SerializeField]
		public KeyCode keyTarget = KeyCode.Alpha1;
	
		private bool isAwaked = false;
		public bool IsAwaked{ get{ return isAwaked; } }



		/// <summary>
		/// Copy fields to this from target.
		/// Events dont copy.
		/// </summary>
		/// <param name="window">Target window.</param>
		public virtual void CopyFieldsFrom( UIWindow window ){
		
			this.isAwaked = window.isAwaked;
			this.content = window.content;
			this.bClose = window.bClose;
			this.soundOpen = window.soundOpen;
			this.soundClose = window.soundClose;
			this.typeAwake = window.typeAwake;
			this.isCloseEscape = window.isCloseEscape;
			this.isToTopAtOpen = window.isToTopAtOpen;

			this.rectWindow = window.rectWindow;
			this.rectContent = window.rectContent;
			this.isOpen = window.isOpen;
			this.isFirstAnimation = window.isFirstAnimation;
			this.isAnimationOpenCompleted = window.isAnimationOpenCompleted;

		}

		/// <summary>
		/// Create new component of window on this GameObject.
		/// After copy fields from this window to NEW window.
		/// And after destroy this window.
		/// Return new window.
		/// </summary>
		/// <typeparam name="T">Type of new window.</typeparam>
		/// <returns>Replaced window.</returns>
		public T Replace<T>() where T : UIWindow{
			T windowNew = this.AddComponent<T>();
			windowNew.CopyFieldsFrom( this );
			Destroy( this );
			return windowNew;
		}



	
		// params window
		protected RectTransform rectWindow, rectContent;
		protected CanvasGroup canvasGroupContent;
		protected CanvasGroup CanvasGroupContent{
			get{
				if( canvasGroupContent == null ){
					canvasGroupContent = content.transform.AddComponent<CanvasGroup>();
				}
				return canvasGroupContent;
			}
		}
		public RectTransform RectWindow{
			get{
				return rectWindow;
			}
		}
		public RectTransform RectContent{
			get{
				return rectContent;
			}
		}
		// state window
		[SerializeField, HideInInspector]
		private bool isOpen;

		/// <summary>
		/// True - ignore in "CountWindowsOpened"
		/// </summary>
		[SerializeField]
		public bool isStatic = false;

		/// <summary>
		/// False - animation was. True - animation was not.
		/// </summary>
		private bool isFirstAnimation = true;
		/// <summary>
		/// isFirstOnShow == true in first event OnShow.
		/// Other events OnShow isFirstOnShow == false.
		/// </summary>
		private bool isFirstOnShow = true;
		public bool IsFirstOnShow{ get{ return isFirstOnShow; } }

		// animation open is completed?
		private bool isAnimationOpenCompleted = false;
		public bool IsAnimationOpenCompleted{ get{ return isAnimationOpenCompleted; } }

		// resolution for open / close window when have curreny animation open close
		public bool isOpenCloseAtAnimation = true;
	
		[SerializeField]
		protected AnimationSet animationSetOpen = null;
		public AnimationSet AnimationSetOpen{ 
			get{
				if( animationSetOpen == null ){
					animationSetOpen = AnimationSet.CreateFor( this.transform, "Open" );
				}
				return animationSetOpen;
			}
		}
		[SerializeField]
		protected AnimationSet animationSetClose = null;
		public AnimationSet AnimationSetClose{
			get{
				if( animationSetClose == null ){
					animationSetClose = AnimationSet.CreateFor( this.transform, "Close" );
				}
				return animationSetClose;
			}
		}

		private AnimationSet animationSetLast = null;



		// events
		public UnityEvent OnOpen, OnClose, OnCloseOnEscape;
		public UnityEvent OnOpenAnimationCompleted, OnCloseAnimationCompleted;

		[System.Serializable]
		public class UnityActionType : UnityEvent<System.Type>{}
		public UnityActionType OnOpenAnimationCompletedType;



		protected void ClearAllEvents(){
			OnOpen = null;
			OnClose = null;
			OnOpenAnimationCompleted = null;
			OnCloseAnimationCompleted = null;
			OnOpenAnimationCompletedType = null;
		}




		/// <summary>
		/// Window is open?
		/// </summary>
		public bool IsOpen{
			get{
				return isOpen;
			}
#if UNITY_EDITOR
			set{
				isOpen = value;
			}
#endif
		}
		/// <summary>
		/// Content window is visible?
		/// </summary>
		public bool IsContentActive{
			get{
				if( content == null ){
					return false;
				}
				return content.activeInHierarchy;
			}
		}



		/// <summary>
		/// Check (if need) key target and open/close window.
		/// </summary>
		public virtual void CheckKey(){
			if( isOpenAtKey == true ){
				if( Input.GetKeyDown( keyTarget ) ){
					if( IsOpen == false ){
						Open();

					}else if( isCloseAtKey ){
						Close();
					}
				}
			}
		}


		protected void Awake(){

			Windows.Add( this );
			if( id != null ){
				WindowsIDS[id] = this;
			}

			rectWindow = GetComponent<RectTransform>();
			if( content != null ){
				rectContent = content.GetComponent<RectTransform>();
				canvasGroupContent = content.transform.AddComponent<CanvasGroup>();
			}

			// current state window
			isOpen = content.activeSelf;
			
			Close( false );
		
			if( bClose != null ){
				bClose.onClick.RemoveListener( () =>{ Close(); } );
				bClose.onClick.AddListener( () =>{ Close(); } );
			}



			// for close at escape
			if( isCloseEscape == true ){
				OnOpen.AddListener( () =>{
					UIStackOperations.singleton.Add( CloseFromStack );
				} );
				OnClose.AddListener( () =>{
					UIStackOperations.singleton.Remove( CloseFromStack );
				} );
			}

			isAwaked = true;
			OnAwake();

		}
		protected virtual void OnDestroy(){
			Windows.Remove( this );
		}
		protected virtual void OnAwake(){}

		void Start(){

			//OpenClose( true, false );
			AnimationSetClose.ReAwakeStart();
			AnimationSetOpen.ReAwakeStart();
			//OpenClose( false, false );
			

			if( typeAwake == AwakeType.OpenFastOnAwake ){
				Open( false );
			}

			OnStart();

		}
		protected virtual void OnStart(){}



		void IListenerLoaderSceneCompleted.OnLoadSceneCompleted(){
			
			if( typeAwake == AwakeType.OpenOnLoadedScene ){
				Open();
			}

		}






		private TimerExecutor.Item timerOpenClose = null;
		private void StopTimerOpenClose(){
			if( timerOpenClose != null ){
				timerOpenClose.ForceComplete( false );
			}
		}

		/// <summary>
		/// Open window.
		/// </summary>
		/// <param name="isAnimation">True - open with animation. False - fast open (without animation).</param>
		/// <param name="timeDelay">Wait before open in seconds.</param>
		public void Open( bool isAnimation = true, bool isStrong = false, float timeDelay = 0.05f ){

			StopTimerOpenClose();
			timerOpenClose = TimerExecutor.Add( timeDelay, timeDelay + 1f, ( float progress ) =>{}, () =>{
				OpenClose( true, isAnimation, isStrong );
			} );

		}

		/// <summary>
		/// Close window with animation.
		/// </summary>
		public void Close( bool isStrong = false ){
			Close( true, isStrong );
		}
		/// <summary>
		/// Close window.
		/// </summary>
		/// <param name="isAnimation">True - with animation, False - fast close (without animation).</param>
		public void Close( bool isAnimation, bool isStrong = false ){
			StopTimerOpenClose();
			OpenClose( false, isAnimation, isStrong );
		}
		public bool CloseFromStack(){
			Close();
			if( OnCloseOnEscape != null ){
				OnCloseOnEscape.Invoke();
			}
			return IsOpen == false;
		}


		/// <summary>
		/// If window opened - close and if closed - open.
		/// With animation.
		/// </summary>
		public void OpenClose( bool isStrong = false ){
			OpenClose( true, isStrong );
		}
		/// <summary>
		/// If window opened - close and if closed - open.
		/// </summary>
		/// <param name="isAnimation">True - with animation, False - fast (without animation).</param>
		public void OpenClose( bool isAnimation, bool isStrong = false ){

			OpenClose( !isOpen, isAnimation, isStrong );

		}
		/// <param name="isOpen">True - open, False - close.</param>
		/// <param name="isAnimation">True - with animation, False - fast (without animation).</param>
		private void OpenClose( bool isOpen, bool isAnimation, bool isStrong = false ){

			if( isStrong == false && this.isOpen == isOpen ) return;
			if( isOpenCloseAtAnimation == false
				&& (AnimationSetOpen.IsAnimationNow == true
				|| AnimationSetClose.IsAnimationNow == true)
			){
				return;
			}

			this.isOpen = isOpen;


#if UNITY_EDITOR
			if( UnityEditor.EditorApplication.isPlaying == false ){
				isAnimation = false;
			}
#endif
		
			// not need animation
			bool withoutAnimation = isAnimation == false;
			if( withoutAnimation == true ){

				content.SetActive( isOpen );
				canvasGroupContent.Interactable( isOpen );
				// open
				if( this.isOpen == true ){
					PlaySound( soundOpen );

				// close
				}else{
					PlaySound( soundClose );
				}


			// need animation
			}else{

				// open
				if( this.isOpen == true ){
					AnimationOpenStart();
					PlaySound( soundOpen );

				// close
				}else{
					AnimationCloseStart();
					PlaySound( soundClose );
				}

				isFirstAnimation = false;

			}


#if UNITY_EDITOR
			if( UnityEditor.EditorApplication.isPlaying ){
#endif
				// events
				// open
				if( isOpen == true ){
					if( isToTopAtOpen == true ){
						transform.SetAsLastSibling();
					}
				
					if( OnOpen != null ){
						OnOpen.Invoke();
					}
					if( withoutAnimation == true ){
						ObserverAnimationCompleted();
					}
					isFirstOnShow = false;
			
				// close
				}else if( OnClose != null ){
					if( isFirstOnShow == false ){
						OnClose.Invoke();
						if( withoutAnimation == true ){
							ObserverAnimationCompleted();
						}
					}
				}
#if UNITY_EDITOR
			}
#endif

		}


		private void PlaySound( AudioClip clip ){
			//AudioManager.PlayEffect( clip );
		}


	




		/// <summary>
		/// Start animation open.
		/// </summary>
		private void AnimationOpenStart( bool isAnimation = false ){

			isAnimationOpenCompleted = false;

			// visible
			canvasGroupContent.Interactable( true );
			content.SetActive( true );

			AnimationSetOpen.Run( ref animationSetLast, OnAnimationCompleted, isAnimation );
			animationSetLast = AnimationSetOpen;

		}
		/// <summary>
		/// Start animation close.
		/// </summary>
		private void AnimationCloseStart( bool isAnimation = false ){
		
			AnimationSetClose.Run( ref animationSetLast, OnAnimationCompleted, isAnimation );
			animationSetLast = AnimationSetClose;

		}




		/// <summary>
		/// Observer completed animation.
		/// </summary>
		private void ObserverAnimationCompleted(){
			if( IsOpen ){
				if( OnOpenAnimationCompleted != null ){
					OnOpenAnimationCompleted.Invoke();
				}
				if( OnOpenAnimationCompletedType != null ){
					OnOpenAnimationCompletedType.Invoke( GetType() );
				}
				isAnimationOpenCompleted = true;

			}else if( OnCloseAnimationCompleted != null ){
				OnCloseAnimationCompleted.Invoke();
			}
		}
		private void OnAnimationCompleted(){
			canvasGroupContent.Interactable( isOpen );
			content.SetActive( isOpen );
			ObserverAnimationCompleted();
		}




#if UNITY_EDITOR
		public virtual void OnInspectorGUI() { }
#endif


	}



	public class UIWindow<T> : UIWindow where T : Kondrat.MVC.Element {

		public T Owner {
			get {
				return owner = Find( owner );
			}
		}
		private T owner = null;

	}

}