using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UIWindowManager{

	public abstract class AnimationItem : MonoBehaviour{

		public abstract string Name{ get; }


		[SerializeField]
		protected AnimationCurve curve = null;
		[SerializeField]
		protected RectTransform target = null,
								targetDefault = null,
								targetCustom = null;
		[SerializeField]
		protected CanvasGroup canvasGroup =  null;
		protected CanvasGroup CanvasGroup{
			get{
				if( canvasGroup == null
					&& target != null
				){
					canvasGroup = target.AddComponent<CanvasGroup>();
				}
				return canvasGroup;
			}
			set{
				canvasGroup = value;
			}
		}

		[SerializeField]
		private bool isTargetCustom = false;
		[SerializeField]
		private float progressStart = 0, progressEnd = 1;

		private bool isRangeEnd = false;


#region Fix Screen Point
		private Vector2 scaleBegin,
						screenBegin;
		protected Vector2 ScalePositionFix{ get{ return new Vector2( screenBegin.x / Screen.width, screenBegin.y / Screen.height ); } }
#endregion


		protected void Awake(){
			canvasGroup = CanvasGroup;
			if( curve == null ){
				ResetCurveForON();
			}
			if( target != null ){
				scaleBegin = target.lossyScale;
				screenBegin = new Vector2( Screen.width, Screen.height );
				OnAwake();
			}
		}
		protected void Start(){
			if( target != null ){
				OnStart();
			}
		}
		public void ReAwakeStart(){
			if( target != null ){
				OnAwake();
				OnStart();
			}
		}
		public void SetTargetDefault( RectTransform target ){
			this.targetDefault = target;
			this.targetCustom = target;
			SetTarget( target );
		}
		public void SetTarget( RectTransform target ){
			this.target = target;
		}

		protected virtual void RecalculateRealtime(){}


	
		public void Prepare(){
			OnPrepare();
			SetProgress( 0 );
			isRangeEnd = false;
		}
		public void SetProgress( float progress ){
			if( target != null ){
				if( progress >= progressStart
					&& (progress <= progressEnd || isRangeEnd == false)
				){
					if( progress > progressEnd ){
						isRangeEnd = true;
					}

					float progressNew = (progress - progressStart) / (progressEnd - progressStart);
					if( float.IsNaN( progressNew ) == false ){
						progress = progressNew;
					}
					OnUpdate( progress );
				}
			}
		}
		public void Completed(){
			SetProgress( 1 );
			OnCompleted();
		}
	
	
		protected virtual void OnAwake(){}
		protected virtual void OnStart(){}
		protected virtual void OnPrepare(){}
		protected abstract void OnUpdate( float progress );
		protected virtual void OnCompleted(){}



	
		public void ResetCurveForON(){
			curve = AnimationCurveExtensions.GetCurve( 0, 1f );
		}
		public void ResetCurveForOFF(){
			curve = AnimationCurveExtensions.GetCurve( 1f, 0 );
		}

	
#if UNITY_EDITOR
		public virtual void DrawGUIDetail( float duration ){

			EditorGUILayout.BeginHorizontal();
				// toggle
				bool isTargetCustom = EditorGUILayout.Toggle( this.isTargetCustom, GUILayout.Width( 15 ) );
				if( isTargetCustom != this.isTargetCustom ){
					this.isTargetCustom = isTargetCustom;

					if( isTargetCustom == false ){
						target = targetDefault;

					}else{
						target = targetCustom;
					}
				}

				// target drop
				GUI.enabled = isTargetCustom;
				EditorGUILayout.LabelField( "Target", GUILayout.Width( EditorGUIUtility.labelWidth - 20 ) );
				target = EditorGUILayout.ObjectField( target, typeof(RectTransform), true ) as RectTransform;
				GUI.enabled = true;

				// save custom
				if( isTargetCustom == true ){
					targetCustom = target;
				}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Animate in Segment", GUILayout.Width( EditorGUIUtility.labelWidth ) );
		
				float durationStart = (float)System.Math.Round( duration * progressStart, 2 );
				float durationEnd = (float)System.Math.Round( duration * progressEnd, 2 );
			
				if( durationStart > durationEnd ){
					durationStart = durationEnd;
				}

				durationStart = EditorGUILayout.FloatField( durationStart, GUILayout.Width( 35 ) );
				EditorGUILayout.MinMaxSlider( ref durationStart, ref durationEnd, 0, duration );
				durationEnd = EditorGUILayout.FloatField( durationEnd, GUILayout.Width( 35 ) );
		
				progressStart = durationStart / duration;
				progressEnd = durationEnd / duration;
			
			EditorGUILayout.EndHorizontal();
		
			if( IsDrawGUICurve() == true ){
				DrawGUICurve();
			}
		}
		public virtual void DrawGUISimple(){}
		public void CheckpRecalculateRealtime(){
			if( GUI.changed == true
				&& Application.isPlaying == true
			){
				RecalculateRealtime();
			}
		}

		protected virtual bool IsDrawGUICurve(){ return true; }

													public void DrawGUICurve(){
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( "Curve", GUILayout.Width( EditorGUIUtility.labelWidth ) );
			curve = EditorGUILayout.CurveField( curve );
			if( MyOperationEditor.DrawButtonMini( "on", AnimationSet.ColorWhite, 25 ) ){
				ResetCurveForON();
			}
			if( MyOperationEditor.DrawButtonMini( "off", AnimationSet.ColorWhite, 25 ) ){
				ResetCurveForOFF();
			}
		EditorGUILayout.EndHorizontal();
	}
												public void DrawGUICurveSimple(){
		EditorGUILayout.BeginHorizontal();
			curve = EditorGUILayout.CurveField( curve );
			if( MyOperationEditor.DrawButtonMini( "on", AnimationSet.ColorWhite, 25 ) ){
				ResetCurveForON();
			}
			if( MyOperationEditor.DrawButtonMini( "off", AnimationSet.ColorWhite, 25 ) ){
				ResetCurveForOFF();
			}
		EditorGUILayout.EndHorizontal();
	}

		public virtual void CopyFrom( AnimationItem item ){
			this.curve = item.curve;
			this.target = item.target;
			//this.targetDefault = item.targetDefault;
			this.targetCustom = item.targetCustom;
			this.CanvasGroup = item.CanvasGroup;
			this.isTargetCustom = item.isTargetCustom;
		}

		[CustomEditor(typeof(AnimationItem), true)]
		public class AnimationItemEditor : Editor{

			private AnimationItem animationItem = null;

			void OnEnable(){
				animationItem = base.target as AnimationItem;
			}

			public override void OnInspectorGUI(){
				AnimationSet set = animationItem.GetComponent<AnimationSet>();
				if( set != null ){
					animationItem.DrawGUIDetail( set.Duration );

				}else{
					animationItem.DrawGUISimple();
				}
			}
		}
#endif

	}

}