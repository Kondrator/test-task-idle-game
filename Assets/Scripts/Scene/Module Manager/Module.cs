using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif




namespace ModuleManager {

    [System.Serializable]
    public class Module {

        [SerializeField]
        private string name = "";
        public string Name { get { return name; } set { name = value; } }

        [SerializeField]
        private string path = "";
        public string Path { get { return path; } set { path = value; } }

        [SerializeField]
        private string parent = "";
        public string Parent { get { return parent; } set { parent = value; } }


        [System.Serializable]
        public enum TypeParent {
            Relative = 0,
            Absolute = 1,
        }

        [SerializeField]
        private TypeParent parentType = TypeParent.Relative;
        public TypeParent ParentType { get { return parentType; } set { parentType = value; } }


        [SerializeField]
        private bool useParentForOther = false;
        public bool UseParentForOther { get { return useParentForOther; } set { useParentForOther = value; } }

        [SerializeField]
        private string parentForOther = "";
        public string ParentForOther { get { return parentForOther; } set { parentForOther = value; } }






#if UNITY_EDITOR

        [SerializeField]
        private string guid = "";
        public string GUID { get { return guid; } set { guid = value; } }

        [SerializeField]
        private bool isParentCustom = false;


        public const string FOLDER_MODULES = "Assets/Resources/Modules";

        public void OnInspectorGUI() {

            // name
            name = EditorGUILayout.TextField( "Name", name );

            // path
            DrawPath();

            // parent
            DrawParent();

            // parent for other
            DrawUseParentOther();

        }







        private const string ID_POPUP_PATS = "module.manager.paths.modules";

        private static string[] paths = null;
        public static string[] Paths {
            get {
                if( paths == null ) {
                    MyOperationEditor.DrawPopupClearCache( ID_POPUP_PATS );

                    paths = AssetDatabase.FindAssets( "t:Prefab", new string[] { FOLDER_MODULES } );
                    paths = paths.Select( item => AssetDatabase.GUIDToAssetPath( item ) ).ToArray();
                    paths = paths.Select( item => item.Replace( FOLDER_MODULES + "/", "" ) ).ToArray();
                    paths = paths.Select( item => item.Replace( ".prefab", "" ) ).ToArray();
                }
                return paths;
            }
            set {
                paths = value;
            }
        }

        private void DrawPath() {

            string pathNew = MyOperationEditor.DrawPopup( ID_POPUP_PATS, "Module", path, Paths );

            if( pathNew != path ) {
                path = pathNew;
                UpdateGUID();

                if( pathNew.Contains( "/" ) ) {
                    name = pathNew.Split( '/' ).Last();

                } else {
                    name = pathNew;
                }

                parentForOther = name;
            }

            if( string.IsNullOrEmpty( guid ) ) {
                UpdateGUID();
            }

        }

        /// <summary>
        /// Update guid by path
        /// </summary>
        public void UpdateGUID() {
            guid = AssetDatabase.AssetPathToGUID( FOLDER_MODULES + "/" + path + ".prefab" );
        }

        /// <summary>
        /// Update path by guid
        /// </summary>
        public void UpdatePath() {
            path = AssetDatabase.GUIDToAssetPath( guid );
        }





        private const string ID_POPUP_PARENTS = "module.manager.paths.parents";

        private static string[] parents = null;
        private static string[] Parents {
            get {
                if( parents == null ) {
                    MyOperationEditor.DrawPopupClearCache( ID_POPUP_PARENTS );

                    List<string> parentsList = new List<string>();

                    string[] paths = AssetDatabase.FindAssets( "t:Prefab", new string[] { FOLDER_MODULES } );
                    paths = paths.Select( item => AssetDatabase.GUIDToAssetPath( item ) ).ToArray();

                    List<ModuleManagerModel> moduleManagerModels = new List<ModuleManagerModel>();
                    moduleManagerModels.AddRange( Object.FindObjectsOfType<ModuleManagerModel>() );

                    for( int i = 0; i < paths.Length; i++ ) {
                        ModuleManagerModel moduleManagerModel = AssetDatabase.LoadAssetAtPath<ModuleManagerModel>( paths[i] );
                        if( moduleManagerModel != null ) {
                            moduleManagerModels.Add( moduleManagerModel );
                        }
                    }

                    for( int i = 0; i < moduleManagerModels.Count; i++ ) {
                        for( int m = 0; m < moduleManagerModels[i].Modules.Length; m++ ) {

                            if( moduleManagerModels[i].Modules[m].UseParentForOther == false ) {
                                continue;
                            }

                            parentsList.Add( moduleManagerModels[i].Modules[m].ParentForOther );

                        }
                    }

                    parents = parentsList.ToArray();
                }
                return parents;
            }
        }

        private void DrawParent() {

            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField( "Parent", GUILayout.Width( EditorGUIUtility.labelWidth - 5 ) );  

                isParentCustom = EditorGUILayout.Toggle( isParentCustom, GUILayout.Width( 13 ) );


                if( isParentCustom == true ) {
                    parent = EditorGUILayout.TextField( parent );
                    parentType = (TypeParent)EditorGUILayout.EnumPopup( parentType, GUILayout.Width( 70 ) );

                } else {
                    parent = MyOperationEditor.DrawPopup( ID_POPUP_PARENTS, parent, Parents );
                    parentType = (TypeParent)EditorGUILayout.EnumPopup( parentType, GUILayout.Width( 70 ) );
                }


                if( MyOperationEditor.DrawButtonMini( "clear", 40 ) ){
                    parent = "";
                }

            EditorGUILayout.EndHorizontal();

        }



        private void DrawUseParentOther() {

            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField( "Use Parent For Other", GUILayout.Width( EditorGUIUtility.labelWidth - 5 ) );
                
                bool useParentForOtherNew = EditorGUILayout.Toggle( useParentForOther, GUILayout.Width( 13 ) );

                if( useParentForOtherNew != useParentForOther ) {
                    useParentForOther = useParentForOtherNew;
                    parentForOther = useParentForOther ? name : "";
                    parents = null;
                }

                if( useParentForOther == true ) {
                    parentForOther = EditorGUILayout.TextField( parentForOther );
                }

            EditorGUILayout.EndHorizontal();

        }

#endif


    }

}