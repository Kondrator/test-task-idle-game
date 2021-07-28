using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
#endif


namespace UIWindowManager{

	public class AnimationSet : MonoBehaviour{

		private enum TypeVisible{
			Simple,
			Detail
		}


		[SerializeField]
		private List<AnimationItem> animations = null;
		private List<AnimationItem> Animations{
			get{
				if( animations == null ){;
					animations = new List<AnimationItem>();
				}
				return animations;
			}
		}

		[SerializeField]
		private new string name = "";
		[SerializeField]
		private bool isShowItems = false;
		[SerializeField]
		private TypeVisible typeVisible = TypeVisible.Simple;
		[SerializeField]
		private float duration = 1f;
		public float Duration{ get{ return duration; } set{ duration = value; } }

		public bool IsAnimationNow{
			get{
				return timer != null && timer.IsCompleted == false;	
			}
		}



		private CanvasGroup canvas = null;
		public CanvasGroup Canvas{
			get{
				if( canvas == null ){
					canvas = this.AddComponent<CanvasGroup>();
				}
				return canvas;
			}
		}

	
		public void ReAwakeStart(){
			for( int i = 0; i < Animations.Count; i++ ){
				Animations[i].ReAwakeStart();
			}
		}
	

		public static AnimationSet CreateFor( Transform parent, string nameExtra ){
			if( parent.gameObject.activeInHierarchy == false ){
				return null;
			}

			AnimationSet animationSet = new GameObject( typeof(AnimationSet) + " for " + nameExtra ).AddComponent<AnimationSet>();
			animationSet.transform.SetParent( parent );
			animationSet.name = nameExtra;
			MyOperation.ResetTransform( animationSet.transform );
		
			return animationSet;
		}

	
		/// <summary>
		/// Start animations.
		/// If have current animtion - stop and start again.
		/// </summary>
		public void Run( System.Action callbackCompleted = null, bool isFast = false ){
			Stop();

			// Update
			TimerAnimation( callbackCompleted, isFast );
		}
		/// <summary>
		/// Start animations.
		/// If have current animtion - stop and start again.
		/// Before start this AnimationSet - stop need AnimationSet.
		/// </summary>
		public void Run( ref AnimationSet animationStopBefore, System.Action callbackCompleted = null, bool isFast = false ){
			if( animationStopBefore != null ){
				animationStopBefore.Stop();
				animationStopBefore = null;
			}
			Stop();
			
			// Update
			TimerAnimation( callbackCompleted, isFast );
		}
		/// <summary>
		/// Stop animations.
		/// </summary>
		public void Stop(){

			// Update
			if( timer != null ){
				timer.ForceComplete( false );
				timer = null;
			}

		}
		/// <summary>
		/// Stop current animations and set progress animation at end.
		/// </summary>
		public void StopEnd(){

			// Update
			if( timer != null ){
				timer.ForceComplete( true );
				timer = null;
			}

		}




		private TimerExecutor.Item timer = null;
		private void TimerAnimation( System.Action callbackCompleted = null, bool isFast = false ){
			
			if( timer != null ){
				timer.ForceComplete( false );
			}


			System.Action callback = () =>{
				for( int i = 0; i < Animations.Count; i++ ){
					Animations[i].Completed();
				}
				if( callbackCompleted != null ){
					callbackCompleted.Invoke();
				}
				
				timer = null;
			};

			for( int i = 0; i < Animations.Count; i++ ){
				Animations[i].Prepare();
			}

			if( isFast == true ){
				for( int i = 0; i < Animations.Count; i++ ){
					Animations[i].SetProgress( 1f );
				}
				callback();

			}else{

				timer = TimerExecutor.Add( duration, 0.01f, ( float progress ) =>{
					
					for( int i = 0; i < Animations.Count; i++ ){
						Animations[i].SetProgress( progress );
					}

				}, callback );

			}

		}





#if UNITY_EDITOR

