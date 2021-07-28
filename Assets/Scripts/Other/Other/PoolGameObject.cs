using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pooling game objects.
/// If create GameObject from this class - for pooling use 'this.gameObject.SetActive( false )' instead destroy.
/// </summary>
public class PoolGameObject : MonoBehaviour {
	
	private static PoolGameObject singleton_;
	private static PoolGameObject singleton{
		get{
			if( singleton_ == null ){
				GameObject goNew = new GameObject( typeof(PoolGameObject).Name );
				singleton_ = goNew.AddComponent<PoolGameObject>();
			}
			return singleton_;
		}
	}

	private Dictionary<int, List<GameObject>> pool;
	private Dictionary<int, List<GameObject>> Pool{
		get{
			if( pool == null ){
				pool = new Dictionary<int, List<GameObject>>();
			}
			return pool;
		}
	}



	

	public static Transform TransformForOther{
		get{
			if( singleton.transformForOther == null ){
				singleton.transformForOther = new GameObject( "Container for Other" ).transform;
				singleton.transformForOther.SetParent( singleton.transform );
			}
			return singleton.transformForOther;
		}
	}
	private Transform transformForOther = null;


	public static Transform TransformForUI {
		get{
			if( singleton.transformForUI == null ){
				singleton.transformForUI = new GameObject( "Container for UI", typeof(Canvas) ).transform;
				singleton.transformForUI.SetParent( singleton.transform );
			}
			return singleton.transformForUI;
		}
	}
	private Transform transformForUI = null;




	/// <summary>
	/// Return gameobject form cache or instantiate if not have in cache.
	/// </summary>
	/// <param name="original">Original/target gameobject.</param>
	/// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
	public static GameObject Get( GameObject original, bool isSetActiveTrue = true ){
		
		if( original == null ){
			return new GameObject( "Cahce == NULL" );
		}
		
		// identifier gameobject
		int idOriginal = original.GetInstanceID();

		// check have gameobject
		if( singleton.Pool.ContainsKey( idOriginal ) == false ){
			singleton.Pool[idOriginal] = new List<GameObject>();
		}

		// gameobject list at identifier
		List<GameObject> objects = singleton.Pool[idOriginal];

		// returns this gameobject
		GameObject objGet = null;

		// check inactive
		for( int i = 0; i < objects.Count; i++ ){
			if( objects[i] == null ){
#if UNITY_EDITOR
				print( "objects[" + i + "] is NULL !!!" );
#endif
				objects.RemoveAt( i-- );
				continue;
			}

			if( objects[i].activeSelf == false 
				&& (	objects[i].transform.parent == null
						|| objects[i].transform.parent == TransformForOther
						|| objects[i].transform.parent == TransformForUI )
			){
				objGet = objects[i];
				break;
			}
		}

		// not have inactive
		if( objGet == null ){
			// create a new
			objGet = MyOperation.Instantiate( original );
			objects.Add( objGet );
		}

		// need activated
		if( objGet.activeSelf != isSetActiveTrue ){
			objGet.SetActive( isSetActiveTrue );
		}
		
		return objGet;
	}
	

	/// <summary>
	/// Return gameobject form cache or instantiate if not have in cache.
	/// </summary>
	/// <param name="original">Original/target gameobject.</param>
	/// <param name="parent">Set parent.</param>
	/// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
	public static GameObject Get( GameObject original, Transform parent, bool isSetActiveTrue = true ){
		
		GameObject objGet = Get( original, isSetActiveTrue );
		
		if( objGet == null ){
			return null;
		}

		objGet.transform.SetParent( parent );

		return objGet;
	}


    /// <summary>
	/// Return gameobject form cache or instantiate if not have in cache.
	/// </summary>
	/// <param name="original">Original/target gameobject.</param>
	/// <param name="parent">Set parent.</param>
    /// <param name="position">Set position of instance GameObject.</param>
	/// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
	public static GameObject Get(GameObject original, Transform parent, Vector3 position, bool isSetActiveTrue = true)
    {

        GameObject objGet = Get( original, parent, isSetActiveTrue );

        if (objGet == null)
        {
            return null;
        }

        objGet.transform.position = position;

        return objGet;
    }









