using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace UIWindowManager{

	public class UIWindowListGeneratorEditorWindow : EditorWindow {

		private class GUIGenerateName{
			private string name = "";
			private string label = "";
			private bool isAuto = true;
			private System.Func<string> dGenerateName = null;
		
			private static float widthLabel = 160;

			public string Name{
				get{
					if( isAuto == true ){
						return dGenerateName();
					}
					return name;
				}
				set{
					if( isAuto == false ){
						name = value;
					}
				}
			}

			public void ResetName(){
				name = dGenerateName();
			}

			public bool IsCorrect(){
				return !string.IsNullOrEmpty( Name );
			}

			public GUIGenerateName( string label, System.Func<string> dGenerateName, bool isAuto = true ){
				this.label = label;
				this.isAuto = isAuto;
				this.dGenerateName = dGenerateName;
				this.name = dGenerateName();
			}

			public void OnGUI(){
			
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( label, MyOperationEditor.StyleLabel, GUILayout.Width( widthLabel ) );
					isAuto = EditorGUILayout.Toggle( isAuto, GUILayout.Width( 14 ) );
					GUI.enabled = !isAuto;
						Name = GUIGenerateName.FixName( EditorGUILayout.TextArea( Name ) );
					GUI.enabled = true;
				EditorGUILayout.EndVertical();

			}

			public void OnGUISimple(){
			
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( label, MyOperationEditor.StyleLabel, GUILayout.Width( widthLabel ) );
					Name = GUIGenerateName.FixName( EditorGUILayout.TextArea( Name ) );
				EditorGUILayout.EndVertical();

			}

			public static string FixName( string name ){
				Regex regex = new Regex( @"[\W]+|^[\d]+" );
				return regex.Replace( name, "" );
			}

		}

	
		private static GUIGenerateName guiNameGeneral = new GUIGenerateName( "<b>General Name</b>", () =>{ return "GenerateExampleName"; }, false );
		private static GUIGenerateName guiNameWindow = new GUIGenerateName( "Name class of <b>Window</b>", () =>{ return "UIWindowList" + guiNameGeneral.Name; } );
		private static GUIGenerateName guiNameItem = new GUIGenerateName( "Name class of <b>List Item</b>", () =>{ return guiNameWindow.Name + "Item"; } );
		private static GUIGenerateName guiNameData = new GUIGenerateName( "Name class of <b>Item Data</b>", () =>{ return "Data" + guiNameGeneral.Name; } );

		[MenuItem( "Window/UI Manager/Generate classes for List", false, 0 )]
		public static void ShowWindow(){
			EditorWindow.GetWindow( typeof(UIWindowListGeneratorEditorWindow), false, "Generator List" );
		}

		void OnEnable(){
			Reset();
			minSize = new Vector2( 500, 95 );
			maxSize = new Vector2( 700, 95 );
		}

		private void Reset(){
			guiNameGeneral.ResetName();
			guiNameWindow.ResetName();
			guiNameItem.ResetName();
			guiNameData.ResetName();
		}


		void OnGUI(){

			guiNameGeneral.OnGUISimple();
			guiNameWindow.OnGUI();
			guiNameItem.OnGUI();
			guiNameData.OnGUI();

			EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Reset", GUILayout.Width( 60 ) ) ){
					GUI.FocusControl( "" );
					Reset();
				}
				GUI.enabled =	guiNameGeneral.IsCorrect()
								&& guiNameWindow.IsCorrect()
								&& guiNameItem.IsCorrect()
								&& guiNameData.IsCorrect();
					if( GUILayout.Button( "Generate" ) ){
						GUI.FocusControl( "" );
						string path = EditorUtility.OpenFolderPanel( "Select folder for save C# files.", "Assets", "" );
						Save( path );
					}
				GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
		
			if( Event.current.keyCode == KeyCode.Return
				|| Event.current.keyCode == KeyCode.KeypadEnter
			){
				GUI.FocusControl( "" );
				Repaint();
			}

		}

		private void Save( string pathFolder ){

			if( Directory.Exists( pathFolder ) == false ){
				return;
			}
		
			string pathClassWindow = pathFolder + "/" + guiNameWindow.Name + ".cs";
			string pathClassItem = pathFolder + "/" + guiNameItem.Name + ".cs";
		
			if(	CheckFile( pathClassWindow ) == false
				|| CheckFile( pathClassWindow ) == false
			){
				return;
			}
		
			File.WriteAllText( pathClassWindow, GenerateFileWindow() );
			File.WriteAllText( pathClassItem, GenerateFileItem() );

			EditorUtility.DisplayDialog( "Generate Files", "Files generate success!", "Finish" );
			Close();
			AssetDatabase.Refresh();

		}

		private bool CheckFile( string pathFile ){
		
			if(	File.Exists( pathFile ) == true ){
				EditorUtility.DisplayDialog( "Error Save File", "File \"" + pathFile + "\" Exists!\nFiles not generated.", "Close" );
				return false;
			}

			return true;
		}

		private string GenerateFileWindow(){
			return 
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWindowManager;

public class " + guiNameWindow.Name + @" : UIWindowListArray<" + guiNameItem.Name + @", " + guiNameData.Name + @"> {

	[SerializeField]
	private int countItemsExample = 5; // for example!

	// only two method:
	// 1) setting each element
	// 2) get data

	// setting at item
	protected override void SettingItemExtra( " + guiNameItem.Name + @" container, " + guiNameData.Name + @" item, int index ){
		container.Set( item );
	}


	// this method executed at open window or only one time
	protected override " + guiNameData.Name + @"[] GetItems(){

		print( ""<b>"" + this.name + ""</b>: get items (count="" + countItemsExample + "")"" );

		// load data (example)
		" + guiNameData.Name + @"[] array = new " + guiNameData.Name + @"[countItemsExample];
		for( int i = 0; i < array.Length; i++ ){
			array[i] = new " + guiNameData.Name + @"( ""Item #"" + (i + 1) + "" (random="" + Random.Range( 5, 20 ) + "")"" );
		}

		// return data
		return array;
	}

}";
		}

		private string GenerateFileItem(){
			return
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWindowManager;

public class " + guiNameItem.Name + @" : MonoBehaviour {

	[SerializeField]
	private Text textItem = null;

	private " + guiNameData.Name + @" data = null;


	public void Set( " + guiNameData.Name + @" data ){
		this.data = data;
	}


	// this method for example!
	void Update(){
		if( textItem != null
			&& data != null
		){
			textItem.text = data.text;
		}
	}

}

public class " + guiNameData.Name + @"{
	public string text = " + "\"\"" + @";

	public " + guiNameData.Name + @"( string text ){
		this.text = text;
	}
}";
		}

	}

}