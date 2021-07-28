using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;


#if UNITY_EDITOR
namespace UnityEditor{

	public static class MyOperationEditor {


		public static void DrawScript( MonoBehaviour target, string label = "Script:" ) {
			MonoScript script = MonoScript.FromMonoBehaviour( target );

			bool guiEnabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayout.ObjectField( label == null ? GUIContent.none : new GUIContent( label ), script, typeof( MonoScript ), false );
			GUI.enabled = guiEnabled;
		}



		/// <summary>
		/// Creates a new inspector window instance and locks it to inspect the specified target
		/// </summary>
		public static void InspectTarget( GameObject target ) {

			// Get a reference to the `InspectorWindow` type object
			var inspectorType = typeof( Editor ).Assembly.GetType( "UnityEditor.InspectorWindow" );

			// Create an InspectorWindow instance
			var inspectorInstance = ScriptableObject.CreateInstance( inspectorType ) as EditorWindow;

			// We display it - currently, it will inspect whatever gameObject is currently selected
			// So we need to find a way to let it inspect/aim at our target GO that we passed
			// For that we do a simple trick:
			// 1- Cache the current selected gameObject
			// 2- Set the current selection to our target GO (so now all inspectors are targeting it)
			// 3- Lock our created inspector to that target
			// 4- Fallback to our previous selection
			inspectorInstance.Show();

			// Cache previous selected gameObject
			var prevSelection = Selection.activeGameObject;

			// Set the selection to GO we want to inspect
			Selection.activeGameObject = target;

			// Get a ref to the "locked" property, which will lock the state of the inspector to the current inspected target
			var isLocked = inspectorType.GetProperty( "isLocked", BindingFlags.Instance | BindingFlags.Public );

			// Invoke `isLocked` setter method passing 'true' to lock the inspector
			isLocked.GetSetMethod().Invoke( inspectorInstance, new object[] { true } );

			// Finally revert back to the previous selection so that other inspectors continue to inspect whatever they were inspecting...
			Selection.activeGameObject = prevSelection;
		}




		public static GameObject InstantiatePrefab( GameObject prefab, Transform parent = null ) {
			GameObject go = PrefabUtility.InstantiatePrefab( prefab ) as GameObject;
			go.transform.SetParent( parent );
			go.transform.ResetTransform();
			return go;
		}



		/// <summary>
		/// Move component to up in inspector
		/// </summary>
		/// <param name="component">Target</param>
		public static void MoveComponentUp( Component component ) {
			UnityEditorInternal.ComponentUtility.MoveComponentUp( component );
		}
		/// <summary>
		/// Move component to down in inspector
		/// </summary>
		/// <param name="component">Target</param>
		public static void MoveComponentDown( Component component ) {
			UnityEditorInternal.ComponentUtility.MoveComponentDown( component );
		}




		/// <summary>
		/// Repaint all windows in editor
		/// </summary>
		public static void RepaintAll() {

			EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
			for( int i = 0; i < windows.Length; i++ ) {
				windows[i].Repaint();
			}

		}