    /// <summary>
    /// Return component (gameobject) form cache or instantiate if not have in cache.
    /// </summary>
    /// <typeparam name="T">Type of component from gameobject.</typeparam>
    /// <param name="original">Original/target gameobject.</param>
    /// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
    public static T Get<T>( GameObject original, bool isSetActiveTrue = true ) where T : MonoBehaviour{
		
		GameObject objGet = Get( original, isSetActiveTrue );
		
		if( objGet == null ){
			return default(T);
		}

		return objGet.GetComponent<T>();
	}
	

	/// <summary>
	/// Return component (gameobject) form cache or instantiate if not have in cache.
	/// </summary>
	/// <typeparam name="T">Type of component from gameobject.</typeparam>
	/// <param name="original">Original/target gameobject.</param>
	/// <param name="parent">Set parent.</param>
	/// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
	public static T Get<T>( GameObject original, Transform parent, bool isSetActiveTrue = true ) where T : MonoBehaviour{
		
		GameObject objGet = Get( original, parent, isSetActiveTrue );
		
		if( objGet == null ){
			return default(T);
		}

		return objGet.GetComponent<T>();
	}

	/// <summary>
	/// Return component (gameobject) form cache or instantiate if not have in cache.
	/// </summary>
	/// <typeparam name="T">Type of component from gameobject.</typeparam>
	/// <param name="original">Original/target gameobject.</param>
	/// <param name="position">Set gameobject in position (World Coordination).</param>
	/// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
	public static T Get<T>( GameObject original, Vector2 position, bool isSetActiveTrue = true ) where T : MonoBehaviour{
		
		GameObject objGet = Get( original, null, position, isSetActiveTrue );
		
		if( objGet == null ){
			return default(T);
		}

		return objGet.GetComponent<T>();
	}

	/// <summary>
	/// Return component (gameobject) form cache or instantiate if not have in cache.
	/// </summary>
	/// <typeparam name="T">Type of component from gameobject.</typeparam>
	/// <param name="original">Original/target gameobject.</param>
	/// <param name="parent">Set parent.</param>
	/// <param name="position">Set gameobject in position (World Coordination).</param>
	/// <param name="isSetActiveTrue">True - Enable GameObject before return, False - Disable GameObject before return.</param>
	public static T Get<T>( GameObject original, Transform parent, Vector2 position, bool isSetActiveTrue = true ) where T : MonoBehaviour{
		
		GameObject objGet = Get( original, parent, position, isSetActiveTrue );
		
		if( objGet == null ){
			return default(T);
		}

		return objGet.GetComponent<T>();
	}






	/// <summary>
	/// Get all objects in pool (Activate and Deactivate).
	/// </summary>
	/// <typeparam name="T">Type objects.</typeparam>
	/// <param name="original">Original GameObject (created from her).</param>
	public static List<T> GetAll<T>( GameObject original ) where T : MonoBehaviour{
		
		if( original == null ){
			return new List<T>();
		}
		
		// identifier gameobject
		int idOriginal = original.GetInstanceID();

		// check have gameobject
		if( singleton.Pool.ContainsKey( idOriginal ) == false ){
			singleton.Pool[idOriginal] = new List<GameObject>();
		}

		// items in cache
		List<GameObject> objects = singleton.Pool[idOriginal];
		List<T> objectsT = new List<T>();

		for( int i = 0; i < objects.Count; i++ ){
			T obj = objects[i].GetComponent<T>();
			if( obj != null ){
				objectsT.Add( obj );
			}
		}
		
		return objectsT;
	}

	/// <summary>
	/// Get deactivated objects in pool.
	/// </summary>
	/// <typeparam name="T">Type objects.</typeparam>
	/// <param name="original">Original GameObject (created from her).</param>
	public static List<T> GetInactive<T>( GameObject original ) where T : MonoBehaviour{
		
		if( original == null ){
			return new List<T>();
		}
		
		// identifier gameobject
		int idOriginal = original.GetInstanceID();

		// check have gameobject
		if( singleton.Pool.ContainsKey( idOriginal ) == false ){
			singleton.Pool[idOriginal] = new List<GameObject>();
		}

		// items in cache
		List<GameObject> objects = singleton.Pool[idOriginal];
		List<T> objectsT = new List<T>();

		for( int i = 0; i < objects.Count; i++ ){
			if( objects[i].activeSelf == false ){
				T obj = objects[i].GetComponent<T>();
				if( obj != null ){
					objectsT.Add( obj );
				}
			}
		}
		
		return objectsT;
	}





	void OnDisable(){
		if( enabled == false ){
			Pool.Clear();
		}
	}


	
}
