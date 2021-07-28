using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UIWindowManager{

	/// <summary>
	/// Template for create ui element from inspector.
	/// </summary>
	public abstract class UICreateFromInspector : MonoBehaviour {

		public enum EventCreate{
			Enable = 0,
			Disable = 1,
			Button = 2,
			ColliderDown = 3,
			Timer = 4,
			Programmacly = 10,
		}


		[SerializeField]
		private EventCreate eventCreate = EventCreate.Enable;

		[SerializeField]
		private Button buttonEvent = null;

		[SerializeField, Range( 0, 10 ), Tooltip( "In seconds" )]
		private float timerEvent = 1;



	
	
		void Awake(){
			if( buttonEvent != null ){
				buttonEvent.onClick.AddListener( () =>{
					if( eventCreate == EventCreate.Button ){
						Create();
					}
				} );
			}
		}

		void OnEnable(){
			switch( eventCreate ) {

				case EventCreate.Enable:
					Create();
					break;

				case EventCreate.Timer:
					TimerExecutor.Add( timerEvent, Create );
					break;

			}
			if( eventCreate == EventCreate.Enable ){
				Create();
			}
		}

		void OnDisable(){
			if( eventCreate == EventCreate.Disable ){
				Create();
			}
		}

		void OnMouseDown(){
			if( eventCreate == EventCreate.ColliderDown ){
				Create();
			}
		}



		public abstract void Create();

	}

}