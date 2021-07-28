using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.SceneManagement;


namespace UIWindowManager{

	[CustomEditor(typeof(UIAnimation), true)]
	public class UIAnimationEditor : Editor{
	

		private UIAnimation targetAnim;
		protected MonoScript script;

		private AnimationSet	animationSetShow = null,
								animationSetHide = null;

		protected virtual void OnEnable() {
			targetAnim = (UIAnimation)target;
			script = MonoScript.FromMonoBehaviour( targetAnim );

			if( IsDrawAnimationSets() ){
				AnimationSet.SettingSets( targetAnim.AnimationSetShow, targetAnim.AnimationSetHide );
				animationSetShow = targetAnim.AnimationSetShow;
				animationSetHide = targetAnim.AnimationSetHide;
			}
		}

		protected virtual void OnDestroy(){
			if( Application.isEditor ){
				if( targetAnim == null ){
					if( animationSetShow != null ){
						GameObject.DestroyImmediate( animationSetShow.gameObject );
					}
					if( animationSetHide != null ){
						GameObject.DestroyImmediate( animationSetHide.gameObject );
					}
				}
			}
		}


		public override void OnInspectorGUI(){

			// save in other variable all fields
			Dictionary<string, SerializedProperty> fieldsGUI = new Dictionary<string, SerializedProperty>();
			SerializedProperty iterator = serializedObject.GetIterator();
			while( iterator.NextVisible( true ) ){
				fieldsGUI[iterator.name] = serializedObject.FindProperty( iterator.name );
			}
			fieldsGUI.Remove( "m_Script" );
			fieldsGUI.Remove( "x" );
			fieldsGUI.Remove( "y" );
			// after removing drawing fields in this method



			
			EditorGUILayout.ObjectField( "Script:", script, typeof(MonoScript), false );


			if( Application.isPlaying == true ){
				MyOperationEditor.DrawSeparator( 5 );
				EditorGUILayout.BeginHorizontal();
					if( MyOperationEditor.DrawButtonMini( "show" ) ){
						targetAnim.Show();
					}
					if( MyOperationEditor.DrawButtonMini( "hide" ) ){
						targetAnim.Hide();
					}
				EditorGUILayout.EndHorizontal();
			}



			// window
			if( targetAnim.eventType == UIAnimation.EventType.WindowShowClose
				|| targetAnim.eventType == UIAnimation.EventType.WindowShowCloseAnimation
			) {
				MyOperationEditor.DrawSeparator( 5 );
				/*bool isHaveWindow = */DrawWindow();
			}
			fieldsGUI.Remove( "window" );
			fieldsGUI.Remove( "controlled" );
			fieldsGUI.Remove( "isStopAtWindowHidden" );


			MyOperationEditor.DrawSeparator( 5 );


			// event
			EditorGUI.BeginDisabledGroup( targetAnim.controlled );
			if( targetAnim.controlled != null ){
				targetAnim.eventType = (UIAnimation.EventType)EditorGUILayout.EnumPopup( "Event", UIAnimation.EventType.Programmacly );

			}else{
				targetAnim.eventType = (UIAnimation.EventType)EditorGUILayout.EnumPopup( "Event", targetAnim.eventType );
			}
			fieldsGUI.Remove( "eventType" );
			EditorGUI.EndDisabledGroup();

			// animation cycle
			DrawCycle( ref targetAnim.isAutoCycle, ref targetAnim.waitCycleShow, ref targetAnim.waitCycleHide, ref targetAnim.cycleType );
			fieldsGUI.Remove( "waitCycleShow" );
			fieldsGUI.Remove( "waitCycleHide" );
			fieldsGUI.Remove( "isAutoCycle" );
			fieldsGUI.Remove( "cycleType" );
			
			// auto hide at awake
			targetAnim.isHideAtAwake = EditorGUILayout.Toggle( "Hide At Awake", targetAnim.isHideAtAwake );
			fieldsGUI.Remove( "isHideAtAwake" );

			// block raycast
			targetAnim.isBlockRaycastWhenAnimation = EditorGUILayout.Toggle( "Block Raycast When Animation", targetAnim.isBlockRaycastWhenAnimation );
			fieldsGUI.Remove( "isBlockRaycastWhenAnimation" );



			if( IsDrawAnimationSets() ){
				DrawAnimationSets();
			}

			fieldsGUI.Remove( "animationSetShow" );
			fieldsGUI.Remove( "animationSetHide" );


			if( fieldsGUI.Count > 0 ){

				MyOperationEditor.DrawSeparator( 5 );


				if( GetType() != typeof(UIAnimationEditor) ){
					OnInspectorGUI( ref fieldsGUI );
					MyOperationEditor.DrawSeparator( 5 );
				}


				// draw other custom fields
				MyOperationEditor.DrawFields( fieldsGUI, serializedObject );

			}
		

		
			if( GUI.changed
				&& Application.isPlaying == false
			){
				EditorUtility.SetDirty( targetAnim );
				serializedObject.ApplyModifiedProperties();
				EditorSceneManager.MarkAllScenesDirty();
			}
		
		}

		/// <summary>
		/// Draw fields.
		/// </summary>
		/// <param name="fieldsGUI">Remove fields after drawing!</param>
		protected virtual void OnInspectorGUI( ref Dictionary<string, SerializedProperty> fieldsGUI ){}
	
		protected virtual bool IsDrawAnimationSets(){
			return true;
		}
		protected virtual void DrawAnimationSets(){
			AnimationSet.DrawGUI( targetAnim.AnimationSetShow, targetAnim.transform as RectTransform );
			AnimationSet.DrawGUI( targetAnim.AnimationSetHide, targetAnim.transform as RectTransform );
		}



		/// <summary>
		/// Draw field window.
		/// </summary>
		/// <returns>Is set window at field.</returns>
		protected bool DrawWindow(){
			// window
			EditorGUILayout.BeginHorizontal();
			targetAnim.window = MyOperationEditor.DrawDropArea<UIWindow>( targetAnim.window, "Window" );
			bool isHaveWindow = targetAnim.window != null;
			if( MyOperationEditor.DrawButtonMini( "find", 50 ) ){
				targetAnim.window = MyOperation.GetComponentInParent<UIWindow>( targetAnim.gameObject );
			}
			EditorGUILayout.EndHorizontal();

			if( isHaveWindow == false ){
				EditorGUILayout.HelpBox( "Widnow auto finding in run game, in parent.", MessageType.Info );
			}

			return isHaveWindow;
		}

		/// <summary>
		/// Draw cycle for animation.
		/// </summary>
		public static void DrawCycle( ref bool cycle, ref float waitCycleShow, ref float waitCycleHide, ref UIAnimation.CycleType cycleType ){
		
			cycle = EditorGUILayout.Toggle( "Auto Cycle", cycle );
			if( cycle == true ){
				cycleType = (UIAnimation.CycleType)EditorGUILayout.EnumPopup( "Cycle Type", cycleType );

				if( cycleType == UIAnimation.CycleType.Switch
					|| cycleType == UIAnimation.CycleType.RepeatShow
 					|| cycleType == UIAnimation.CycleType.ShowLast
				){
					waitCycleShow = EditorGUILayout.Slider( "Wait Cycle Show", waitCycleShow, 0, 10 );
				}

				if( cycleType == UIAnimation.CycleType.Switch
					|| cycleType == UIAnimation.CycleType.RepeatHide
 					|| cycleType == UIAnimation.CycleType.HideLast
				){
					waitCycleHide = EditorGUILayout.Slider( "Wait Cycle Hide", waitCycleHide, 0, 10 );
				}
				
			}

		}


	}

}