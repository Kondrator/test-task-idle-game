using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace UIWindowManager{

	public class UIButtonAnimation : MonoBehaviour {
		
		[SerializeField]
		private new UIAnimation animation = null;


		public UnityEvent OnClick = new UnityEvent();


		void Awake(){
			GetComponentsInChildren<Button>().AddListenerOnClick( () => {
				OnClick.Invoke();
			} );
		}

		

		public void Show(){
			if( animation != null ){
				animation.Show();
			}
		}

		public void Hide(){
			if( animation != null ){
				animation.Hide();
			}
		}

		public void Visible( bool isVisible ){
			if( animation != null ){
				animation.Visible( isVisible );
			}
		}

		public void VisibleCheck( bool isVisible ){
			if( animation != null ){
				animation.VisibleCheck( isVisible );
			}
		}


	}

}