using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	public class UIData : MonoBehaviour {

		private static UIData singleton_;
		public static UIData singleton{
			get{
				if( singleton_ == null ){
					UIData data = Resources.Load<UIData>( "UIData" );
					if( data != null ){
						singleton_ = MyOperation.Instantiate<UIData>( data.gameObject );
					}
				}
				return singleton_;
			}
		}
		void Awake(){
			singleton_ = this;
		}

	


		[SerializeField]
		private UIWindowMenu menuPrefab = null;
		public static UIWindowMenu MenuPrefab{
			get{
				if( singleton != null ){
					return singleton.menuPrefab;
				}
				return null;
			}
		}


		[SerializeField]
		private UILoaderBetweenScenes loaderBetweenScenes = null;
		public static UILoaderBetweenScenes LoaderBetweenScenes{
			get{
				if( singleton != null ){
					return singleton.loaderBetweenScenes;
				}
				return null;
			}
		}


		[SerializeField]
		private UIMessageBox messageBoxPregab = null;
		public static UIMessageBox MessageBoxPregab{
			get{
				if( singleton != null ){
					return singleton.messageBoxPregab;
				}
				return null;
			}
		}


		[SerializeField]
		private UINotification notificationPrefab = null;
		public static UINotification NotificationPrefab{
			get{
				if( singleton != null ){
					return singleton.notificationPrefab;
				}
				return null;
			}
		}

	}

}