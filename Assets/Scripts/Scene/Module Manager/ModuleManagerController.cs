using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UIWindowManager;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace ModuleManager {

    public class ModuleManagerController : Controller<ModuleManagerModel, ModuleManagerView>, IListenerLoaderSceneProgress {


        protected override void PreInitiate() { }

        protected override void Initiate() { }




        public IEnumerator SceneLoading() {

            yield return null;

            Transform scene = View.SceneView?.transform;


            for( int i = 0; i < Model.Modules.Length; i++ ) {
                ResourceRequest module = Resources.LoadAsync( "Modules/" + Model.Modules[i].Path );
                yield return module;

                if( module.asset ) {
                    GameObject gameObjectmodule = module.asset as GameObject;

                    GameObject gameObject = GameObject.Instantiate( gameObjectmodule ) as GameObject;
                    gameObject.name = module.asset.name;

                    Transform parent = null;
                    switch( Model.Modules[i].ParentType ) {
                        case Module.TypeParent.Relative:
                            parent = MyOperationTransform.GetPath( this.transform.parent, Model.Modules[i].Parent );
                            break;
                        case Module.TypeParent.Absolute:
                            parent = MyOperationTransform.GetPath( scene, Model.Modules[i].Parent );
                            break;
                    }
                    gameObject.transform.SetParent( parent );

                    gameObject.transform.localPosition = gameObjectmodule.transform.localPosition;
                    gameObject.transform.localRotation = gameObjectmodule.transform.localRotation;
                    gameObject.transform.localScale = gameObjectmodule.transform.localScale;

                    if( gameObject.transform is RectTransform ) {
                        RectTransform rectTarget = (RectTransform)gameObject.transform;
                        RectTransform rectSource = (RectTransform)gameObjectmodule.transform;

                        rectTarget.CopyFrom( rectSource );
                    }


                    yield return null;

                    // loading submodules
                    IListenerLoaderSceneProgress[] modules = gameObject.GetComponentsInChildren<IListenerLoaderSceneProgress>();
                    if( modules != null ) {
                        for( int m = 0; m < modules.Length; m++ ) {
                            yield return modules[m].SceneLoading();
                        }
                    }
                }
            }

            yield return null;

            Destroy( this.gameObject );

        }

    }

}