		public static void SettingSets( AnimationSet on, AnimationSet off ){
		
			animationSets.Clear();

			if( on != null ){
				on.Reset();
				on.OnCreateAnimation += ( AnimationItem item ) =>{
					item.ResetCurveForON();

					if( item is AnimationFromTo ){
						((AnimationFromTo)item).IsFrom = true;
					}
				};
				on.OnCreateAnimationExtra += ( AnimationItem item ) =>{
					off.AddCopyAnimation( item );
				};
				animationSets.Add( on );
			}
			if( off != null ){
				off.Reset();
				off.OnCreateAnimation += ( AnimationItem item ) =>{
					item.ResetCurveForOFF();

					if( item is AnimationFromTo ){
						((AnimationFromTo)item).IsFrom = false;
					}
				};
				off.OnCreateAnimationExtra += ( AnimationItem item ) =>{
					on.AddCopyAnimation( item );
				};
				animationSets.Add( off );
			}

		}


	
		public delegate void DCreateAnimationItem( AnimationItem item );
		public RectTransform targetDefault = null;
		public DCreateAnimationItem OnCreateAnimation = null,
									OnCreateAnimationExtra = null;

	
		private delegate AnimationItem DAI();
		private static Dictionary<string, System.Type> animationsData = null;
		private static Dictionary<string, System.Type> AnimationsData{
			get{
				if( animationsData == null ){
					animationsData = new Dictionary<string, System.Type>();
					animationsData[AnimationOpacity.NAME] = typeof(AnimationOpacity);
					animationsData[AnimationRotation.NAME] = typeof(AnimationRotation);
					animationsData[AnimationScale.NAME] = typeof(AnimationScale);
					animationsData[AnimationOffset.NAME] = typeof(AnimationOffset);
					animationsData[AnimationMoveOverBorder.NAME] = typeof(AnimationMoveOverBorder);
					animationsData[AnimationClipCustom.NAME] = typeof(AnimationClipCustom);

					animationsNames = new string[animationsData.Keys.Count];
					int index = 0;
					foreach( string key in animationsData.Keys ){
						animationsNames[index++] = key;
					}
				}
				return animationsData;
			}
		}
		private static string[] animationsNames = null;
		private static string[] AnimationsNames{
			get{
				if( animationsNames == null ){
					object instance = AnimationsData;
				}
				return animationsNames;
			}
		}
		private static int indexAnimationSelect = 0;

		public void Reset(){
			OnCreateAnimation = null;
			OnCreateAnimationExtra = null;
			animationsData = null;
			animationsNames = null;
			animationSets.Remove( this );
		}



		private static GUIStyle styleLabel = null;
		private static GUIStyle StyleLabel{
			get{
				if( styleLabel == null ){
					styleLabel = MyOperationEditor.StyleLabel;
				}
				return styleLabel;
			}
		}
	
		private static Color colorRed = new Color( 0, 0, 0, 0 );
		public static Color ColorRed{
			get{
				if( colorRed.a == 0 ){
					colorRed = Color.red.Clone( 0.5f );
				}
				return colorRed;
			}
		}
		private static Color colorWhite = new Color( 0, 0, 0, 0 );
		public static Color ColorWhite{
			get{
				if( colorWhite.a == 0 ){
					colorWhite = Color.white.Clone( 0.3f );
				}
				return colorWhite;
			}
		}
		private static Color colorWhite2 = new Color( 0, 0, 0, 0 );
		public static Color ColorWhite2{
			get{
				if( colorWhite2.a == 0 ){
					colorWhite2 = Color.white.Clone( 0.8f );
				}
				return colorWhite2;
			}
		}


