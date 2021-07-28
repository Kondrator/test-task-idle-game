using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace UIWindowManager{

	[RequireComponent(typeof(Canvas))]
	public class UILoaderBetweenScenes : UIWindow {
	
		private static UILoaderBetweenScenes singleton_;
		public static UILoaderBetweenScenes singleton{
			get{
				if( singleton_ == null ){
					MyOperation.Instantiate<UILoaderBetweenScenes>( UIData.LoaderBetweenScenes.gameObject );
				}
			
				return singleton_;
			}
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnBeforeSceneLoadRuntimeMethod(){
			if( singleton_ == null ){
				object instanсe = singleton;
			}
		}




		[SerializeField]
		private Slider sliderProgress = null;
		[SerializeField]
		private Image imageFillProgress = null;
		[SerializeField]
		private Text textPercentProgress = null;

		private object[] _params = null;
		public object[] Params{
			get{
				if( _params == null ){
					_params = new object[0];
				}
				return _params;
			}
		}

		private object[] _paramsForNextLoad = null;
		public object[] ParamsForNextLoad{
			set{
				_paramsForNextLoad = value;
			}
		}



		private IEnumerator coroutineLoadScene;
		// indexes in build
		protected int scenePrev, sceneCur, sceneLoad = -1;
		protected string sceneLoadName = null;



#if UNITY_EDITOR
		public static bool isFirstScene = true;
#endif

		private bool isObersversCompleted = false;


		protected override void OnAwake(){
			base.OnAwake();


			if( singleton_ != null 
				&& singleton_ != this
			){
				Destroy( this.gameObject );
				return;
			}
			singleton_= this;
			DontDestroyOnLoad( this.gameObject );


		
			if( sliderProgress != null ){
				sliderProgress.minValue = 0;
				sliderProgress.maxValue = 1f;
			}
			UpdateProgress( 0 );

		

			OnOpen.AddListener( () =>{

				UIStackOperations.singleton.AddBlockEskape( this );

				// *2
				if( IsFirstOnShow == true
					&& IsAnimationOpenCompleted == false
				){
					//OnOpenAnimationCompleted.Invoke();
				}
			} );

			OnOpenAnimationCompleted.AddListener( () =>{
				if( coroutineLoadScene != null ){
					StartCoroutine( coroutineLoadScene );
				}
			} );

			OnCloseAnimationCompleted.AddListener( () => {
				if( IsFirstOnShow == false ) {
					ObserverListenersClosed();
				}
			} );

			sceneCur = scenePrev = GetCurrent();

			if( coroutineLoadScene == null ){
				coroutineLoadScene = LoadingSimple();
				// *1
				Open( false );
			}

		}




		/// <summary>
		/// Load scene at index in build.
		/// </summary>
		public static void LoadScene( int scene, params object[] _params ){
		
			LoadSceneObject( scene, _params );

		}
		/// <summary>
		/// Load scene at name.
		/// </summary>
		public static void LoadScene( string scene, params object[] _params ){

			LoadSceneObject( scene, _params );

		}

		private static void LoadSceneObject( object scene, params object[] _params ){

			if( scene is int ){
				singleton.sceneLoad = (int)scene;
				singleton.sceneLoadName = null;

			}else{
				singleton.sceneLoad = -1;
				singleton.sceneLoadName = (string)scene;
			}


			MyOperation.StopCoroutine( singleton, ref singleton.coroutineLoadScene );
			singleton.coroutineLoadScene = singleton.CoroutineLoadScene( scene, _params );
		
			singleton.UpdateProgress( 0 );
			singleton.OnPreLoadScene();

			if( singleton.IsOpen == true ){
				singleton.StartCoroutine( singleton.coroutineLoadScene );

			}else{
				singleton.Open();
			}

		}


		// this is load scene
		private IEnumerator CoroutineLoadScene( object scene, params object[] _params ){

			if( UIWindowMenu.singleton != null ){
				UIWindowMenu.singleton.Close();
			}

			this._params = _params;
			if( _paramsForNextLoad != null ){
				this._params = this._params.Concat( _paramsForNextLoad ).ToArray();
				_paramsForNextLoad = null;
			}


#if UNITY_EDITOR
			isFirstScene = false;
#endif
			UpdateProgress( 0 );
		
			yield return LoadSceneBefore();
			ObserverListenersBegin();

			
			// async load scene
			AsyncOperation loading =  null;
			if( scene is int ){
				loading = SceneManager.LoadSceneAsync( (int)scene );

			}else{
				loading = SceneManager.LoadSceneAsync( (string)scene );
			}

		
			// wait load
			while( loading != null && loading.isDone == false ){
				UpdateProgress( loading.progress / 2f );
				yield return new WaitForSeconds( 0.1f );
			}
			UpdateProgress( 0.5f );
			
			// error loading
			if( loading == null ){
				ErrorLoadScene();
				yield break;
			}
		

			scenePrev = sceneCur;
			if( scene is int ){
				sceneCur = (int)scene;

			}else{
				sceneCur = SceneManager.GetSceneByName( (string)scene ).buildIndex;
			}

			System.GC.Collect();

			yield return new WaitForEndOfFrame();

		

			yield return LoadSceneNow();
			UpdateProgress( 0.6f );

			yield return LoadSceneAfter();
			UpdateProgress( 0.7f );

			// observer
			isObersversCompleted = false;
			yield return ObserverListeners( 0.7f );
			while( isObersversCompleted == false ){
				yield return null;
			}
			
			coroutineLoadScene = null;
			Close();

		}
	
		/// <summary>
		/// Loading only observers.
		/// </summary>
		private IEnumerator LoadingSimple(){

			UIWindowMenu.singleton.Close();

			UpdateProgress( 0 );
		
			yield return LoadSceneBefore();
			UpdateProgress( 0.2f );
		
			yield return LoadSceneNow();
			UpdateProgress( 0.45f );
		
			yield return LoadSceneAfter();
			UpdateProgress( 0.7f );

			// observers
			isObersversCompleted = false;
			yield return ObserverListeners( 0.7f );
			while( isObersversCompleted == false ){
				yield return null;
			}
			UpdateProgress( 1f );

			Close();

			coroutineLoadScene = null;

		}



#region For Other Logic Load (in inherit class)
		protected virtual void OnPreLoadScene(){}

		protected virtual IEnumerator LoadSceneBefore(){
			yield return new WaitForEndOfFrame();
		}
		protected virtual IEnumerator LoadSceneNow(){
			yield return new WaitForEndOfFrame();
		}
		protected virtual IEnumerator LoadSceneAfter(){
			yield return new WaitForEndOfFrame();
		}

		protected virtual void UpdateProgress( float progress ){
		
			if( sliderProgress != null ){
				sliderProgress.value = progress;
			}

			if( imageFillProgress != null ){
				imageFillProgress.fillAmount = progress;
			}

			if( textPercentProgress != null ){
				textPercentProgress.text = (int)(progress * 100f) + " %";
			}

		}

		protected virtual void ErrorLoadScene(){
			UIMessageBox box = UIMessageBox.CreateShow( "Error load scene", UIMessageBoxButtons.YesNo, "Load preview scene", "Close" );
			box.OnClickYes.AddListener( () =>{
				// load preview
				LoadScene( scenePrev );
			} );
			Close();
		}
#endregion





		private IEnumerator ObserverListeners( float progress ){
			// FIND LESTENERS
			
			// loading
			List<IListenerLoaderSceneProgress> listenersLoading = MyOperation.FindObjectsOfType<IListenerLoaderSceneProgress>( false );
			listenersLoading.Reverse();

			float progressStep = (1f - progress) / listenersLoading.Count;
			for( int i = 0; i < listenersLoading.Count; i++ ){
				yield return listenersLoading[i].SceneLoading();
				progress += progressStep;
				UpdateProgress( progress );
			}
		
			UpdateProgress( 1f );
			

			// completed
			List<IListenerLoaderSceneCompleted> listenersCompleted = MyOperation.FindObjectsOfType<IListenerLoaderSceneCompleted>( false );
			listenersCompleted.Reverse();

			for( int i = 0; i < listenersCompleted.Count; i++ ){
				listenersCompleted[i].OnLoadSceneCompleted();
			}

			yield return new WaitForEndOfFrame();

			isObersversCompleted = true;

		}

		private void ObserverListenersBegin(){

			// begin
			List<IListenerLoaderSceneBegin> listenersCompleted = MyOperation.FindObjectsOfType<IListenerLoaderSceneBegin>( false );
			listenersCompleted.Reverse();

			for( int i = 0; i < listenersCompleted.Count; i++ ){
				listenersCompleted[i].OnLoadSceneBegin();
			}

		}

		private void ObserverListenersClosed() {

			// begin
			List<IListenerLoaderSceneClosed> listenersClosed = MyOperation.FindObjectsOfType<IListenerLoaderSceneClosed>( false );
			listenersClosed.Reverse();

			for( int i = 0; i < listenersClosed.Count; i++ ) {
				listenersClosed[i].OnLoadSceneClosed();
			}

		}







		/// <summary>
		/// Get scene name at index.
		/// </summary>
		/// <param name="scene">Index scene in build.</param>
		public static string GetName( int index ){
			
			if( index == -1 ){
				return "Not found scene";
			}

			Scene scene = SceneManager.GetSceneByBuildIndex( index );
			if( scene.IsValid() == false ){
				return "Not found scene";
			}

			return scene.name;
		}
		/// <summary>
		/// Get name of current scene.
		/// </summary>
		public static string GetNameCurrent(){
			return GetName( GetCurrent() );
		}
		/// <summary>
		/// Get name of load scene.
		/// </summary>
		public static string GetNameLoad(){
			if( singleton.sceneLoadName != null ){
				return singleton.sceneLoadName;
			}
			return GetName( singleton.sceneLoad );
		}

		/// <summary>
		/// Get index (in build) of current scene.
		/// </summary>
		public static int GetCurrent(){
			return singleton.sceneCur;
		}
	
		/// <summary>
		/// Get preview scene.
		/// </summary>
		public static int GetPrev(){
			return singleton.scenePrev;
		}



	}



	public interface IListenerLoaderSceneProgress{
		IEnumerator SceneLoading();
	}
	public interface IListenerLoaderSceneBegin{
		void OnLoadSceneBegin();
	}
	public interface IListenerLoaderSceneCompleted {
		void OnLoadSceneCompleted();
	}
	public interface IListenerLoaderSceneClosed {
		void OnLoadSceneClosed();
	}

}