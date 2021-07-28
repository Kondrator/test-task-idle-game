using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIWindowManager{

	public class AnimationOpacity : AnimationItem {

		public const string NAME = "Opacity";
		public override string Name{ get{ return NAME; } }




		protected override void OnUpdate( float progress ){
			CanvasGroup.alpha = curve.Evaluate( progress );
		}

		/*protected override void OnCompleted(){
			print( "OnCompleted: " + CanvasGroup.alpha );
			CanvasGroup.alpha = 1;
		}*/


	}

}