		/// <summary>
		/// Draw content in BeginHorizontal <> EndHorizontal
		/// </summary>
		/// <param name="content">Drawing content</param>
		public static void LayoutHorizontal( System.Action content, params GUILayoutOption[] options ) {
			EditorGUILayout.BeginHorizontal( options );
			content();
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Draw content in BeginHorizontal <> EndHorizontal
		/// </summary>
		/// <param name="content">Drawing content</param>
		public static void LayoutVertical( System.Action content, params GUILayoutOption[] options ) {
			EditorGUILayout.BeginVertical( options );
			content();
			EditorGUILayout.EndVertical();
		}




		/// <summary>
		/// Draw text fild in seach style
		/// </summary>
		public static string DrawSeachable( string value ) {

			using( new EditorGUILayout.HorizontalScope() ) {

				value = GUILayout.TextField( value, GUI.skin.FindStyle( "ToolbarSeachTextField" ) );

				if( GUILayout.Button( "", GUI.skin.FindStyle( "ToolbarSeachCancelButton" ) ) ) {
					// Remove focus if cleared
					value = "";
					GUI.FocusControl( null );
				}

			}

			return value;
		}




		/// <summary>
		/// Draw popup with inherited classes
		/// </summary>
		/// <typeparam name="T">Base class</typeparam>
		public static System.Type DrawTypePopup<T>( string label = null, System.Type typeSelected = null ) where T : class {

			// get inherited classes
			List<System.Type> types = ReflectionExtension.GetInheritedListTypes<T>();
			types.Insert( 0, null );

			// prepare for popup
			List<GUIContent> displayedOptions = new List<GUIContent>();
			List<int> optionValues = new List<int>();
			int selectedValue = -1;

			// setting for popup
			for( int i = 0; i < types.Count; i++ ) {

				if( types[i] == null ) {
					displayedOptions.Add( new GUIContent( "None" ) );

				} else {
					displayedOptions.Add( new GUIContent( types[i].Name.Replace( typeof( T ).Name, "" ).ToStringSplitWords() ) );
				}

				optionValues.Add( i );
				if( types[i] == typeSelected ) {
					selectedValue = optionValues.Count - 1;
				}
			}

			// popup
			GUIContent labelContent = label != null ? new GUIContent( label ) : GUIContent.none;
			selectedValue = EditorGUILayout.IntPopup( labelContent, selectedValue, displayedOptions.ToArray(), optionValues.ToArray() );

			return types[selectedValue];
		}




		/// <summary>
		/// Draw sprite in rect
		/// </summary>
		/// <param name="position">Rect</param>
		/// <param name="sprite">Sprite</param>
		public static void DrawTexturePreview( Rect position, Sprite sprite ) {
			if( sprite == null ) {
				return;
			}

			Vector2 fullSize = new Vector2( sprite.texture.width, sprite.texture.height );
			Vector2 size = new Vector2( sprite.textureRect.width, sprite.textureRect.height );

			Rect coords = sprite.textureRect;
			coords.x /= fullSize.x;
			coords.width /= fullSize.x;
			coords.y /= fullSize.y;
			coords.height /= fullSize.y;

			Vector2 ratio;
			ratio.x = position.width / size.x;
			ratio.y = position.height / size.y;
			float minRatio = Mathf.Min( ratio.x, ratio.y );

			Vector2 center = position.center;
			position.width = size.x * minRatio;
			position.height = size.y * minRatio;
			position.center = center;

			GUI.DrawTextureWithTexCoords( position, sprite.texture, coords );
		}


		public static void DrawTextureGUI( Rect position, Sprite sprite, Vector2 size ) {
			Rect spriteRect = new Rect( sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
									sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height );
			Vector2 actualSize = size;

			actualSize.y *= (sprite.rect.height / sprite.rect.width);
			GUI.DrawTextureWithTexCoords( new Rect( position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y ), sprite.texture, spriteRect );
		}


		public static void DrawTextureGraphics( Rect rect, Sprite sprite ) {

			Vector2 scale = new Vector2();
			scale.x = sprite.rect.width / sprite.texture.width;
			scale.y = sprite.rect.height / sprite.texture.height;

			Vector2 offset = new Vector2();
			offset.x = sprite.rect.position.x / sprite.texture.width;
			offset.y = sprite.rect.position.y / sprite.texture.height;

			Rect rectFixRatio = new Rect( rect.x, rect.y, rect.width, rect.height );

			if( sprite.rect.width < sprite.rect.height ) {
				rectFixRatio.width *= sprite.rect.width / sprite.rect.height;
				rectFixRatio.x += (rect.width - rectFixRatio.width) / 2f;

			} else {
				rectFixRatio.height *= sprite.rect.height / sprite.rect.width;
				rectFixRatio.y += (rect.height - rectFixRatio.height) / 2f;
			}

			rectFixRatio.y -= -rectFixRatio.height;
			rectFixRatio.height *= -1f;

			Graphics.DrawTexture( rectFixRatio, sprite.texture, new Rect( offset.x, offset.y, scale.x, scale.y ), 0, 0, 0, 0 );
		}





		private const char SYMBOL_INVISIBLE_FOR_POPUP = '\u00A0';
		private static Dictionary<string, string[]> cacheDisplayedOptions = null;
		private static Dictionary<string, string[]> CacheDisplayedOptions {
			get {
				if( cacheDisplayedOptions == null ) {
					cacheDisplayedOptions = new Dictionary<string, string[]>();
				}
				return cacheDisplayedOptions;
			}
		}


		public static string DrawPopup( string id, string label, string selectedOption, string[] displayedOptions ) {

			if( CacheDisplayedOptions.ContainsKey( id ) ) {
				displayedOptions = CacheDisplayedOptions[id];

			} else {
				displayedOptions = displayedOptions.Select( item => item + SYMBOL_INVISIBLE_FOR_POPUP ).ToArray();
				int indexSeparator = System.Array.FindIndex( displayedOptions, item => item.Contains( "/" ) );
				if( indexSeparator == -1 ) {
					indexSeparator = 0;
				}
				displayedOptions = displayedOptions.InsertItem( indexSeparator, "" );
				System.Array.Sort( displayedOptions, new ComparerDrawPopup() );

				CacheDisplayedOptions[id] = displayedOptions;
			}

			int index = -1;
			if( string.IsNullOrEmpty( selectedOption ) == false ) {
				index = System.Array.IndexOf( displayedOptions, selectedOption + SYMBOL_INVISIBLE_FOR_POPUP );
			}

			if( label != null ) {
				index = EditorGUILayout.Popup( label, index, displayedOptions );

			} else {
				index = EditorGUILayout.Popup( index, displayedOptions );
			}

			selectedOption = index == -1 ? "" : displayedOptions[index];
			selectedOption = selectedOption.Trim( SYMBOL_INVISIBLE_FOR_POPUP );

			return selectedOption;
		}

		public static void DrawPopupClearCache( string id ) {
			CacheDisplayedOptions.Remove( id );
		}

		public static string DrawPopup( string id, string selectedOption, string[] displayedOptions ) {
			return DrawPopup( id, null, selectedOption, displayedOptions );
		}

		private class ComparerDrawPopup : IComparer<string> {

			public int Compare( string a, string b ) {

				int aCountFolders = System.Text.RegularExpressions.Regex.Matches( a, "/" ).Count;
				int bCountFolders = System.Text.RegularExpressions.Regex.Matches( b, "/" ).Count;

				if( aCountFolders != bCountFolders ) {
					return bCountFolders.CompareTo( aCountFolders );
				}

				return a.CompareTo( b );
			}

		}






		/// <summary>
		/// Find objects in project
		/// </summary>
		/// <param name="type">Type of find object</param>
		public static List<Object> FindAssetsByType( System.Type type ) {
			List<Object> assets = new List<Object>();
			string[] guids = AssetDatabase.FindAssets( "t:Prefab" );
			for( int i = 0; i < guids.Length; i++ ) {
				string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
				GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>( assetPath );
				if( asset != null ) {
					Component component = asset.GetComponent( type );
					if( component != null ) {
						assets.Add( component );
					}
				}
			}
			return assets;
		}

		/// <summary>
		/// Find objects in project
		/// </summary>
		/// <typeparam name="T">Type of find object</typeparam>
		public static List<T> FindAssetsByType<T>() where T : Object {
			return FindAssetsByType( typeof( T ) ).Cast<T>().ToList();
		}

		/// <summary>
		/// Find object in project
		/// </summary>
		/// <typeparam name="T">Type of find object</typeparam>
		public static T FindAssetByType<T>() where T : Object {
			return FindAssetsByType( typeof( T ) ).FirstOrDefault() as T;
		}




		/// <summary>
		/// Remove item from array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] ArrayRemoveItem<T>( T[] array, int index ) {
			if( index < 0
				|| index >= array.Length
			) {
				return array;
			}

			T[] arrayNew = new T[array.Length - 1];
			for( int i = 0, iNew = 0; i < array.Length; i++ ) {
				if( i != index ) {
					arrayNew[iNew++] = array[i];
				}
			}

			return arrayNew;
		}
		/// <summary>
		/// Remove item from array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] RemoveItem<T>( this T[] array, int index ) {
			return ArrayRemoveItem( array, index );
		}


