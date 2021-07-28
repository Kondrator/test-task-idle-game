using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UIWindowManager{

	public class UIWindowTabControlSimpleButtonSwitch : MonoBehaviour {

		public enum SwitchType{
			ToTarget = 0,
			ToPrev = 1,
			ToNext = 2,
			ToFirst = 3,
			ToLast = 4
		}


		[SerializeField]
		private SwitchType typeSwitch = 0;
		public SwitchType Type{
			get{ return typeSwitch; }
			set{ typeSwitch = value; }
		}

		// index switch to target
		[SerializeField]
		private int indexToTarget = 0;
		public int IndexToSwitch{
			get{ return indexToTarget; }
			set{ indexToTarget = value; }
		}

		private UIWindowTabControlSimple target = null;
		public UIWindowTabControlSimple Target{
			get{ return target; }
		}


		void Awake(){
			Button button = GetComponentInChildren<Button>();
			if( button != null ){
				button.onClick.AddListener( Switch );
			}
		}

		void Start(){
			ReFindTarget();
		}

		public void ReFindTarget(){
			target = GetComponentInParent<UIWindowTabControlSimple>();
		}

		public void Switch(){
			if( target != null ){

				switch( typeSwitch ){

					case SwitchType.ToTarget: 
						target.IndexSelectTab = indexToTarget;
						break;

					case SwitchType.ToPrev: 
						if( target.IndexSelectTab > 0 ){
							target.IndexSelectTab = target.IndexSelectTab - 1;
						}
						break;

					case SwitchType.ToNext: 
						if( target.IndexSelectTab + 1 < target.CountTabs ){
							target.IndexSelectTab = target.IndexSelectTab + 1;
						}
						break;

					case SwitchType.ToFirst: 
						target.IndexSelectTab = 0;
						break;

					case SwitchType.ToLast: 
						target.IndexSelectTab = target.CountTabs - 1;
						break;

				}

			}
		}

	}

}