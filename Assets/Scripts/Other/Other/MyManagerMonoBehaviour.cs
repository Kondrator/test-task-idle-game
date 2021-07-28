using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;


/// <summary>
/// Из этого класса производится запуск функции когда происходит Update / LateUpdate / FixedUpdate.
/// </summary>
public class MyManagerMonoBehaviour : MonoBehaviour {
	

/*


	void Start(){
		MyManagerMonoBehaviour.Add( this, MyManagerMonoBehaviourType.Update );
	}



	void OnEnable(){
		MyManagerMonoBehaviour.Add( this, MyManagerMonoBehaviourType.Update );
	}
	void OnDisable(){
		MyManagerMonoBehaviour.Remove( this );
	}
 


	void IMyManagerMonoBehaviour.UpdateMe( float timeDelta, MyManagerMonoBehaviourType type ){
		
	}
	
	
*/



	private static MyManagerMonoBehaviour singleton_;
	// for fix create after destroyed game
	private static bool isInstanced = false;

	// если этого объекта не существует на сцене, он будет автоматически создан
	public static MyManagerMonoBehaviour singleton{
		get{
			if( singleton_ == null ){
				if( isInstanced == true ){
					return null;
				}

				new GameObject( typeof(MyManagerMonoBehaviour).Name, new System.Type[]{ typeof(MyManagerMonoBehaviour) } );
				isInstanced = true;
			}

			return singleton_;
		}
	}



	private List<Item> updates;

	void Awake(){

		if( singleton_ != null ){
			Destroy( this );
			return;
		}

		updates = new List<Item>();

		singleton_ = this;
		DontDestroyOnLoad( this.gameObject );
	}




	void Update(){
		UpdateMe( Time.deltaTime, MyManagerMonoBehaviourType.Update );
	}
	void LateUpdate(){
		UpdateMe( Time.deltaTime, MyManagerMonoBehaviourType.LateUpdate );
	}
	void FixedUpdate(){
		UpdateMe( Time.fixedDeltaTime, MyManagerMonoBehaviourType.FixedUpdate );
	}


	
	private void UpdateMe( float timeDelta, MyManagerMonoBehaviourType type ){

		for( int i = 0; i < updates.Count; i++ ){

			Item item = updates[i];
			if( item.IsNeedRemove() == true ){
				updates.RemoveAt( i-- );
				continue;
			}

			if( item.initiator.isActiveAndEnabled == true
				&& (item.type & type) == type
			){
				item.listener.UpdateMe( timeDelta, type );
			}

		}

	}





	public static void Add( MonoBehaviour initiator, MyManagerMonoBehaviourType type = MyManagerMonoBehaviourType.Update ){

		if( singleton == null
			|| initiator == null
		){
			return;
		}

		IMyManagerMonoBehaviour listener = initiator as IMyManagerMonoBehaviour;

		if( listener == null ){
			return;
		}

		for( int i = 0; i < singleton.updates.Count; i++ ){
			if( singleton.updates[i].initiator == initiator ){
				return;
			}
		}

		singleton.updates.Add( new Item( initiator, listener, type ) );
	}

	public static void Remove( MonoBehaviour initiator ){

		if( singleton == null
			|| initiator == null
		){
			return;
		}

		for( int i = 0; i < singleton.updates.Count; i++ ){
			
			if( singleton.updates[i].initiator == initiator ){
				singleton.updates.RemoveAt( i );
				break;
			}

		}

	}





	private class Item{
		public MonoBehaviour initiator;
		public IMyManagerMonoBehaviour listener;
		public MyManagerMonoBehaviourType type;

		public Item( MonoBehaviour initiator, IMyManagerMonoBehaviour listener, MyManagerMonoBehaviourType type = MyManagerMonoBehaviourType.Update ){
			this.initiator = initiator;
			this.listener = listener;
			this.type = type;
		}
		
		public bool IsNeedRemove(){
			return initiator == null;
		}
	}

}


[Flags]
public enum MyManagerMonoBehaviourType{
	Update = 1,
	LateUpdate = 2,
	FixedUpdate = 4
}

public interface IMyManagerMonoBehaviour{
	/// <summary>
	/// Обновление.
	/// </summary>
	/// <param name="timeDelta">Время обновления.</param>
	void UpdateMe( float timeDelta, MyManagerMonoBehaviourType type );
}