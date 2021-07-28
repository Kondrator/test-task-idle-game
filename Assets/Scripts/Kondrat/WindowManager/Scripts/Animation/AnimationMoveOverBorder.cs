using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif


namespace UIWindowManager{

	public class AnimationMoveOverBorder : AnimationFromTo {

		public const string NAME = "Move Over Border";
		public override string Name{ get{ return NAME; } }


		public enum TypeOverBorder{
			Left,
			Right,
			Up,
			Down
		}
	
		[SerializeField]
		private TypeOverBorder typeOverBorder = TypeOverBorder.Left;

		private Vector2 positionBegin, positionLeft, positionRight, positionUp, positionDown;

		protected override void OnAwake(){

			//Rect target = base.target.ToScreenSpaceWithCheckLayout();
			Rect target = new Rect( base.target.anchoredPosition, base.target.sizeDelta );
			/*
			float scale = this.transform.lossyScale.x;
			target.position *= scale;
			target.width *= scale;
			target.height *= scale;
			*/

			positionBegin = target.position;

			positionLeft = target.position;
			positionLeft.x = 0 - target.width;

			positionRight = target.position;
			positionRight.x = Screen.width + target.width;

			positionUp = target.position;
			positionUp.y = /*Screen.height + */target.height;

			positionDown = target.position;
			positionDown.y = 0 - target.height;

		}


		private Vector2 GetOverBorder(){
		
			switch( typeOverBorder ){
				case TypeOverBorder.Left: return positionLeft;
				case TypeOverBorder.Right: return positionRight;
				case TypeOverBorder.Up: return positionUp;
				case TypeOverBorder.Down: return positionDown;
			}

			return Vector2.zero;
		}
		private Vector2 GetFrom(){
			if( isFrom == true ){
				return GetOverBorder();
			}
			return positionBegin;
		}
		private Vector2 GetTo(){
			if( isFrom == true ){
				return positionBegin;
			}
			return GetOverBorder();
		}


		protected override void OnUpdate( float progress ){

			progress = FixProgress( progress );

			target.anchoredPosition = curve.Evaluate( progress, GetFrom() / ScalePositionFix, GetTo() / ScalePositionFix );

		}

		protected override void OnCompleted(){

			target.anchoredPosition = GetTo() / ScalePositionFix;

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
				EditorGUILayout.LabelField( isFrom ? "From" : "To", GUILayout.Width( EditorGUIUtility.labelWidth ) );
				typeOverBorder = (TypeOverBorder)EditorGUILayout.EnumPopup( typeOverBorder );
			EditorGUILayout.EndHorizontal();
		}

		protected override bool IsDrawGUICurve(){
			return false;
		}

		public override void CopyFrom( AnimationItem item ){
			base.CopyFrom( item );

			if( item is AnimationMoveOverBorder ){
				this.typeOverBorder = ((AnimationMoveOverBorder)item).typeOverBorder;
			}
		}
#endif

	}

}