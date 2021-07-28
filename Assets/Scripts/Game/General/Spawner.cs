using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class Spawner : Element {

    public class NOTIFY {

        public class SPAWNED {

            public const string NAME = "spawner.spawned";

        }


        /// <summary>
        /// Type = GameObject
        /// </summary>
        public const string PARAM_INSTANCE = "instance";

    }



    [Header( "Instance" )]

    [SerializeField]
    [Tooltip( "Set this parent for target instance" )]
    private Transform container = null;
    public Transform Container { get { return container ??= this.transform; } }

    [SerializeField]
    [Tooltip( "Set this position and rotation to target instance" )]
    private Transform transformTarget = null;
    public Transform TransformTarget { get { return transformTarget ??= this.transform; } }

    [SerializeField]
    private Transform target = null;



    [Header( "Params" )]

    [SerializeField]
    [Range( 0.1f, 10f )]
    private float radius = 5f;




    public void Spawn() {
        if( target == null ) {
            return;
        }

        Vector3 positionInRadius = Random.insideUnitSphere * radius;
        positionInRadius.y = 0;

        GameObject instance = PoolGameObject.Get( target.gameObject, Container );
        instance.transform.ResetTransform();
        instance.transform.position = TransformTarget.position + positionInRadius;
        instance.transform.rotation = TransformTarget.rotation;

        Notify(
            NOTIFY.SPAWNED.NAME,
            new NotifyData.Param( NOTIFY.PARAM_INSTANCE, instance )
        );

    }




#if UNITY_EDITOR
    [CustomEditor( typeof( Spawner ) )]
    private class SpawnerEditor : Editor<Spawner> {

        private void OnSceneGUI() {
            Handles.DrawWireDisc( component.transform.position, component.transform.up, component.radius );
        }

    }
#endif

}
