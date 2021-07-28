using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Security.Policy;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.IO;
using System.Linq;


public class UITextManagerEditorWindow : EditorWindow{
	
	private interface IFontGUI{
		void OnGUI();
		void Apply( Text text );
	}
	private interface ISetIndent{
		void SetIndent( float indent );
	}

	// field in font
	private delegate T DFFGUI<T>( T value );
	private delegate void DFFApply<T>( Text text, T value );
	[System.Serializable]
	private class FontFieldData<T> : IFontGUI, ISetIndent, ISerializable{
		private DFFGUI<T> onGUI = null;
		private DFFApply<T> onApply = null;
		
		private string title = "Font Field";
		private bool isUse = false;
		protected T value = default(T);
		public T Value{ get{ return value; } }

		public float indent = 0;
		public void SetIndent( float indent ){
			this.indent = indent;
		}

		public FontFieldData( T valueDefault ){
			this.value = valueDefault;
		}
		public void Set( string title, DFFGUI<T> onGUI, DFFApply<T> onApply ){
			this.title = title;
			this.onGUI = onGUI;
			this.onApply = onApply;
		}


		public void OnGUI(){
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space( indent );
				isUse = EditorGUILayout.Toggle( isUse, GUILayout.Width( 14 ) );
				GUI.enabled = isUse;
				EditorGUILayout.LabelField( title, GUILayout.Width( 120 ) );
				value = onGUI( value );
				GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
		}

		public void Apply( Text text ){
			if( isUse == true ){
				onApply( text, value );
			}
		}


		
		protected virtual bool IsSerializeCustom{ get{ return false; } }
		
		private const string KEY_IS_USE = "u";
		protected const string KEY_VALUE = "v";

