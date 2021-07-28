using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIWindowManager{

	public class AnimationRotation : AnimationFromTo {

		public const string NAME = "Rotation";
		public override string Name{ get{ return NAME; } }

	
		[SerializeField]
		private Vector3 angleFrom, angleTo;


		private Vector3 rotationBegin, rotationFrom, rotationTo;

		protected override void OnAwake(){
			rotationBegin = target.transform.localEulerAngles;
			RecalculateRealtime();
		}

		protected override void RecalculateRealtime(){
			rotationFrom = rotationBegin + angleFrom;
			rotationTo = rotationBegin + angleTo;
		}




		protected override void OnUpdate( float progress ){

			progress = FixProgress( progress );

			target.transform.localEulerAngles = curve.Evaluate( progress, rotationFrom, rotationTo );
		
		}

		protected override void OnCompleted(){

			target.transform.localEulerAngles = rotationBegin;

		}





	#if UNITY_EDITOR
	
		public override void DrawGUIDetail( float duration ){
			base.DrawGUIDetail( duration );
		
			DrawGUISimple();

			DrawGUICurve();
		}

		public override void DrawGUISimple(){
			base.DrawGUISimple();

			if( isFrom == true ){
				angleFrom = DrawAxisDirection( "Rotation From", angleFrom );
				angleTo = Vector3.zero;
		
			}else{
				angleFrom = Vector3.zero;
				angleTo = DrawAxisDirection( "Rotation To", angleTo );
			}
		}

		private Vector3 DrawAxisDirection( string name, Vector3 value ){

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( name, GUILayout.Width( EditorGUIUtility.labelWidth ) );
			
				EditorGUILayout.LabelField( "X", GUILayout.Width( 15 ) );
				value.x = EditorGUILayout.FloatField( value.x, GUILayout.MinWidth( 30 ) );
		
				EditorGUILayout.LabelField( "Y", GUILayout.Width( 15 ) );
				value.y = EditorGUILayout.FloatField( value.y, GUILayout.MinWidth( 30 ) );
		
				EditorGUILayout.LabelField( "Z", GUILayout.Width( 15 ) );
				value.z = EditorGUILayout.FloatField( value.z, GUILayout.MinWidth( 30 ) );

			EditorGUILayout.EndHorizontal();

			return value;
		}


		protected override bool IsDrawGUICurve(){
			return false;
		}

		public override void CopyFrom( AnimationItem item ){
			base.CopyFrom( item );

			if( item is AnimationRotation ){
				this.angleFrom = ((AnimationRotation)item).angleFrom;
				this.angleTo = ((AnimationRotation)item).angleTo;
			}
		}
	#endif

	}

}