		public static AnimationSet DrawGUI( AnimationSet animationSet, RectTransform targetDefault ){
		
			if( animationSet != null ){
				animationSet.targetDefault = targetDefault;
				animationSet.DrawGUI();

			}else{
				MyOperationEditor.DrawSeparator( 5 );
				EditorGUILayout.HelpBox( "For setting animation - active GameObject in scene.", MessageType.Info );
			}

			return animationSet;
		}
		public virtual void DrawGUI(){
		
			MyOperationEditor.DrawSeparator( 10 );

			EditorGUILayout.BeginVertical( EditorStyles.helpBox );

			MyOperationEditor.DrawTitle( "Animation " + name );

			if( Animations.Count == 0 ){
				isShowItems = true;
			}
		
			if( isShowItems == true ){
				if( Animations.Count > 0 ){
					OnGUIButtonDisplayType();

				}else{
					GUILayout.Space( 10 );
				}

				int indexRemove = -1;
				for( int i = 0; i < Animations.Count; i++ ){

					if( Animations[i] == null ){
						Animations.RemoveAt( i-- );
						continue;
					}

					switch( typeVisible ){
						case TypeVisible.Simple:
							// title
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField( "<b>" + Animations[i].Name + "</b>", StyleLabel, GUILayout.Width( EditorGUIUtility.labelWidth ) );
								Animations[i].DrawGUICurveSimple();
								if( MyOperationEditor.DrawButtonMini( "x", ColorRed, 18 ) ){
									indexRemove = i;
								}
							EditorGUILayout.EndHorizontal();
							// params
							Animations[i].DrawGUISimple();
							Animations[i].CheckpRecalculateRealtime();
							break;

						case TypeVisible.Detail:
							// title
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField( "<b>" + Animations[i].Name + "</b>", StyleLabel );
								if( MyOperationEditor.DrawButtonMini( "x", ColorRed, 18 ) ){
									indexRemove = i;
								}
							EditorGUILayout.EndHorizontal();
							// params
							Animations[i].DrawGUIDetail( duration );
							Animations[i].CheckpRecalculateRealtime();
							break;
					}
			
					MyOperationEditor.DrawSeparator( 5 );
				}

				if( Animations.Count == 0 ){
					MyOperationEditor.DrawSeparator( 5 );
				}
			

				// duration
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( "Duration (in seconds)", GUILayout.Width( EditorGUIUtility.labelWidth ) );
					float durationMin = 0.03f;
					float durationMax = 10;
					duration = Mathf.Clamp( EditorGUILayout.FloatField( duration, GUILayout.Width( 40 ) ), durationMin, durationMax );
					float durationNew = GUILayout.HorizontalSlider( duration, durationMin, durationMax, GUILayout.MinWidth( 50 ) );
					if( durationNew != duration ){
						duration = (float)System.Math.Round( durationNew, 2 );
						GUI.FocusControl( "" );
					}
				EditorGUILayout.EndHorizontal();
				

				MyOperationEditor.DrawSeparator( 5 );

				// add new animation to set
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( "Add Animation " + name, GUILayout.Width( EditorGUIUtility.labelWidth ) );
					indexAnimationSelect = EditorGUILayout.Popup( indexAnimationSelect, AnimationsNames );
					if( MyOperationEditor.DrawButtonMini( "+", 25 ) ){
						AddAnimation( AnimationsData[AnimationsNames[indexAnimationSelect]], OnCreateAnimation );
					}
					if( OnCreateAnimationExtra != null
						&& MyOperationEditor.DrawButtonMini( "++", 30 )
					){
						AddAnimation( AnimationsData[AnimationsNames[indexAnimationSelect]], ( AnimationItem item ) =>{
							if( OnCreateAnimation != null ){
								OnCreateAnimation( item );
							}
							if( OnCreateAnimationExtra != null ){
								OnCreateAnimationExtra( item );
							}
						} );
					}
				EditorGUILayout.EndHorizontal();

				// remove animation from set
				if( indexRemove != -1 ){
					DestroyImmediate( Animations[indexRemove] );
					Animations.RemoveAt( indexRemove );
				}
			
			}else{
				GUILayout.Space( 10 );
			}


		
			EditorGUILayout.BeginHorizontal();
				if( Animations.Count > 0 ){
					OnGUIButtonShowItems();
					if( MyOperationEditor.DrawButtonMini( "copy", ColorWhite, 40 ) ){
						clipboard = Animations.ToArray();
					}

				}else{
					EditorGUILayout.Space();
				}

				GUI.enabled = clipboard != null;
				if( MyOperationEditor.DrawButtonMini( "paste", ColorWhite, 40 ) ){
					for( int i = 0; i < clipboard.Length; i++ ){
						if( clipboard[i] == null ){
							continue;
						}

						AddCopyAnimation( clipboard[i] );
					}
				}
				GUI.enabled = true;
			EditorGUILayout.EndHorizontal();

		
			EditorGUILayout.EndVertical();

		}

