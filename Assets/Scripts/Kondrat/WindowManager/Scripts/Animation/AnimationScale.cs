using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIWindowManager{

	public class AnimationScale : AnimationItem {

		public const string NAME = "Scale";
		public override string Name{ get{ return NAME; } }


		public enum ScaleType{
			X,
			Y,
			XY
		}
	
		[SerializeField]
		private ScaleType scaleType = ScaleType.XY;



		protected override void OnUpdate( float progress ){

			Vector3 scale = target.localScale;
			float value = curve.Evaluate( progress );

			switch( scaleType ){
				case ScaleType.XY:
					scale = new Vector3( value, value, scale.z );
					break;

				case ScaleType.X:
					scale = new Vector3( value, scale.y, scale.z );
					break;

				case ScaleType.Y:
					scale = new Vector3( scale.x, value, scale.z );
					break;
			}

			target.localScale = scale;

		}

		protected override void OnCompleted(){
			target.localScale = Vector3.one;
		}





#if UNITY_EDITOR
		public override void DrawGUIDetail( float duration ){
			base.DrawGUIDetail( duration );
		
			DrawGUISimple();

			DrawGUICurve();
		}

		public override void DrawGUISimple(){
			base.DrawGUISimple();

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Type", GUILayout.Width( EditorGUIUtility.labelWidth ) );
				scaleType = (ScaleType)EditorGUILayout.EnumPopup( scaleType );
			EditorGUILayout.EndHorizontal();
		}

		protected override bool IsDrawGUICurve(){
			return false;
		}

		public override void CopyFrom( AnimationItem item ){
			base.CopyFrom( item );

			if( item is AnimationScale ){
				this.scaleType = ((AnimationScale)item).scaleType;
			}
		}
#endif

	}

}