		/// <summary>
		/// Remove item from array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] RemoveItem<T>( this T[] array, System.Func<T, bool> selector ) {
			return array.Where( selector ).ToArray();
		}


		/// <summary>
		/// Remove item to array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] ArrayAddItem<T>( T[] array, T item ) {
			T[] arrayNew = new T[array.Length + 1];
			for( int i = 0; i < array.Length; i++ ) {
				arrayNew[i] = array[i];
			}
			arrayNew[arrayNew.Length - 1] = item;

			return arrayNew;
		}
		/// <summary>
		/// Remove item to array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] AddItem<T>( this T[] array, T item ) {
			return ArrayAddItem( array, item );
		}


		/// <summary>
		/// Insert item to array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] ArrayInsertItem<T>( T[] array, int index, T item ) {
			List<T> list = array.ToList();
			list.Insert( index, item );

			return list.ToArray();
		}
		/// <summary>
		/// Insert item to array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] InsertItem<T>( this T[] array, int index, T item ) {
			return ArrayInsertItem( array, index, item );
		}


		/// <summary>
		/// Swap item in array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] ArraySwapItem<T>( T[] array, int index1, int index2 ) {
			T item = array[index1];
			array[index1] = array[index2];
			array[index2] = item;

			return array;
		}
		/// <summary>
		/// Swap item in array
		/// </summary>
		/// <returns>Reuslt array.</returns>
		public static T[] SwapItem<T>( this T[] array, int index1, int index2 ) {
			return ArraySwapItem( array, index1, index2 );
		}




		/// <summary>
		/// Spliter line.
		/// </summary>
		public static void DrawSeparator( int margin = 10, int border = 1 ) {

			DrawSpaceVertical( margin );

			if( border > 0 ) {
				for( int i = 0; i < border; i++ ) {
					GUIStyle separator = new GUIStyle( "box" );
					separator.border.top = border;
					separator.border.left = separator.border.right = separator.border.bottom = 0;
					separator.margin.top = separator.margin.bottom = 0;
					separator.margin.left = separator.margin.right = 0;
					separator.padding.top = separator.padding.bottom = 0;
					separator.padding.left = separator.padding.right = 0;



					GUILayout.Box( GUIContent.none, separator, GUILayout.ExpandWidth( true ), GUILayout.Height( 1f ) );
				}
			}

			DrawSpaceVertical( margin );
		}

		/// <summary>
		/// Free space vertical.
		/// </summary>
		public static void DrawSpaceVertical( float height ) {
			EditorGUILayout.LabelField( "", GUILayout.Height( height ) );
		}



		/// <summary>
		/// Object (Drag && Drop).
		/// </summary>
		/// <param name="typeValue">Type of Value.</param>
		/// <param name="value">Value.</param>
		/// <returns>Value.</returns>
		public static Object DrawDropArea( System.Type typeValue, Object value, string label = null, GUIStyle style = null ) {

			Event e = Event.current;

			EditorGUILayout.BeginHorizontal();

			if( label == null ) {
				value = EditorGUILayout.ObjectField( value, typeValue, true );

			} else {
				value = EditorGUILayout.ObjectField( label, value, typeValue, true );
			}

			if( GUILayoutUtility.GetLastRect().Contains( e.mousePosition ) && e.type == EventType.MouseDrag ) {
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[] { value };
				DragAndDrop.StartDrag( "drag" );
				Event.current.Use();
			}

			EditorGUILayout.EndHorizontal();

			return value;
		}
		/// <summary>
		/// Object (Drag && Drop).
		/// </summary>
		/// <typeparam name="T">Type object.</typeparam>
		/// <param name="value">Value.</param>
		/// <returns>Value.</returns>
		public static T DrawDropArea<T>( T value, string label = null, GUIStyle style = null ) where T : UnityEngine.Object {
			return DrawDropArea( typeof( T ), value, label, style ) as T;
		}

		public static T DrawProObjectField<T>( string label,
												T tObj,
												GUIStyle style,
												bool allowSceneObjects,
												Texture objIcon = null,
												params GUILayoutOption[] options
		) where T : UnityEngine.Object {

			if( objIcon == null ) {
				objIcon = EditorGUIUtility.FindTexture( "PrefabNormal Icon" );
			}
			style.imagePosition = ImagePosition.ImageLeft;

			int pickerID = 455454425;

			if( tObj != null ) {
				EditorGUILayout.LabelField( new GUIContent( label ), new GUIContent( tObj.name, objIcon ), style, options );
			}

			if( GUILayout.Button( "Select" ) ) {
				EditorGUIUtility.ShowObjectPicker<T>( tObj, allowSceneObjects, "", pickerID );
			}

			if( Event.current.commandName == "ObjectSelectorUpdated" ) {
				if( EditorGUIUtility.GetObjectPickerControlID() == pickerID ) {
					tObj = EditorGUIUtility.GetObjectPickerObject() as T;
				}
			}

			return tObj;
		}




		public static AudioClip DrawDropAudioClip( AudioClip sound, string label = null ) {

			using( new GUILayout.HorizontalScope() ) {

				GUILayout.Label( label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
				if( sound != null && MyOperationEditor.DrawButtonMini( "play", 35 ) ) {
					PlayClip( sound );
				}

				return DrawDropArea( sound );

			}

		}

		/// <summary>
		/// Show object with number.
		/// Object not changed.
		/// </summary>
		public static void DrawDropObjectWithIndex<T>( int index, T obj ) where T : Object {

			GUILayout.BeginHorizontal();

			GUILayout.Label( index + ") ", GUILayout.Width( 30 ) );

			EditorGUILayout.ObjectField( obj, typeof( T ), false );

			GUILayout.EndHorizontal();

		}



		public static void DrawFieldWithLabel( string text, System.Action fieldShow, int width = 100 ) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( text, UnityEngine.GUILayout.Width( width ) );
			fieldShow();
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Draw title, where text is centered and bolded.
		/// </summary>
		/// <param name="title">Text in title.</param>
		public static void DrawTitle( string title ) {

			// draw title
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( title, StyleTitle );
			EditorGUILayout.EndHorizontal();

		}

		/// <summary>
		/// Draw button with style. Red mini button with symbol cross.
		/// </summary>
		public static bool DrawButtonRemove() {

			Color oldColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color( 1f, 0.2f, 0.2f, 0.4f );
			if( GUILayout.Button( "✖", EditorStyles.miniButton, GUILayout.Width( 20 ) ) ) {
				return true;
			}
			GUI.backgroundColor = oldColor;

			return false;
		}


		/// <summary>
		/// Draw button of style "mini" with color background.
		/// </summary>
		/// <param name="text">Text in button.</param>
		/// <param name="colorBackground">Color background.</param>
		/// <param name="coefficientLightning">value leff 1 - dark color. value more 1 - light color.</param>
		public static bool DrawButtonMini( string text, Color colorBackground, float coefficientLightning, int width = -1 ) {
			return DrawButtonMini( text, colorBackground.CloneEffect( coefficientLightning ), width );
		}
		/// <summary>
		/// Draw button of style "mini" with color background.
		/// </summary>
		/// <param name="text">Text in button.</param>
		/// <param name="colorBackground">Color background.</param>
		public static bool DrawButtonMini( string text, Color colorBackground, int width = -1 ) {

			Color oldColor = GUI.backgroundColor;
			GUI.backgroundColor = colorBackground;

			if( width == -1 ) {
				if( GUILayout.Button( text, EditorStyles.miniButtonMid ) ) {
					return true;
				}

			} else {
				if( GUILayout.Button( text, EditorStyles.miniButtonMid, GUILayout.Width( width ) ) ) {
					return true;
				}
			}

			GUI.backgroundColor = oldColor;

			return false;
		}
		/// <summary>
		/// Draw button of style "mini" with color background.
		/// </summary>
		/// <param name="text">Text in button.</param>
		public static bool DrawButtonMini( string text, int width = -1 ) {

			return DrawButtonMini( text, Color.white, width );
		}



		/// <summary>
		/// Draw border at rectangle.
		/// </summary>
		/// <param name="rect">Rectangle border..</param>
		/// <param name="color">Color line.</param>
		/// <param name="width">Width line.</param>
		public static void DrawBorder( Rect window, Color color, float width = 5f ) {

			Vector3[] positions = new Vector3[4];
			positions[0] = window.position;
			positions[1] = new Vector3( window.position.x + window.width, window.position.y );
			positions[2] = new Vector3( window.position.x + window.width, window.position.y + window.height );
			positions[3] = new Vector3( window.position.x, window.position.y + window.height );

			for( int i = 0; i < positions.Length - 1; i++ ) {
				Handles.DrawBezier( positions[i], positions[i + 1], positions[i], positions[i + 1], color, null, width );
			}
			Handles.DrawBezier( positions[3], positions[0], positions[3], positions[0], color, null, width );

		}

		/// <summary>
		/// Draw slider.
		/// </summary>
		/// <param name="cur">Current value.</param>
		/// <param name="min">Min values.</param>
		/// <param name="max">Max value.</param>
		public static float DrawSlider( float cur, float min, float max, int numbersAfterDot = 2,
										bool isShowCurValue = true, Color? colorCurrentValue = null, int fontSize = 12 ) {

			Rect rect = EditorGUILayout.BeginVertical();

			float width = 50f;
			if( Event.current.type == EventType.Repaint ) {
				width = rect.width / 3f - 10;
			}


			GUIStyle styleLabel = StyleLabel;
			styleLabel.fontSize = fontSize;
			styleLabel.fontStyle = FontStyle.Bold;

			// labels
			EditorGUILayout.BeginHorizontal();

			// minimum value slider
			styleLabel.alignment = TextAnchor.MiddleLeft;
			EditorGUILayout.LabelField( min.ToString(), styleLabel, GUILayout.Width( width ) );

			// current value slider
			if( isShowCurValue == true ) {
				styleLabel.alignment = TextAnchor.MiddleCenter;

				if( colorCurrentValue.HasValue == false ) {
					colorCurrentValue = Color.black;
				}

				Color colorPrev = styleLabel.normal.textColor;
				styleLabel.normal.textColor = colorCurrentValue.Value;
				cur = EditorGUILayout.FloatField( cur, styleLabel );
				styleLabel.normal.textColor = colorPrev;

				if( cur > max ) {
					cur = max;

				} else if( cur < min ) {
					cur = min;
				}
			}

			// maximum value slider
			styleLabel.alignment = TextAnchor.MiddleRight;
			EditorGUILayout.LabelField( max.ToString(), styleLabel, GUILayout.Width( width ) );

			EditorGUILayout.EndHorizontal();

			// slider
			EditorGUILayout.BeginHorizontal();
			float curPrev = cur;
			cur = GUILayout.HorizontalSlider( cur, min, max, GUILayout.ExpandWidth( true ) );
			if( curPrev != cur ) {
				GUI.FocusControl( "none" );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			// fix beutifully float and returns
			return (float)System.Convert.ToDouble( MyOperation.FloatToString( cur, numbersAfterDot ) );
		}

		/// <summary>
		/// Draw slider.
		/// </summary>
		/// <param name="cur">Current value.</param>
		/// <param name="min">Min values.</param>
		/// <param name="max">Max value.</param>
		public static int DrawSlider( int cur, int min, int max, bool isShowCurrentValue = true, Color? colorCurrentValue = null, int fontSize = 12 ) {
			return (int)DrawSlider( (float)cur, min, max, 0, isShowCurrentValue, colorCurrentValue, fontSize );
		}







		public delegate bool DIsVisible<T>( T item );
		public delegate void DSwitchVisible<T>( T item );
		public delegate T DDraw<T>( T item );
		public delegate T DDrawIndex<T>( T item, int index );

		public delegate void DMove( int from, int to );

		public delegate bool DNeedRemove<T>( T item );
		public delegate void DRemoved<T>( T item );
		public delegate void DRemovedIndex<T>( T item, int index );

		public delegate bool DNeedAdd();
		public delegate void DAdded<T>( T item );
		public delegate void DAddedIndex<T>( T item, int index );



		public class DrawArrayInfo<T> {

			public T[] Items { get; set; }

			public DIsVisible<T> IsVisible { get; set; }
			public DSwitchVisible<T> SwitchVisible { get; set; }


			public DDraw<T> DrawTitle { get; set; }
			public DDrawIndex<T> DrawTitleIndex { get; set; }

			public DDraw<T> DrawContent { get; set; }
			public DDrawIndex<T> DrawContentIndex { get; set; }

			public DDraw<T> DrawSeparator { get; set; }

			public DMove CallbackMove { get; set; }

			public DNeedRemove<T> IsNeedRemove { get; set; }
			public DRemoved<T> CallbackRemoved { get; set; }
			public DRemovedIndex<T> CallbackRemovedIndex { get; set; }

			public DNeedAdd IsNeedAdd { get; set; }
			public DAdded<T> CallbackAdded { get; set; }
			public DAddedIndex<T> CallbackAddedIndex { get; set; }



			public GUIStyle GUIStyleItem { get; set; }



			public DrawArrayInfo() {

			}


			public bool HasDrawTitle() {
				return DrawTitleIndex != null || DrawTitle != null;
			}
			public T InvokeDrawTitle( T item, int index ) {
				if( DrawTitleIndex != null ) {
					DrawTitleIndex( item, index );

				} else if( DrawTitle != null ) {
					return DrawTitle( item );
				}

				return item;
			}

			public bool HasDrawContent() {
				return DrawContentIndex != null || DrawContent != null;
			}
			public T InvokeDrawContent( T item, int index ) {
				if( DrawContentIndex != null ) {
					return DrawContentIndex( item, index );

				} else if( DrawContent != null ) {
					return DrawContent( item );
				}

				return item;
			}

		}

		public static T[] DrawArray<T>( DrawArrayInfo<T> info ) {

			if( info.Items == null ) {
				info.Items = new T[0];
			}

			int iRemove = -1, iSwitchUp = -1, iSwitchDown = -1;
			for( int i = 0; i < info.Items.Length; i++ ) {

				if( i > 0 ) {
					info.DrawSeparator?.Invoke( info.Items[i] );
				}


				EditorGUILayout.BeginVertical( info.GUIStyleItem ?? GUIStyle.none );

				EditorGUILayout.BeginHorizontal();

				// visible
				if( info.IsVisible != null ) {
					if( MyOperationEditor.DrawButtonMini( info.IsVisible( info.Items[i] ) ? "♥" : "♡", Color.white.Clone( 0.4f ), 25 ) ) {
						info.SwitchVisible?.Invoke( info.Items[i] );
					}
				}


				// title
				if( info.HasDrawTitle() ) {
					info.Items[i] = info.InvokeDrawTitle( info.Items[i], i );

				} else {
					EditorGUILayout.LabelField( "" );
				}


				// switch to up
				GUI.enabled = i > 0;
				if( MyOperationEditor.DrawButtonMini( "▲", Color.white.Clone( 0.4f ), 25 ) ) {
					iSwitchUp = i;
				}
				// switch to down
				GUI.enabled = i + 1 < info.Items.Length;
				if( MyOperationEditor.DrawButtonMini( "▼", Color.white.Clone( 0.4f ), 25 ) ) {
					iSwitchDown = i;
				}
				GUI.enabled = true;

				// bettun remove
				if( MyOperationEditor.DrawButtonMini( "✖", Color.red.Clone( 0.4f ), 25 ) ) {
					iRemove = i;
				}

				EditorGUILayout.EndHorizontal();

				if( info.IsNeedRemove?.Invoke( info.Items[i] ) == true ) {
					iRemove = i;
					continue;
				}

				if( info.IsVisible == null
					|| info.IsVisible( info.Items[i] ) == true
				) {
					info.Items[i] = info.InvokeDrawContent( info.Items[i], i );
				}

				EditorGUILayout.EndVertical();

			}

			// is remove item
			if( iRemove != -1 ) {
				info.CallbackRemoved?.Invoke( info.Items[iRemove] );
				info.CallbackRemovedIndex?.Invoke( info.Items[iRemove], iRemove );
				info.Items = MyOperationEditor.ArrayRemoveItem<T>( info.Items, iRemove );

			} else {
				if( iSwitchUp != -1 ) {
					info.CallbackMove?.Invoke( iSwitchUp, iSwitchUp - 1 );
					info.Items = MyOperationEditor.ArraySwapItem( info.Items, iSwitchUp, iSwitchUp - 1 );
				}
				if( iSwitchDown != -1 ) {
					info.CallbackMove?.Invoke( iSwitchDown, iSwitchDown + 1 );
					info.Items = MyOperationEditor.ArraySwapItem( info.Items, iSwitchDown, iSwitchDown + 1 );
				}
			}


			if( info.IsNeedAdd?.Invoke() == true ) {
				EditorGUILayout.Space( 10 );

				if( MyOperationEditor.DrawButtonMini( "+" ) ) {
					T item = MyOperationClass.GetInstance<T>();

					info.CallbackAdded?.Invoke( item );
					info.CallbackAddedIndex?.Invoke( item, info.Items.Length );

					info.Items = info.Items.AddItem( item );
				}
			}

			return info.Items;
		}

		public static void DrawArray( string title, SerializedProperty propertyArray ) {

			EditorGUILayout.Space();

			using( new EditorGUILayout.VerticalScope( StyleBox ) ) {

				DrawTitle( title );
				EditorGUILayout.Space();

				DrawArray( new DrawArrayInfo<object>() {
					Items = new object[propertyArray.arraySize],

					DrawContentIndex = ( data, index ) => {
						EditorGUILayout.PropertyField( propertyArray.GetArrayElementAtIndex( index ) );
						return data;
					},

					DrawSeparator = data => {
						DrawSeparator();
						return data;
					},

					IsNeedAdd = () => true,

					CallbackMove = ( from, to ) => {
						propertyArray.MoveArrayElement( from, to );
						propertyArray.serializedObject.ApplyModifiedProperties();
					},

					CallbackAddedIndex = ( data, index ) => {
						propertyArray.InsertArrayElementAtIndex( index );
						propertyArray.serializedObject.ApplyModifiedProperties();
					},

					CallbackRemovedIndex = ( data, index ) => {
						propertyArray.DeleteArrayElementAtIndex( index );
						propertyArray.serializedObject.ApplyModifiedProperties();
					}

				} );

			}

		}

		public static T[] DrawArray<T>( string title, SerializedProperty propertyArray, T[] array ) where T : class {

			EditorGUILayout.Space();

			using( new EditorGUILayout.VerticalScope( StyleBox ) ) {

				DrawTitle( title );
				EditorGUILayout.Space();

				return DrawArray( new DrawArrayInfo<T>() {
					Items = array,

					DrawContentIndex = ( data, index ) => {
						SerializedProperty propertyItem = propertyArray.GetArrayElementAtIndex( index );

						FieldInfo[] fields = MyOperationClass.GetFields<SerializeField>( data );
						for( int i = 0; i < fields.Length; i++ ) {
							EditorGUILayout.PropertyField( propertyItem.FindPropertyRelative( fields[i].Name ) );
						}

						return data;
					},

					DrawSeparator = data => {
						DrawSeparator();
						return data;
					},

					IsNeedAdd = () => true,

				} );

			}

		}








		private static GUIStyle styleLabel = null;
		public static GUIStyle StyleLabel {
			get {
				if( styleLabel == null ) {
					styleLabel = new GUIStyle( "label" );
					styleLabel.richText = true;
				}
				return styleLabel;
			}
		}

		private static GUIStyle styleLabelCenter = null;
		public static GUIStyle StyleLabelCenter {
			get {
				if( styleLabelCenter == null ) {
					styleLabelCenter = new GUIStyle( "label" );
					styleLabelCenter.richText = true;
					styleLabelCenter.alignment = TextAnchor.UpperCenter;
				}
				return styleLabelCenter;
			}
		}

		private static GUIStyle styleTitle = null;
		public static GUIStyle StyleTitle {
			get {
				if( styleTitle == null ) {
					styleTitle = new GUIStyle( "label" );
					styleTitle.alignment = TextAnchor.MiddleCenter;
					styleTitle.fontStyle = FontStyle.Bold;
					styleTitle.fontSize = 14;
					styleTitle.padding = new RectOffset();
					styleTitle.margin = new RectOffset( 3, 3, 3, 3 );
					styleTitle.richText = true;
					styleTitle.fixedHeight = 20;
					//styleTitle.wordWrap = true;
				}
				return styleTitle;
			}
		}

		private static GUIStyle styleBox = null;
		public static GUIStyle StyleBox {
			get {
				if( styleBox == null ) {
					styleBox = new GUIStyle( EditorStyles.helpBox );
					styleBox.padding = new RectOffset( 10, 10, 10, 10 );
				}
				return styleBox;
			}
		}






		private class TextureColor2D {

			private Color color = Color.black;
			public Color Color { get { return color; } }

			private Texture2D texture2D = null;
			public Texture2D Texture2D { get { return texture2D; } }


			public TextureColor2D( Color color ) {
				this.color = color;

				Texture2D texture2D = new Texture2D( 1, 1, TextureFormat.RGBA32, false );
				texture2D.SetPixel( 0, 0, color );
				texture2D.Apply();

				this.texture2D = texture2D;
			}

			public bool Equals( Color color ) {
				return this.color.Equals( color );
			}

		}

		private static List<TextureColor2D> cacheTexture2D = null;
		private static List<TextureColor2D> CacheTexture2D { get { return cacheTexture2D = cacheTexture2D ?? new List<TextureColor2D>(); } }


		/// <summary>
		/// Get color in texture 2d.
		/// </summary>
		/// <param name="Color">Color.</param>
		/// <returns>Texture2D</returns>
		public static Texture2D GetColorTexture( Color color ) {

			for( int i = 0; i < CacheTexture2D.Count; i++ ) {
				if( CacheTexture2D[i].Equals( color ) == true ) {
					return CacheTexture2D[i].Texture2D;
				}
			}

			TextureColor2D textureColor2D = new TextureColor2D( color );
			CacheTexture2D.Add( textureColor2D );

			return textureColor2D.Texture2D;
		}

		/// <summary>
		/// Get color in texture 2D
		/// </summary>
		/// <param name="red">Red channel [0; 255].</param>
		/// <param name="green">Green channel [0; 255].</param>
		/// <param name="blue">Blue channel [0; 255].</param>
		/// <param name="alpha">Opacity [0; 1].</param>
		/// <returns>Texture2D</returns>
		public static Texture2D GetColorTexture( float red = 255f, float green = 255f, float blue = 255f, float alpha = 1f ) {
			return GetColorTexture( new Color( red / 255f, green / 255f, blue / 255f, alpha ) );
		}














		public static void PlayClip( AudioClip clip, int startSample = 0, bool loop = false ) {
			Assembly unityEditorAssembly = typeof( AudioImporter ).Assembly;

			System.Type audioUtilClass = unityEditorAssembly.GetType( "UnityEditor.AudioUtil" );
			MethodInfo method = audioUtilClass.GetMethod(
				"PlayPreviewClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] { typeof( AudioClip ), typeof( int ), typeof( bool ) },
				null
			);

			method.Invoke(
				null,
				new object[] { clip, startSample, loop }
			);
		}

		public static void StopAllClips() {
			Assembly unityEditorAssembly = typeof( AudioImporter ).Assembly;

			System.Type audioUtilClass = unityEditorAssembly.GetType( "UnityEditor.AudioUtil" );
			MethodInfo method = audioUtilClass.GetMethod(
				"StopAllPreviewClips",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new System.Type[] { },
				null
			);

			method.Invoke(
				null,
				new object[] { }
			);
		}



		/// <summary>
		/// Get fields for editor.
		/// </summary>
		/// <param name="type">Target type.</param>
		public static FieldInfo[] GetFields( System.Type type ) {
			List<FieldInfo> fieldList = new List<FieldInfo>( type.GetFields(
																		  BindingFlags.Instance
																		| BindingFlags.Public
																		| BindingFlags.NonPublic
																		| BindingFlags.FlattenHierarchy
																		| BindingFlags.GetProperty
																		| BindingFlags.SetProperty
																)
															);
			// filter at attribute SerializeField
			for( int i = 0; i < fieldList.Count; i++ ) {
				if( fieldList[i].IsPublic == false ) {
					if( MyOperationClass.IsHaveAttribute<SerializeField>( fieldList[i] ) == false ) {
						fieldList.RemoveAt( i-- );

					} else if( MyOperationClass.IsHaveAttribute<HideInInspector>( fieldList[i] ) == true ) {
						fieldList.RemoveAt( i-- );

					}
				}
			}

			return fieldList.ToArray();
		}


		/// <summary>
		/// Draw (default) fields in editor
		/// </summary>
		/// <param name="serializedObject">Target draw</param>
		public static void DrawFields( SerializedObject serializedObject, System.Func<SerializedProperty, bool> enabled = null, System.Func<SerializedProperty, bool> visibled = null ) {

			using( new EditorGUILayout.VerticalScope() ) {

				// draw other custom fields
				SerializedProperty iterator = serializedObject.GetIterator();
				bool enterChildren = true;
				while( iterator.NextVisible( enterChildren ) ) {
					enterChildren = false;

					if( iterator.name == "m_Script" ) {
						continue;
					}

					if( visibled != null
						&& visibled.Invoke( iterator ) == false
					) {
						continue;
					}

					GUI.enabled = enabled == null || enabled.Invoke( iterator );
					EditorGUILayout.PropertyField( iterator, true, new GUILayoutOption[0] );
					GUI.enabled = true;
				}

				if( GUI.changed ) {
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty( serializedObject.targetObject );
				}
			}

		}

		/// <summary>
		/// Draw (default) need fields in editor
		/// </summary>
		/// <param name="serializedObject">Target draw</param>
		/// <param name="fields">Draw fields</param>
		public static void DrawFields( SerializedObject serializedObject, Dictionary<string, FieldInfo> fields ) {

			// draw other custom fields
			foreach( string fieldName in fields.Keys ) {
				SerializedProperty property = serializedObject.FindProperty( fieldName );
				try {
					EditorGUILayout.PropertyField( property, true );
				} catch( System.Exception exc ) {
					Debug.LogError( "<b>" + fieldName + "</b> - " + exc );
				}
			}

			if( GUI.changed ) {
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty( serializedObject.targetObject );
			}

		}
		/// <summary>
		/// Draw (default) need fields in editor
		/// </summary>
		/// <param name="serializedObject">Target draw</param>
		/// <param name="fields">Draw fields</param>
		public static void DrawFields( Dictionary<string, SerializedProperty> fields, SerializedObject serializedObjectForApply ) {

			// draw other custom fields
			foreach( string fieldName in fields.Keys ) {
				try {
					EditorGUILayout.PropertyField( fields[fieldName], true );
				} catch/*( System.Exception exc )*/{
					//Debug.LogError( "<b>" + fieldName + "</b> - " + exc );
				}
			}

			if( GUI.changed ) {
				serializedObjectForApply.ApplyModifiedProperties();
				EditorUtility.SetDirty( serializedObjectForApply.targetObject );
			}

		}



		public static void GUIChanged( SerializedObject serializedObject ) {
			if( GUI.changed ) {
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty( serializedObject.targetObject );
			}
		}

		public static void GUIChanged( MonoBehaviour component ) {
			if( GUI.changed ) {
				EditorUtility.SetDirty( component );
			}
		}
















		private const string KEY_EDITOR_COLORS = "editor_colors";

		/// <summary>
		/// Save array colors.
		/// </summary>
		/// <param name="colors">Array colors.</param>
		public static void SaveColors( this EditorWindow editorWindow, Color[] colors ){
		
			float[][] colorData = new float[colors.Length][];

			for( int k = 0; k < colorData.Length; k++ ){
				colorData[k] = new float[]{ colors[k].r, colors[k].g, colors[k].b, colors[k].a };
			}

			MyOperationFile.Serialize( colorData, KEY_EDITOR_COLORS );

		}

		/// <summary>
		/// Load array colors.
		/// </summary>
		public static Color[] LoadColors( this EditorWindow editorWindow ){

			float[][] colorData = MyOperationFile.Deserialize<float[][]>( KEY_EDITOR_COLORS );
			if( colorData == null ){
				return new Color[0];
			}

			Color[] colors = new Color[colorData.Length];
			for( int i = 0; i < colorData.Length; i++ ){
				colors[i] = new Color( colorData[i][0], colorData[i][1], colorData[i][2], colorData[i][3] );
			}

			return colors;

		}









		/// <summary>
		/// Save param for editor (in folder Library)
		/// </summary>
		/// <param name="name">Name of config file</param>
		/// <param name="key">Key of param</param>
		/// <param name="value">Value of param</param>
		public static void SaveParam( string name, string key, object value ) {
			JObject data = new JObject();

			string path = Application.dataPath.Replace( "/Assets", "/Library/" ) + name;
			if( File.Exists( path ) == true ) {
				try {
					data = JsonConvert.DeserializeObject<JObject>( File.ReadAllText( path ) );

				} catch {
					data = new JObject();
				}
			}

			data[key] = value.ToString();

			File.WriteAllText( path, data.ToString() );

		}


		/*
		/// <summary>
		/// Load param for editor (from folder Library)
		/// </summary>
		/// <typeparam name="T">Type of param</typeparam>
		/// <param name="name">Name of config file</param>
		/// <param name="key">Key of param</param>
		/// <param name="valueDefault">Default value of param</param>
		public static T LoadParam<T>( string name, string key, T valueDefault = default( T ) ) {

			string path = Application.dataPath.Replace( "/Assets", "/Library/" ) + name;
			if( File.Exists( path ) == true ) {
				try {
					JObject data = JsonConvert.DeserializeObject<JObject>( File.ReadAllText( path ) );
					return data[key].GetValue<T>( valueDefault );

				} catch {}
			}

			return valueDefault;
		}
		*/





		/// <summary>
		/// Save params (class) for editor (in folder Library)
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="name">Name of config file</param>
		/// <param name="value">Object with value</param>
		public static void SaveParams<T>( string name, T value ){

			string path = Application.dataPath.Replace( "/Assets", "/Library/" ) + name;

			File.WriteAllText( path, JsonConvert.SerializeObject( value ) );

		}


		/// <summary>
		/// Load params (class) for editor (from folder Library)
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="name">Name of config file</param>
		/// <param name="valueDefault">Default value</param>
		public static T LoadParams<T>( string name, T valueDefault = default( T ) ) {

			string path = Application.dataPath.Replace( "/Assets", "/Library/" ) + name;
			if( File.Exists( path ) == true ) {
				try {
					T data = JsonConvert.DeserializeObject<T>( File.ReadAllText( path ) );
					return data;

				} catch { }
			}

			return valueDefault;
		}



	}

}
#endif