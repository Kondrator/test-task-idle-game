using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;



namespace UIWindowManager{

	/// <summary>
	/// Window with tabs.
	/// </summary>
	public class UIWindowTabControlSimple : UIWindow {

		// container with page contents
		[SerializeField]
		public Transform tabContents = null;
		// container with titles pages (buttons)
		[SerializeField]
		public Transform tabButtons = null;
		// dropdown list - titles pages
		[SerializeField]
		public Dropdown tabDropdown = null;
		bool isTabDropdownSetProgrammacly = false;

		[SerializeField]
		public bool isShowTabAtOpen = false;
		[SerializeField]
		public int indexTabAtOpen = -1;
		[SerializeField]
		public bool toPrevTabAtEscape = false;

	
	
		/// <summary>
		/// Event at show content page.
		/// </summary>
		public EventShowTab OnShowAt = new EventShowTab();
		public class EventShowTab : UnityEvent<int>{}



		private int indexSelectTab = -1;
		public int IndexSelectTab{
			get{ return indexSelectTab; }
			set{ ShowAt( value ); }
		}

		public int CountTabs{
			get{
				if( pages == null ) return 0;
				return pages.Length;
			}
		}
		
		public bool IsSelectTabFirst{ get{ return IndexSelectTab == 0; } }
		public bool IsSelectTabLast{ get{ return IndexSelectTab + 1 >= CountTabs; } }

		public Dropdown TabDropdown{
			get{ return tabDropdown; }
		}


	
		private Button[] buttonsTabs = null;
		private GameObject[] pages = null;
		public GameObject[] Pages{ get{ return pages; } }



		protected override void OnAwake(){
			base.OnAwake();

			// setting for switch from buttons
			if( tabButtons != null ){
				buttonsTabs = tabButtons.GetComponentsInChildren<Button>();
				for( int i = 0; i < buttonsTabs.Length; i++ ){
					int index = i;
					buttonsTabs[i].onClick.RemoveAllListeners();
					buttonsTabs[i].onClick.AddListener( () =>{
						ShowAt( index );
					} );
				}
			}
			if( buttonsTabs == null ){
				buttonsTabs = new Button[0];
			}

			// switch for switch from dropdown list
			if( tabDropdown != null ){
				tabDropdown.onValueChanged.RemoveAllListeners();
				tabDropdown.onValueChanged.AddListener( (int index) =>{
					if( isTabDropdownSetProgrammacly == false ){
						ShowAt( index );
					}
				} );
			}
		
		
			// fix
			if( tabContents != null ){
				pages = new GameObject[tabContents.childCount];
				for( int i = 0; i < tabContents.childCount; i++ ){
					GameObject goTab = tabContents.GetChild( i ).gameObject;
					UIAnimation animTab = goTab.GetComponent<UIAnimation>();
					if( animTab != null ){
						animTab.eventType = UIAnimation.EventType.Programmacly;
						animTab.Stop();

					}else{
						goTab.SetActive( false );
					}
					pages[i] = goTab;
				}
			}else{
				pages = new GameObject[0];
			}
		

			// setting
			OnOpen.AddListener( () =>{
				// hide all fast
				if( pages != null ){
					for( int i = 0; i < pages.Length; i++ ){
						HideTab( pages[i], true );
					}
				}
				// show first need (if have) tab by open
				ShowTabIndexAtOpen( isShowTabAtOpen );
			} );
			OnClose.AddListener( () =>{
				UIStackOperations.singleton.RemoveBlockEskape( this );
			} );
			OnShowAt .AddListener( ( int index ) =>{
				// setting
				if( toPrevTabAtEscape == true ){
					MyOperation.ExecuteAtNextUpdate( this, () =>{
						if( index > 0 ){
							UIStackOperations.singleton.AddBlockEskape( this );

						}else{
							UIStackOperations.singleton.RemoveBlockEskape( this );
						}
					} );
				}
			} );
		
		}
		// show tab at first open
		private void ShowTabIndexAtOpen( bool isShow = true ){
			if( isShow == false ){
				indexTabAtOpen = indexSelectTab;
				if( indexTabAtOpen == -1 ){
					indexTabAtOpen = 0;
				}
			}

			// show select tab
			ShowAt( indexTabAtOpen, true );
		}


		public override void CopyFieldsFrom( UIWindow window ){
			base.CopyFieldsFrom( window );

			if( window is UIWindowTabControlSimple ){
				UIWindowTabControlSimple windowTabControl = (UIWindowTabControlSimple)window;
				this.tabContents = windowTabControl.tabContents;
				this.tabButtons = windowTabControl.tabButtons;
				this.tabDropdown = windowTabControl.tabDropdown;
				this.isTabDropdownSetProgrammacly = windowTabControl.isTabDropdownSetProgrammacly;
				this.isShowTabAtOpen = windowTabControl.isShowTabAtOpen;
				this.indexTabAtOpen = windowTabControl.indexTabAtOpen;
				this.indexSelectTab = windowTabControl.indexSelectTab;
			}
		}


