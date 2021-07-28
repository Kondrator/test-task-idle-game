using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Kondrat.MVC {
    
    [CustomEditor( typeof(Controller), true )]
    public class ControllerEditor : Editor {

        protected Controller controller = null;

        private void OnEnable() {
            controller = base.target as Controller;

        }


        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if( controller.gameObject.activeInHierarchy == false ){
                return;
            }

            if( MyOperationEditor.DrawButtonMini( "setting mvc" ) ) {
                CreatePart( controller.gameObject, "Model" );
                CreatePart( controller.gameObject, "View" );
                CreatePart( controller.gameObject, "Controller" );
            }

            /*if( MyOperationEditor.DrawButtonMini( "setting mvc children" ) ) {
                CreateChildren( "Model" );
                CreateChildren( "View" );
                CreateChildren( "Controller" );

                DestroyImmediate( controller );
            }*/
        }
        

        private void CreatePart( GameObject target, string part ) {
            string name = controller.GetType().FullName.Replace( "Controller", part );

            Type typeModel = MyOperationClass.GetType( name );
            if( typeModel == null ) {
                return;
            }

            Component component = target.GetComponent( typeModel );
            if( component != null ) {
                MoveComponentToEnd( component );
                return;
            }

            component = target.AddComponent( typeModel );
            MoveComponentToEnd( component );
        }

        private void CreateChildren( string part ) {
            GameObject goPart = new GameObject( part );
            goPart.transform.SetParent( controller.transform );
            goPart.transform.ResetTransform();

            CreatePart( goPart, part );
         }


        private void MoveComponentToEnd( Component target ) {
            for( int i = 0; i < 10; i++ ) {
                MyOperationEditor.MoveComponentDown( target );
            }
        }

    }

}