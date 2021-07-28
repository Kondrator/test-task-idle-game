using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;



namespace UIWindowManager{

	/// <summary>
	/// Stack operations.
	/// </summary>
	public class UIStackOperations : MonoBehaviour, IListenerLoaderSceneCompleted {
	
		private static UIStackOperations singleton_;
		public static UIStackOperations singleton{
			get{
				if( singleton_ == null ){
					new GameObject( typeof(UIStackOperations).Name ).AddComponent<UIStackOperations>();
				}
				return singleton_;
			}
		}

		private UIStackOperations(){}


	
		// when push button ESCAPE - used last operation (after remove her)
		private static List<System.Func<bool>> operations;


		/// <summary>
		/// Count operations in stack.
		/// </summary>
		public int Count{
			get{
				return operations.Count;
			}
		}

		// block useing operations at ESCAPE
		private List<UIWindow> winodwsBlockEskape;




		void Awake(){

			if( singleton_ != null ){
				Destroy( this );
				return;
			}


			DontDestroyOnLoad( this );

			singleton_ = this;
			operations = new List<System.Func<bool>>();
			winodwsBlockEskape = new List<UIWindow>();

		}



		void IListenerLoaderSceneCompleted.OnLoadSceneCompleted(){
			operations.Clear();
		}



		void Update(){
		
			if( Input.GetKeyDown( KeyCode.Escape ) ){
				
				// check blockers
				for( int i = 0; i < winodwsBlockEskape.Count; i++ ){
					if( winodwsBlockEskape[i].IsOpen == false ){
						winodwsBlockEskape.RemoveAt( i-- );
					}
				}

				if( winodwsBlockEskape.Count > 0 ){
					return;
				}

				// have operations?
				if( operations.Count > 0 ){

					// number last operation
					int indexLast = operations.Count - 1;

					// execute operation
					System.Func<bool> method = operations[indexLast];
					if( method != null ){
						if( method() == false ){
							return;
						}
					}
				
					// oepration deleted himself in executing from method .Remove
					if( indexLast < operations.Count ){
						// remove operation
						operations.RemoveAt( indexLast );
					}

				// not have operations
				}else{
					// open main menu window
					UIWindowMenu.OnBack();
				}
			}

		}
	

		/// <summary>
		/// Add window to list.
		/// When list have opened windows - Eskape not working.
		/// </summary>
		public void AddBlockEskape( UIWindow window ){
			if( winodwsBlockEskape.Contains( window ) == false ){
				winodwsBlockEskape.Add( window );
			}
		}
		/// <summary>
		/// Remove window from list.
		/// When list have opened windows - Eskape not working.
		/// </summary>
		public void RemoveBlockEskape( UIWindow window ){
			winodwsBlockEskape.Remove( window );
		}



		/// <summary>
		/// Add new operation.
		/// Returns: TRUE - remove from stack. FALSE - dont remove from stack.
		/// </summary>
		/// <param name="method">Operation.</param>
		public void Add( System.Func<bool> method ){
		
			if( method != null ){
			
				operations.Add( method );

			}

		}

		/// <summary>
		/// Remove operation.
		/// </summary>
		/// <param name="method">Operation.</param>
		public void Remove( System.Func<bool> method ){
		
			if( method != null ){

				operations.Remove( method );

			}

		}


		/// <summary>
		/// Close all windows.
		/// </summary>
		/// <param name="ignore">Not close this windows.</param>
		public void CloseAllWindows( params UIWindow[] ignore ){

			// fix
			if( ignore == null ){
				ignore = new UIWindow[0];
			}

			for( int i = 0; i < operations.Count; i++ ){

				UIWindow window = operations[i].Target as UIWindow;
				if( window != null 
					&& ignore.Contains( window ) == false
					&& window.CloseFromStack == operations[i]
				){

					int count = operations.Count;

					operations[i]();

					if( count == operations.Count ){
						operations.RemoveAt( i-- );
					}

				}

			}

		}

	
	}

}