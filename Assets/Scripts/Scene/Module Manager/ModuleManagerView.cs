using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ModuleManager {

    public class ModuleManagerView : View {


        private SceneView sceneView = null;
        public SceneView SceneView { get { return sceneView = Find( sceneView ); } }


    }

}