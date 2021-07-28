using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UIWindowManager{

	public class AnimationClipCustom : AnimationItem {

		public const string NAME = "Animation Clip";
		public override string Name{ get{ return NAME; } }

		[SerializeField]
		private AnimationClip clip = null;




		protected override void OnUpdate( float progress ){
			if( target.gameObject.activeSelf == true ){
				clip.SampleAnimation( target.gameObject, clip.length * progress );
			}
		}
	










#if UNITY_EDITOR

		public AnimationClip Clip{ get{ return clip; } }

		private static Color colorYellow = new Color( 0, 0, 0, 0 );
		private static Color ColorYellow{
			get{
				if( colorYellow.a == 0 ){
					colorYellow = Color.yellow.Clone( 0.5f );
				}
				return colorYellow;
			}
		}

		public override void DrawGUIDetail( float duration ){
			base.DrawGUIDetail( duration );

			DrawGUISimple();

			DrawGUICurve();
		}

		public override void DrawGUISimple(){
			base.DrawGUISimple();

			EditorGUILayout.BeginHorizontal();

				// animation clip setting
				if( target != null ){
					Animation animation = target.GetComponent<Animation>();
					if( MyOperationEditor.DrawButtonMini(	animation != null ? "stop" : "setting",
															animation != null ? AnimationSet.ColorRed : AnimationSet.ColorWhite,
															(int)EditorGUIUtility.labelWidth ) 
					){
						if( animation == null ){
							AnimationSet.EnableAnimationClipSetting( target );

						}else{
							DestroyImmediate( animation );
							EditorApplication.RepaintAnimationWindow();
						}
					}
				}
				clip = MyOperationEditor.DrawDropArea<AnimationClip>( clip );

				// help
				if( MyOperationEditor.DrawButtonMini( "?", ColorYellow, 18 ) ){
					EditorUtility.DisplayDialog(	"How to create correct AnimationClip?", 
													"1) Click at bgutton \"setting\".\n"
													+ "2) Select target GameObject for this animation item.\n"
													+ "2.1) Or this is auto selected.\n"
													+ "3) Add component Animation (after setting remove her).\n"
													+ "3.1) Or auto created and for remove component - click button in AnimationItem.\n"
													+ "4) Open window Animation: from menu Window -> Animation."
													+ "5) Create new AnimationClip.\n"
													+ "6) Drop new AnimationClip to animation item.\n", 
													"OK"
												);
				}

			EditorGUILayout.EndHorizontal();

		}

		protected override bool IsDrawGUICurve(){
			return false;
		}

		public override void CopyFrom( AnimationItem item ){
			base.CopyFrom( item );

			if( item is AnimationClipCustom ){
				this.clip = ((AnimationClipCustom)item).clip;
			}
		}
#endif

	}

}