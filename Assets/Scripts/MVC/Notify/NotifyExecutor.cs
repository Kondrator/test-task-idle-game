using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace Kondrat.MVC {

    public class NotifyExecutor : Controller {

		[System.Serializable]
		private class Event {

			[SerializeField]
			private NotifyName notify = null;

			[SerializeField]
			private UnityEvent callback = null;



			public Event() {
				notify = new NotifyName();
				callback = new UnityEvent();
			}


			/// <summary>
			/// Get name of notify
			/// </summary>
			public string GetValue() {
				return notify.GetValue();
			}


			/// <summary>
			/// Invoke callback
			/// </summary>
			public void Invoke() {
				callback.Invoke();
			}



#if UNITY_EDITOR
			[SerializeField]
			private bool isVisible = true;
			public bool IsVisible { get { return isVisible; } set { isVisible = value; } }
#endif


		}





		[SerializeField]
		[HideInInspector]
		private Event[] events = null;




		protected override void PreInitiate() {}

		protected override void Initiate() {

			for( int i = 0; i < events.Length; i++ ) {
				Event target = events[i];

				Add(
					target.GetValue(),
					data => target.Invoke()
				);
			}

		}


#if UNITY_EDITOR


		[CustomEditor( typeof(NotifyExecutor) )]
		private class NotifyExecutorEditor : Editor<NotifyExecutor> {


			protected override void OnEnable() {
				base.OnEnable();

				component.events = component.events ?? new Event[0];

			}


			public override void OnInspectorGUI() {
				base.OnInspectorGUI();


				SerializedProperty propertyEvents = serializedObject.FindProperty( "events" );

				MyOperationEditor.DrawSeparator();

				component.events = MyOperationEditor.DrawArray( new MyOperationEditor.DrawArrayInfo<Event>() {
					Items = component.events,

					DrawTitle = data => {

						EditorGUILayout.LabelField( data.GetValue(), EditorStyles.boldLabel );

						return data;
					},

					DrawContentIndex = ( Event data, int index ) => {

						SerializedProperty propertyEvent = propertyEvents.GetArrayElementAtIndex( index );
						SerializedProperty propertyNotify = propertyEvent.FindPropertyRelative( "notify" );
						SerializedProperty propertyCallback = propertyEvent.FindPropertyRelative( "callback" );

						using( new EditorGUILayout.VerticalScope( MyOperationEditor.StyleBox ) ) {

							EditorGUILayout.PropertyField( propertyNotify );
							EditorGUILayout.Space( 10 );

							using( new EditorGUILayout.VerticalScope( MyOperationEditor.StyleBox ) ) {

								MyOperationEditor.DrawTitle( "Callbacks" );
								EditorGUILayout.Space( 10 );

								EditorGUILayout.PropertyField( propertyCallback );

							}
						}

						return data;
					},

					IsVisible = data => data.IsVisible,
					SwitchVisible = data => data.IsVisible = !data.IsVisible,

					DrawSeparator = data => {
						MyOperationEditor.DrawSeparator();
						return data;
					},

					IsNeedAdd = () => true,
				} );



				if( GUI.changed ) {
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty( component );
				}

			}

		}
#endif


	}

}