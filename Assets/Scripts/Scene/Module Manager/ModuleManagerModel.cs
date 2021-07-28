using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using  UnityEditor;
#endif



namespace ModuleManager {

    public class ModuleManagerModel : Model {




        [SerializeField, HideInInspector]
        private Module[] modules = null;
        public Module[] Modules { get { return modules; } }






#if UNITY_EDITOR

        [CustomEditor( typeof( ModuleManagerModel ), true )]
        public class ModuleManagerModelEditor : Editor<ModuleManagerModel> {


            public override void OnInspectorGUI() {
                base.OnInspectorGUI();

                if( component.modules == null ) {
                    component.modules = new Module[0];
                }


                MyOperationEditor.DrawTitle( "Modules" );
                MyOperationEditor.DrawSpaceVertical( 5 );


                component.modules = MyOperationEditor.DrawArray( new MyOperationEditor.DrawArrayInfo<Module>() {
                    Items = component.modules,

                    DrawContent = item => {

                        item.OnInspectorGUI();

                        return item;
                    },
                    DrawSeparator = item => {
                        MyOperationEditor.DrawSeparator();
                        return item;
                    },

                    IsNeedAdd = () => {
                        MyOperationEditor.DrawSeparator();
                        return true;
                    },
                    
                } );



                if( GUI.changed ) {
                    EditorUtility.SetDirty( component );
                }


            }


        }




        public class ModuleImporter : AssetPostprocessor {


            // update links for modules (if changed asset)
            public static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths ) {

                Module.Paths = null;

                // get prefabs from project
                string[] paths = AssetDatabase.FindAssets( "t:Prefab", new string[] { Module.FOLDER_MODULES } );
                paths = paths.Select( item => AssetDatabase.GUIDToAssetPath( item ) ).ToArray();

                // changed assets
                List<string> assetsChanged = new List<string>();
                assetsChanged.AddRange( deletedAssets );
                assetsChanged.AddRange( movedFromAssetPaths );

                if( assetsChanged.Count == 0 ) {
                    return;
                }

                // optimize
                System.Type typeModules = typeof( Module[] );

                // find field ModuleManagerModel in prefabs
                for( int i = 0; i < paths.Length; i++ ) {

                    // check perfab
                    GameObject goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>( paths[i] );

                    // get components
                    // find need component
                    MonoBehaviour[] components = goPrefab.GetComponentsInChildren<MonoBehaviour>();
                    for( int k = 0; k < components.Length; k++ ) {

                        // get fields in component
                        // find ModuleManagerModel field
                        FieldInfo[] fields = MyOperationClass.GetFields<SerializeField>( components[k] );
                        for( int f = 0; f < fields.Length; f++ ) {

                            // this field is ModuleManagerModel ?
                            if( fields[f].FieldType == typeModules ) {

                                // get value field of ModuleManagerModel
                                Module[] modules = fields[f].GetValue( components[k] ) as Module[];

                                // set changes modules
                                bool hasChanges = false;
                                for( int m = 0; m < modules.Length; m++ ) {
                                    if( assetsChanged.Exists( asset => asset.Contains( modules[m].Path ) ) ) {

                                        string path = AssetDatabase.GUIDToAssetPath( modules[m].GUID );
                                        Object obj = AssetDatabase.LoadAssetAtPath<GameObject>( path );

                                        modules[m].Path = path.Replace( Module.FOLDER_MODULES + "/", "" ).Replace( ".prefab", "" );
                                        modules[m].Name = obj != null ? obj.name : modules[m].Name;

                                        // log
                                        Debug.Log( string.Format( "Updated <b>Module</b> => Set new path <i><b>{0}</b></i>", modules[m].Path ) );

                                        hasChanges = true;
                                    }
                                }

                                // have changes modules ?
                                if( hasChanges == true ) {

                                    // instance prefabe for set and save new params
                                    GameObject goPrefabInstance = PrefabUtility.InstantiatePrefab( goPrefab ) as GameObject;
                                    MonoBehaviour[] componentsInstance = goPrefabInstance.GetComponentsInChildren<MonoBehaviour>();
                                    FieldInfo[] fieldsInstance = MyOperationClass.GetFields<SerializeField>( componentsInstance[k] );

                                    // set new changed value to field
                                    fieldsInstance[f].SetValue( componentsInstance[k], modules );

                                    // save prefab (apply changes)
                                    PrefabUtility.ApplyPrefabInstance( goPrefabInstance, InteractionMode.AutomatedAction );
                                    Object.DestroyImmediate( goPrefabInstance );

                                    // log
                                    Debug.Log( string.Format( "Updated <b>ModuleManagerModel Link</b> in asset <b>{0}</b> <i>(field <b>{1}</b>)</i>", paths[i], fields[f].Name ) );
                                }
                            }

                        }
                    }
                }


            }// function

        }// class

#endif


    }

}