		private AnimationItem AddAnimation( System.Type typeAnimation, DCreateAnimationItem callback ){
			AnimationItem animationNew = (AnimationItem)this.gameObject.AddComponent( typeAnimation );
			animationNew.SetTargetDefault( targetDefault );
			animationNew.ResetCurveForON();
			Animations.Add( animationNew );
			if( callback != null ){
				callback.Invoke( animationNew );
			}
			InternalEditorUtility.SetIsInspectorExpanded( animationNew, false );
			if( Application.isPlaying == false ){
				EditorSceneManager.MarkAllScenesDirty();
			}
			isShowItems = true;

			return animationNew;
		}
		public AnimationItem AddCopyAnimation( AnimationItem itemSource ){
			AddAnimation( itemSource.GetType(), ( AnimationItem item ) =>{
				item.CopyFrom( itemSource );
				if( OnCreateAnimation != null ){
					OnCreateAnimation.Invoke( item );
				}
			} );

			return itemSource;
		}

		private static AnimationItem[] clipboard = null;
	
	
		private void OnGUIButtonDisplayType(){
			EditorGUILayout.BeginHorizontal();
				if( MyOperationEditor.DrawButtonMini( TypeVisible.Simple.ToString(), typeVisible == TypeVisible.Simple ? ColorWhite2 : ColorWhite ) ){
					typeVisible = TypeVisible.Simple;
				}
				if( MyOperationEditor.DrawButtonMini( TypeVisible.Detail.ToString(), typeVisible == TypeVisible.Detail ? ColorWhite2 : ColorWhite ) ){
					typeVisible = TypeVisible.Detail;
				}
			EditorGUILayout.EndHorizontal();
		}
		private void OnGUIButtonShowItems(){
			if( MyOperationEditor.DrawButtonMini( isShowItems ? "hide" : "show", ColorWhite ) ){
				isShowItems = !isShowItems;
			}
		}




		private static List<AnimationSet> animationSets = new List<AnimationSet>();
		public static void EnableAnimationClipSetting( Transform target ){

			Animation animation = target.AddComponent<Animation>();
			for( int i = 0; i < animationSets.Count; i++ ){

				for( int a = 0; a < animationSets[i].Animations.Count; a++ ){
					AnimationClipCustom animClip = animationSets[i].Animations[a] as AnimationClipCustom;
					if( animClip != null
						&& animClip.Clip != null
					){
						animation.AddClip( animClip.Clip, animClip.Clip.name );
					}
				}

			}
			//Selection.activeTransform = target;
			EditorGUIUtility.PingObject( target );
			
			EditorApplication.ExecuteMenuItem( "Window/Animation" );
			EditorApplication.RepaintAnimationWindow();
			Selection.activeObject = null;
			//Selection.activeObject = target;

		}

	
		[CustomEditor(typeof(AnimationSet), true)]
		public class AnimationSetEditor : Editor{

			private AnimationSet animationSet = null;

			void OnEnable(){
				animationSet = base.target as AnimationSet;
				animationSets.Add( animationSet );
			}

			void OnDisable(){
				animationSet.Reset();
			}

			public override void OnInspectorGUI(){
				animationSet.DrawGUI();
			}

		}
#endif

	}

}