using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	public abstract class AnimationFromTo : AnimationItem {


		[SerializeField]
		protected bool isFrom = true;
	
		protected float FixProgress( float progress ){
			if( isFrom == false ){
				return 1 - progress;
			}
			return progress;
		}

	#if UNITY_EDITOR
	
		public bool IsFrom{ get{ return isFrom; } set{ isFrom = value; } }
	
		public override void CopyFrom( AnimationItem item ){
			base.CopyFrom( item );

			if( item is AnimationFromTo ){
				this.isFrom = ((AnimationFromTo)item).isFrom;
			}
		}

	#endif

	}

}