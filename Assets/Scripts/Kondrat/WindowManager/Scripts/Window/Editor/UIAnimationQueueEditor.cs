using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;



namespace UIWindowManager{

	[CustomEditor(typeof(UIAnimationQueue))]
	public class UIAnimationQueueEditor : UIAnimationEditor{

		private UIAnimationQueue targetAnimQueue;
		private RectTransform rectDropDown;

		protected override void OnEnable(){
			base.OnEnable();
			targetAnimQueue = target as UIAnimationQueue;
		}

		public override void OnInspectorGUI(){
			
			EditorGUILayout.ObjectField( "Script:", script, typeof(MonoScript), false );


			MyOperationEditor.DrawSeparator( 5 );


			if( targetAnimQueue.queue == null ){
				targetAnimQueue.queue = new UIAnimationQueue.UIAnimationQueueItem[0];
			}
			List<UIAnimationQueue.UIAnimationQueueItem> queue = new List<UIAnimationQueue.UIAnimationQueueItem>( targetAnimQueue.queue );


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( "Queue (" + queue.Count + " items)", EditorStyles.boldLabel );
			if( GUILayout.Button( "clear", GUILayout.Width( 40 ) ) ){
				queue.Clear();
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( "swap", EditorStyles.boldLabel, GUILayout.Width( 75 ) );
			EditorGUILayout.LabelField( "wait", EditorStyles.boldLabel, GUILayout.Width( 50 ) );
			EditorGUILayout.LabelField( "target", EditorStyles.boldLabel/*, GUILayout.Width( 50 )*/ );
			EditorGUILayout.EndHorizontal();


			// operation with queue
			int iFrom = -1, iTo = -1, iRemove = -1;

			for( int i = 0; i < queue.Count; i++ ){
				if( queue[i].animation == null ){
					queue.RemoveAt( i-- );
					continue;
				}

				queue[i].animation.eventType = UIAnimation.EventType.Programmacly;
				queue[i].animation.controlled = targetAnimQueue;

				GUILayout.BeginHorizontal();
			
				// move to up
				EditorGUI.BeginDisabledGroup( i == 0 );
				if( MyOperationEditor.DrawButtonMini( "↑", 35 ) ){
					iFrom = i;
					iTo = i - 1;
				}
				EditorGUI.EndDisabledGroup();
			
				// move to down
				EditorGUI.BeginDisabledGroup( i + 1 == queue.Count );
				if( MyOperationEditor.DrawButtonMini( "↓", 35 ) ){
					iFrom = i;
					iTo = i + 1;
				}
				EditorGUI.EndDisabledGroup();
			

				queue[i].intervalSec = EditorGUILayout.FloatField( queue[i].intervalSec, GUILayout.Width( 50 ) );

				// item
				EditorGUILayout.ObjectField( queue[i].animation, typeof(UIAnimation), false );

				// remove from queue
				if( MyOperationEditor.DrawButtonRemove() ){
					iRemove = i--;
				}

				GUILayout.EndHorizontal();
			}

			// apply move
			if( iFrom != -1 ){
				UIAnimationQueue.UIAnimationQueueItem animCur = queue[iFrom];
				queue.RemoveAt( iFrom );
				queue.Insert( iTo, animCur );
			}
			// apply remove
			if( iRemove != -1 ){
				GameObject.DestroyImmediate( queue[iRemove].animation );
				queue.RemoveAt( iRemove );
			}

		

			MyOperationEditor.DrawSeparator( 5 );
		
		
			// window
			DrawWindow();

			// event show queue
			targetAnimQueue.eventType = (UIAnimation.EventType)EditorGUILayout.EnumPopup( "Event", targetAnimQueue.eventType );

			// animation cycle
			DrawCycle( ref targetAnimQueue.isAutoCycle, ref targetAnimQueue.waitCycleShow, ref targetAnimQueue.waitCycleHide, ref targetAnimQueue.cycleType );


			MyOperationEditor.DrawSeparator( 5 );

		
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField( "Drop RectTransform for add to queue:", EditorStyles.centeredGreyMiniLabel );
			EditorGUILayout.EndHorizontal();


		
			GUILayout.BeginHorizontal();
		

			// check add new item to queue
			Event e = Event.current;
			rectDropDown = EditorGUILayout.ObjectField( rectDropDown, typeof(RectTransform), true, GUILayout.Height( 80 ) ) as RectTransform;

			if( GUILayoutUtility.GetLastRect().Contains( e.mousePosition ) && e.type == EventType.MouseDrag ){
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[]{ rectDropDown };
				DragAndDrop.StartDrag( "drag" );
				Event.current.Use();
			}

			GUILayout.EndHorizontal();



			// deopped
			if( rectDropDown != null ){
				// get / add animation
				UIAnimation animNew = rectDropDown.AddComponent<UIAnimation>( true );
			
				// check have in queue
				bool isHave = false;
				for( int i = 0; i < queue.Count; i++ ){
					if( queue[i].animation == animNew ){
						isHave = true;
						break;
					}
				}

				// have allready in queue
				if( isHave == true ){
					Debug.Log( "<b>" + animNew + "</b> allready inside queue!" );
					EditorGUIUtility.PingObject( animNew );

				}else{
					// add to queue
					UIAnimationQueue.UIAnimationQueueItem animQueueItem = new UIAnimationQueue.UIAnimationQueueItem( animNew );
					if( queue.Count == 0 ){
						animQueueItem.intervalSec = 0;
					}
					queue.Add( animQueueItem );
				}
				rectDropDown = null;
			}

			// save
			targetAnimQueue.queue = queue.ToArray();
		
		}


		protected override bool IsDrawAnimationSets(){
			return false;
		}

	}

}