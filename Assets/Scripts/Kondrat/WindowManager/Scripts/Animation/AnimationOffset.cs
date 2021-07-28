using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIWindowManager{

	public class AnimationOffset : AnimationFromTo {

		public const string NAME = "Offset";
		public override string Name{ get{ return NAME; } }

	
		[SerializeField]
		private Vector2 offsetFrom, offsetTo;


		private Vector2 positionBegin, positionFrom, positionTo;

		protected override void OnAwake(){
			positionBegin = target.anchoredPosition;
			RecalculateRealtime();
		}

		protected override void RecalculateRealtime(){
			positionFrom = positionBegin + offsetFrom;
			positionTo = positionBegin + offsetTo;
		}




		protected override void OnUpdate( float progress ){

			progress = FixProgress( progress );

			target.anchoredPosition = curve.Evaluate( progress, positionFrom, positionTo );
		
		}

		protected override void OnCompleted(){

			target.anchoredPosition = positionBegin;

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
				offsetFrom = DrawAxisDirection( "Move From", offsetFrom );
				offsetTo = Vector2.zero;
		
			}else{
				offsetFrom = Vector2.zero;
				offsetTo = DrawAxisDirection( "Move To", offsetTo );
			}
		}

		private Vector2 DrawAxisDirection( string name, Vector2 value ){
		
			string dirX = value.x < 0 ? "▶" : (value.x > 0 ? "◀" :"•");
			string dirY = value.y < 0 ? "▲" : (value.y > 0 ? "▼" : "•");

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( name, GUILayout.Width( EditorGUIUtility.labelWidth ) );
			
				if( dirX == "•" ){
					EditorGUILayout.LabelField( "X " + dirX, GUILayout.Width( 25 ) );

				}else{
					EditorGUILayout.LabelField( "X", GUILayout.Width( 25 ) );
					Rect rect = GUILayoutUtility.GetLastRect();
					rect.width = rect.height = 20;
					rect.x += 12;
					rect.y -= 3;
					GUI.Label( rect, "<size=18>" + dirX + "</size>", MyOperationEditor.StyleLabel );
				}
				value.x = EditorGUILayout.FloatField( value.x, GUILayout.MinWidth( 30 ) );
		
				EditorGUILayout.LabelField( "Y " + dirY, GUILayout.Width( 25 ) );
				value.y = EditorGUILayout.FloatField( value.y, GUILayout.MinWidth( 30 ) );

			EditorGUILayout.EndHorizontal();

			return value;
		}


		protected override bool IsDrawGUICurve(){
			return false;
		}

		public override void CopyFrom( AnimationItem item ){
			base.CopyFrom( item );

			if( item is AnimationOffset ){
				this.offsetFrom = ((AnimationOffset)item).offsetFrom;
				this.offsetTo = ((AnimationOffset)item).offsetTo;
			}
		}
	#endif

	}

}