		protected FontFieldData( SerializationInfo info, StreamingContext context ){
			isUse = info.GetBoolean( KEY_IS_USE );

			if( IsSerializeCustom == false ){
				value = (T)info.GetValue( KEY_VALUE, typeof(T) );
			}
		}  
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context ){
			info.AddValue( KEY_IS_USE, isUse );
			
			if( IsSerializeCustom == false ){
				info.AddValue( KEY_VALUE, value );
			}
		} 

	}
	// field color custom
	[System.Serializable]
	private class FontFieldDataColor : FontFieldData<Color>{
		public FontFieldDataColor( Color valueDefault ) :base( valueDefault ){}

		protected override bool IsSerializeCustom{ get{ return true; } }
		protected const string KEY_R = "r";
		protected const string KEY_G = "g";
		protected const string KEY_B = "b";
		protected const string KEY_A = "a";
		
		protected FontFieldDataColor( SerializationInfo info, StreamingContext context ) :base( info, context ){
			value = new Color();
			value.r = (float)info.GetDouble( KEY_R );
			value.g = (float)info.GetDouble( KEY_G );
			value.b = (float)info.GetDouble( KEY_B );
			value.a = (float)info.GetDouble( KEY_A );
		}  
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ){
			base.GetObjectData( info, context );

			info.AddValue( KEY_R, value.r );
			info.AddValue( KEY_G, value.g );
			info.AddValue( KEY_B, value.b );
			info.AddValue( KEY_A, value.a );
		} 
	}
	// field material custom
	[System.Serializable]
	private class FontFieldDataObject<T> : FontFieldData<T> where T : Object{
		private string extension = "";
		private bool loadFromResources = false;

		public FontFieldDataObject( T valueDefault ) :base( valueDefault ){}

		public void Set( string title, string extension, bool loadFromResources, DFFGUI<T> onGUI, DFFApply<T> onApply ){
			this.extension = extension;
			this.loadFromResources = loadFromResources;
			base.Set( title, onGUI, onApply );
		}

		protected override bool IsSerializeCustom{ get{ return true; } }
		protected const string KEY_LOAD = "lfr";
		protected const string KEY_PATH = "p";
		
		protected FontFieldDataObject( SerializationInfo info, StreamingContext context ) :base( info, context ){
			loadFromResources = info.GetBoolean( KEY_LOAD );
			bool isPath = info.GetBoolean( KEY_PATH );
			string path = info.GetString( KEY_VALUE );
			
			if( string.IsNullOrEmpty( path ) ){
				value = default(T);
				return;
			}

			if( isPath == true ){
				value = AssetDatabase.LoadAssetAtPath<T>( path );

			}else{
				if( loadFromResources == true ){
					value = Resources.GetBuiltinResource<T>( path );

				}else{
					value = AssetDatabase.GetBuiltinExtraResource<T>( path );
				}
			}
		}  
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ){
			base.GetObjectData( info, context );

			info.AddValue( KEY_LOAD, loadFromResources );

			string path = AssetDatabase.GetAssetPath( value );
			T obj = AssetDatabase.LoadAssetAtPath<T>( path );
			if( obj == null
				&& value != null
			){
				path = value.name + "." + extension;
			}
			info.AddValue( KEY_PATH, obj != null );
			info.AddValue( KEY_VALUE, path );
		} 
	}


	
	[System.Serializable]
	private class FontData : IFontGUI{
		[System.AttributeUsage( System.AttributeTargets.Field )]  
		private class FontFieldDataCategory : System.Attribute{
			public string name = "";
			public FontFieldDataCategory(){}
			public FontFieldDataCategory( string name ){
				this.name = name;
			}
		}

		private const string CATEGORY_CHARACTER = "Character";
		private const string CATEGORY_PARAGRAPH = "Paragraph";
		private const string CATEGORY_OTHER = "";
		private static string[] CATEGORIES = { CATEGORY_CHARACTER, CATEGORY_PARAGRAPH, CATEGORY_OTHER };

		public string name = "Font New";

#region Category Character
		[FontFieldDataCategory( CATEGORY_CHARACTER )]
		private FontFieldDataObject<Font> font = null;

		[FontFieldDataCategory( CATEGORY_CHARACTER )]
		private FontFieldData<FontStyle> style = null;

		[FontFieldDataCategory( CATEGORY_CHARACTER )]
		private FontFieldData<int> size = null;

		[FontFieldDataCategory( CATEGORY_CHARACTER )]
		private FontFieldData<float> lineSpacing = null;

		[FontFieldDataCategory( CATEGORY_CHARACTER )]
		private FontFieldData<bool> richText = null;
#endregion

#region Category Paragraph
		[FontFieldDataCategory( CATEGORY_PARAGRAPH )]
		private FontFieldData<TextAnchor> alignment = null;

		[FontFieldDataCategory( CATEGORY_PARAGRAPH )]
		private FontFieldData<bool> alignByGeometry = null;

		[FontFieldDataCategory( CATEGORY_PARAGRAPH )]
		private FontFieldData<HorizontalWrapMode> horizontalOverflow = null;

		[FontFieldDataCategory( CATEGORY_PARAGRAPH )]
		private FontFieldData<VerticalWrapMode> verticalOverflow = null;

		[FontFieldDataCategory( CATEGORY_PARAGRAPH )]
		private FontFieldData<bool> bestFit = null;
		private int bestFitMin = 0, bestFitMax = 300;
#endregion

#region Category Other
		[FontFieldDataCategory( CATEGORY_OTHER )]
		private FontFieldDataColor color = null;

		[FontFieldDataCategory()]
		private FontFieldDataObject<Material> material = null;

		[FontFieldDataCategory()]
		private FontFieldData<bool> raycastTarget = null;
#endregion



		private Dictionary<string, List<IFontGUI>> fields = null;
		private Dictionary<string, List<IFontGUI>> Fields{
			get{
				if( fields == null ){
					// prepare dict
					fields = new Dictionary<string, List<IFontGUI>>();
					for( int i = 0; i < CATEGORIES.Length; i++ ){
						fields[CATEGORIES[i]] = new List<IFontGUI>();
					}
					// set fields
					FieldInfo[] fieldsInfo = MyOperationClass.GetFields<FontFieldDataCategory>( this );
					for( int i = 0; i < fieldsInfo.Length; i++ ){
						FontFieldDataCategory category = fieldsInfo[i].GetAttributeField<FontFieldDataCategory>();
						IFontGUI fontGUI = fieldsInfo[i].GetValue( this ) as IFontGUI;
						if( fontGUI != null ){
							((ISetIndent)fontGUI).SetIndent( category.name.Length > 0 ? 20 : 5 );
							fields[category.name].Add( fontGUI );
						}
					}
				}
				return fields;
			}
		}


		public void OnEnable(){
			fields = null;

#region Category Character
			if( font == null ) font = new FontFieldDataObject<Font>( Resources.GetBuiltinResource<Font>( "Arial.ttf" ) );
			font.Set( "Font", "ttf", true,
				( Font value ) =>{ return MyOperationEditor.DrawDropArea<Font>( value ); },
				( Text text, Font value ) =>{ text.font = value; }
			);

			if( style == null ) style = new FontFieldData<FontStyle>( FontStyle.Normal );
			style.Set( "Font Style",
				( FontStyle value ) =>{ return (FontStyle)EditorGUILayout.EnumPopup( value ); },
				( Text text, FontStyle value ) =>{ text.fontStyle = value; }
			);

			if( size == null ) size = new FontFieldData<int>( 14 );
			size.Set( "Font Size",
				( int value ) =>{ return (int)EditorGUILayout.Slider( value, 0, 300f ); },
				( Text text, int value ) =>{ text.fontSize = value; }
			);

			if( lineSpacing == null ) lineSpacing = new FontFieldData<float>( 1 );
			lineSpacing.Set( "Line Spacing",
				( float value ) =>{ return EditorGUILayout.FloatField( value ); },
				( Text text, float value ) =>{ text.lineSpacing = value; }
			);

			if( richText == null ) richText = new FontFieldData<bool>( true );
			richText.Set( "Rich Text",
				( bool value ) =>{ return EditorGUILayout.Toggle( value ); },
				( Text text, bool value ) =>{ text.supportRichText = value; }
			);
#endregion

#region Category Paragraph
			if( alignment == null ) alignment = new FontFieldData<TextAnchor>( TextAnchor.UpperLeft );
			alignment.Set( "Alignment",
				( TextAnchor value ) =>{ return (TextAnchor)EditorGUILayout.EnumPopup( value ); },
				( Text text, TextAnchor value ) =>{ text.alignment = value; }
			);

			if( alignByGeometry == null ) alignByGeometry = new FontFieldData<bool>( false );
			alignByGeometry.Set( "Align By Geometry",
				( bool value ) =>{ return EditorGUILayout.Toggle( value ); },
				( Text text, bool value ) =>{ text.alignByGeometry = value; }
			);

			if( horizontalOverflow == null ) horizontalOverflow = new FontFieldData<HorizontalWrapMode>( HorizontalWrapMode.Wrap );
			horizontalOverflow.Set( "Horizontal Overflow",
				( HorizontalWrapMode value ) =>{ return (HorizontalWrapMode)EditorGUILayout.EnumPopup( value ); },
				( Text text, HorizontalWrapMode value ) =>{ text.horizontalOverflow = value; }
			);

			if( verticalOverflow == null ) verticalOverflow = new FontFieldData<VerticalWrapMode>( VerticalWrapMode.Truncate );
			verticalOverflow.Set( "Vertical Overflow",
				( VerticalWrapMode value ) =>{ return (VerticalWrapMode)EditorGUILayout.EnumPopup( value ); },
				( Text text, VerticalWrapMode value ) =>{ text.verticalOverflow = value; }
			);

			if( bestFit == null ) bestFit = new FontFieldData<bool>( false );
			bestFit.Set( "Best Fit",
				( bool value ) =>{
					value = EditorGUILayout.Toggle( value, GUILayout.Width( 15 ) );
					if( value == true ){
						EditorGUILayout.LabelField( "Min", GUILayout.Width( 30 ) );
						bestFitMin = Mathf.Clamp( EditorGUILayout.IntField( bestFitMin, GUILayout.MinWidth( 30 ) ), 0, size.Value );

						EditorGUILayout.LabelField( "Max", GUILayout.Width( 30 ) );
						bestFitMax = Mathf.Clamp( EditorGUILayout.IntField( bestFitMax, GUILayout.MinWidth( 30 ) ), size.Value, 300 );
					}
					return value;
				},
				( Text text, bool value ) =>{
					text.resizeTextForBestFit = value;
					text.resizeTextMinSize = bestFitMin;
					text.resizeTextMaxSize = bestFitMax;
				}
			);
#endregion
		
#region Category Other
			if( color == null ) color = new FontFieldDataColor( Color.black );
			color.Set( "Color",
				( Color value ) =>{ return (Color)EditorGUILayout.ColorField( value ); },
				( Text text, Color value ) =>{ text.color = value; }
			);

			
			if( material == null ) material = new FontFieldDataObject<Material>( null );
			material.Set( "Material", "mat", false,
				( Material value ) =>{ return MyOperationEditor.DrawDropArea<Material>( value ); },
				( Text text, Material value ) =>{ text.material = value; }
			);

			
			if( raycastTarget == null ) raycastTarget = new FontFieldData<bool>( false );
			raycastTarget.Set( "Raycast Target",
				( bool value ) =>{ return EditorGUILayout.Toggle( value ); },
				( Text text, bool value ) =>{ text.raycastTarget = value; }
			);
#endregion

		}

		public void OnGUI(){
			foreach( string category in Fields.Keys ){
				if( string.IsNullOrEmpty( category ) == false ){
					EditorGUILayout.LabelField( category, EditorStyles.boldLabel );
				}
				for( int i = 0; i < Fields[category].Count; i++ ){
					Fields[category][i].OnGUI();
				}
			}
		}

		public void Apply( Text text ){
			foreach( string category in Fields.Keys ){
				for( int i = 0; i < Fields[category].Count; i++ ){
					Fields[category][i].Apply( text );
				}
			}
		}
	}
	

	private List<FontData> fonts = null;
	private List<FontData> Fonts{
		get{
			if( fonts == null ){
				fonts = new List<FontData>();
			}
			if( fonts.Count == 0 ){
				fonts.Add( new FontData() );
			}
			return fonts;
		}
	}

	private int indexFontCurrent;
	private FontData FontCurrent{
		get{
			return Fonts[indexFontCurrent];
		}
	}

	private Vector2 scrollPosition = Vector2.zero;

	private List<Text> textsUsed = new List<Text>();
	private bool isShowAllInScene = false;
	private bool isIncludeInnactive = false;



	[MenuItem( "Window/UI Manager/Text Manager", false, 1 )]
    public static void ShowWindow(){
		EditorWindow.GetWindow( typeof(UITextManagerEditorWindow), false, "Text Manager" );
    }
	
	private void OnEnable(){
		
		minSize = new Vector2( 340, 100 );

		Load();
		FontCurrent.OnEnable();

	}
	private void OnDisable(){
		
		Save();

	}
	void OnSelectionChange(){
        Repaint();
    }

	private void OnGUI(){

		scrollPosition = GUILayout.BeginScrollView( scrollPosition, false, false, GUILayout.Width( position.width ),  GUILayout.Height( position.height ) );



		OnGUIFontList();

		MyOperationEditor.DrawSeparator();

		FontCurrent.OnGUI();

		MyOperationEditor.DrawSeparator();

		OnGUIFindTexts();

		MyOperationEditor.DrawSeparator();

		OnGUIApply();



		EditorGUILayout.EndScrollView();
	}

	private void OnGUIFontList(){
		
		string[] fontsNames = new string[Fonts.Count];
		for( int i = 0; i < Fonts.Count; i++ ){
			fontsNames[i] = "[" + i + "] " + Fonts[i].name;
		}
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
			int indexFontCurrentNew = EditorGUILayout.Popup( "Current Font", indexFontCurrent, fontsNames );
			// add new
			if( MyOperationEditor.DrawButtonMini( "+", Color.green.Clone( 0.3f ), 18 ) ){
				Fonts.Add( new FontData() );
				indexFontCurrentNew++;
			}
			if( indexFontCurrent != indexFontCurrentNew ){
				indexFontCurrent = indexFontCurrentNew;
				FontCurrent.OnEnable();
			}
			// remove current
			if( MyOperationEditor.DrawButtonMini( "-", Color.red.Clone( 0.3f ), 18 )
				&& EditorUtility.DisplayDialog( "Remove Current Font", "Confirm the deletion \"" + FontCurrent.name + "\"?", "Confirm", "Cancel" )
			){
				Fonts.RemoveAt( indexFontCurrent );
				indexFontCurrent = Mathf.Clamp( indexFontCurrent, 0, Fonts.Count - 1 );
				FontCurrent.OnEnable();
			}
		EditorGUILayout.EndHorizontal();
		FontCurrent.name = EditorGUILayout.TextField( "Name", FontCurrent.name );

	}

	private void OnGUIFindTexts(){
		
		EditorGUILayout.BeginHorizontal();
			isIncludeInnactive = EditorGUILayout.Toggle( isIncludeInnactive, GUILayout.Width( 14 ) );
			EditorGUILayout.LabelField( "Include Innactive (and Resources!)" );
		EditorGUILayout.EndHorizontal();

		GameObject[] selected = Selection.gameObjects;
		if( selected.Length == 0 ){
			// --- FOR ALL IN SCENE
			//Resources.FindObjectsOfTypeAll<Text>();
			//GameObject.FindObjectsOfTypeIncludingAssets
			// find all
			if( isIncludeInnactive == true ){
				textsUsed = new List<Text>( (Text[])Resources.FindObjectsOfTypeAll( typeof(Text) ) );

			// find all only active
			}else{
				textsUsed = new List<Text>( GameObject.FindObjectsOfType<Text>() );
			}

			EditorGUILayout.LabelField( "Setting for <b>all texts</b> in current scene (" + textsUsed.Count +  ")", MyOperationEditor.StyleLabel );
		
		}else{
			// --- FOR ALL IN SELECTED
			EditorGUILayout.LabelField( "Setting for <b>select with childs</b>", MyOperationEditor.StyleLabel );

			if( Selection.activeGameObject != null
				&& MyOperationEditor.DrawButtonMini( "deselect", Color.white.Clone( 0.5f ) )
			){
				Selection.activeGameObject = null;
			}
			
			// find items in selected
			textsUsed = new List<Text>();
			for( int i = 0; i < selected.Length; i++ ){
				Text[] texts = selected[i].GetComponentsInChildren<Text>( isIncludeInnactive );

				for( int t = 0; t < texts.Length; t++ ){
					if( textsUsed.Contains( texts[t] ) == false ){
						textsUsed.Add( texts[t] );
					}
				}
			}
		}


		if( textsUsed.Count == 0 ){
			EditorGUILayout.LabelField( "Not finded texts..." );

		}else{
			// list items
			if( MyOperationEditor.DrawButtonMini( isShowAllInScene ? "close" : "preview", Color.white.Clone( 0.5f ) ) ){
				isShowAllInScene = !isShowAllInScene;
			}

			if( isShowAllInScene == true ){
				for( int i = 0; i < textsUsed.Count; i++ ){
					MyOperationEditor.DrawDropObjectWithIndex<Text>( i + 1, textsUsed[i] );
				}
				// list items
				if( MyOperationEditor.DrawButtonMini( isShowAllInScene ? "close" : "preview", Color.white.Clone( 0.5f ) ) ){
					isShowAllInScene = !isShowAllInScene;
				}

				if( Selection.activeGameObject != null
					&& MyOperationEditor.DrawButtonMini( "deselect", Color.white.Clone( 0.5f ) )
				){
					Selection.activeGameObject = null;
				}
			}
		}

	}

	private void OnGUIApply(){
		
		GUI.enabled = textsUsed.Count > 0;

		if( MyOperationEditor.DrawButtonMini( "Apply" ) ){
			for( int i = 0; i < textsUsed.Count; i++ ){
				FontCurrent.Apply( textsUsed[i] );
				EditorUtility.SetDirty( textsUsed[i] );
			}
			EditorSceneManager.MarkAllScenesDirty();
		}

		GUI.enabled = true;

	}
	


	
	/// <summary>
	/// Path at file with setting this window.
	/// </summary>
	private string GetPathFileData(){
		return new FileInfo( Application.dataPath ).Directory.FullName + "/UITextManagerEditor.data";
	}

	/// <summary>
	/// Save setting to file.
	/// </summary>
	private void Save(){
		object[] data = new object[]{ fonts, indexFontCurrent };
		MyOperationFile.Serialize<object[]>( data, GetPathFileData() );
	}
	private void Load(){
		object[] data = MyOperationFile.Deserialize<object[]>( GetPathFileData() );
		if( data != null ){
			fonts = (List<FontData>)data[0];
			indexFontCurrent = (int)data[1];
		}
	}

}