		public override void CheckKey(){

			if( toPrevTabAtEscape == true
				&& IsOpen == true
				&& IsSelectTabFirst == false
				&& Input.GetKeyDown( KeyCode.Escape )
			){
				ShowAt( IndexSelectTab - 1 );
				return;
			}

			base.CheckKey();
		}

	
	
	
		/// <summary>
		/// Show page at index.
		/// </summary>
		/// <param name="index">Index at tab [0+].</param>
		public void ShowAt( int index, bool isHideFast = false ){

			//HideTab( GetSelect() );
			indexSelectTab = -1;

			if( pages != null ){
				for( int i = 0; i < pages.Length; i++ ){

					// current tab
					GameObject tabCurrent = pages[i];
			
					// show / hide tab
					if( i == index ){
						ShowTab( tabCurrent );
						indexSelectTab = index;

						// switch to need dropdown item
						if( tabDropdown != null ){
							isTabDropdownSetProgrammacly = true;
							tabDropdown.value = index;
							isTabDropdownSetProgrammacly = false;
						}

						// observer
						if( OnShowAt != null ){
							OnShowAt.Invoke( i );
						}

					}else{
						//if( isHideFast == true ){
							HideTab( tabCurrent, isHideFast );
						//}
					}

				}
			}

#if UNITY_EDITOR
			if( Application.isPlaying == false ){
				// setting contenst fot inspector
				if( tabContents != null ){
					for( int i = 0; i < tabContents.childCount; i++ ){

						// current tab
						GameObject tabCurrent = tabContents.GetChild( i ).gameObject;
			
						// show / hide tab
						if( i == index ){
							ShowTab( tabCurrent );
							indexSelectTab = index;

						}else{
							HideTab( tabCurrent, isHideFast );
						}

					}
				}
			
				// setting buttons
				buttonsTabs = null;
				if( tabButtons != null ){
					buttonsTabs = tabButtons.GetComponentsInChildren<Button>();
				}
			}
#endif // UNITY_EDITOR

			if( buttonsTabs != null ){
				for( int b = 0; b < buttonsTabs.Length; b++ ){
					buttonsTabs[b].interactable = b != index;
				}
			}

		}


		/// <summary>
		/// Show tab.
		/// </summary>
		private void ShowTab( GameObject tab ){
			if( tab == null ){
				return;
			}

#if UNITY_EDITOR
			if( Application.isPlaying == false ){
				tab.SetActive( true );
				return;
			}
#endif
			

			// try show by UI Animation
			tab.transform.SetAsLastSibling();
			UIAnimation animTab = tab.GetComponent<UIAnimation>();
			if( animTab != null ){
				tab.SetActive( false );
				tab.SetActive( true );
				animTab.Show();

			}else{
				
				// try open by UI Window
				UIWindow windowTab = tab.GetComponent<UIWindow>();
				if( windowTab != null ){
					tab.SetActive( false );
					tab.SetActive( true );
					windowTab.Open();

				}else{
					// show gameobject
					tab.SetActive( true );
				}
			}
		}
		/// <summary>
		/// Hide tab.
		/// </summary>
		private void HideTab( GameObject tab, bool isFast = false ){
			if( tab == null ){
				return;
			}

#if UNITY_EDITOR
			if( Application.isPlaying == false ){
				tab.SetActive( false );
				return;
			}
#endif
			

			// try hide by UI Animation
			UIAnimation animTab = tab.GetComponent<UIAnimation>();
			if( animTab != null ){
				animTab.Visible( false, isFast );
				
			}else{

				// try close by UI Window
				UIWindow windowTab = tab.GetComponent<UIWindow>();
				if( windowTab != null ){
					tab.SetActive( false );
					tab.SetActive( true );
					windowTab.Close( isFast == false, false );

				}else{
					// hide gameobject
					tab.SetActive( false );
				}
			}
		}



		/// <summary>
		/// Return selected page content.
		/// </summary>
		public GameObject GetSelect(){
		
			if( pages == null || indexSelectTab < 0 || indexSelectTab >= pages.Length ) return null;

			return pages[indexSelectTab];

		}

		/// <summary>
		/// Get component from selected page content..
		/// </summary>
		/// <typeparam name="T">Type component.</typeparam>
		public T GetSelect<T>() where T : Component{
		
			GameObject goSelect = GetSelect();

			if( goSelect == null ) return null;

			return (T)goSelect.GetComponent( typeof(T) );

		}
	
		/// <summary>
		/// Get component need page content..
		/// </summary>
		/// <typeparam name="T">Type component.</typeparam>
		/// <param name="index">Index tab [0+].</param>
		public T GetTab<T>( int index ) where T : Component{
		
			if( pages == null || index < 0 || index >= pages.Length ) return null;

			GameObject goTab = pages[index];

			if( goTab == null ) return null;

			return (T)goTab.GetComponent( typeof(T) );

		}
	

	}

}