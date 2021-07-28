using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



namespace UIWindowManager{

	public class UIWindowMenu : UIWindow, IListenerLoaderSceneCompleted {

		private static UIWindowMenu singleton_;
		public static UIWindowMenu singleton{
			get{
				if( singleton_ == null ){
					UIWindowMenu menu = UIData.MenuPrefab;
					if( menu != null ){
						singleton_ = MyOperation.InstantiateInCanvas<UIWindowMenu>( menu.gameObject, true, true );
					}
				}

				return singleton_;
			}
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void OnBeforeSceneLoadRuntimeMethod(){
			if( singleton_ == null ){
				object instanсe = singleton;
			}
		}


		[SerializeField]
		private bool isOnlyExit = false;

		[SerializeField]
		private Button bContinue = null, bExit = null;
	

		protected override void OnAwake(){
			singleton_ = this;
		
			bClose = bContinue;
			base.OnAwake();


			// exit from game
			if( bExit != null ){
				bExit.onClick.AddListener( QuitGameTry );
			}

		}


		[Space, Header( "Localization" )]
		[SerializeField]
		private string	textExitGameQuestion = null;
		[SerializeField]
		private string	textExit = null,
						textCancel = null;

		
		public const string NOTIFY_DOWN_QUIT_GAME = "ui.window.menu.down.quit.game";
		public const string NOTIFY_QUIT_GAME = "ui.window.menu.quit.game";
		public bool isQuitOnlyNotify = false;
		public bool isQuitWithMessageBox = true;
		private UIMessageBox messageBoxQuit = null;
		/// <summary>
		/// Exit from game with question.
		/// </summary>
		public static void QuitGameTry(){
			System.Action quit = () => {
				if( singleton.isQuitOnlyNotify == true ){
					singleton.Notify( NOTIFY_DOWN_QUIT_GAME );

				}else{
					singleton.Notify( NOTIFY_QUIT_GAME );
					MyOperation.Quit();
				}
			};

			if( singleton.isQuitWithMessageBox == true ){
				if( singleton.messageBoxQuit != null ){
					return;
				}
				singleton.messageBoxQuit = UIMessageBox.CreateShow(	singleton.textExitGameQuestion, 
																	UIMessageBoxButtons.YesNo,
																	singleton.textExit, singleton.textCancel
																);
				singleton.messageBoxQuit.OnClose.AddListener( () =>{
					singleton.messageBoxQuit = null;
					UIStackOperations.singleton.RemoveBlockEskape( singleton.messageBoxQuit );
				} );
				UIStackOperations.singleton.AddBlockEskape( singleton.messageBoxQuit );
				singleton.messageBoxQuit.OnClickYes.AddListener( () =>{
					quit();
				} );

			}else{
				quit();
			}

		}


		public static void OnBack(){
			if( singleton.isOnlyExit == true ){
				QuitGameTry();

			}else{
				singleton.OpenClose();
			}
		}



		void IListenerLoaderSceneCompleted.OnLoadSceneCompleted(){

			// close menu
			Close();

		}

	
	}

}