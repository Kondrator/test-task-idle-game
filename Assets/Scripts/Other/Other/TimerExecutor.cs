using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerExecutor : MonoBehaviour {

	public class Item{
		
		// full time (seconds)
		private float length = 0;
		// current time (seconds)
		private float current = 0;
		// step execution
		private float freequency = 0;
		// time after last execution (seconds)
		private float execution = 0;

		// executetion method
		private System.Action<float> dExecution = null;
		private System.Action dCompleted = null;

		private bool isPause = false;


		public bool IsPaused{
			get{
				return isPause;
			}
		}

		/// <summary>
		/// This executor is completed?
		/// </summary>
		public bool IsCompleted{
			get{
				return current >= length;
			}
		}


		/// <param name="length">Full time (seconds).</param>
		/// <param name="freequency">Step execution callback.</param>
		/// <param name="dExecution">Execution on step (send progress).</param>
		/// <param name="dCompleted">Execution on end timer.</param>
		public Item( float length, float freequency, System.Action<float> dExecution, System.Action dCompleted = null ){
			this.length = length;
			this.freequency = freequency;
			this.dExecution = dExecution;
			this.dCompleted = dCompleted;
			this.current = 0;
			this.execution = 0;
		}

		public void Update( float timeDelta ){

			execution -= timeDelta;
			
			if( isPause == false ){
				current += timeDelta;
			}
			
			if( dExecution != null
				&& execution <= 0
			){
				dExecution( Mathf.Clamp01( current / length ) );
				execution = freequency + execution;
			}

		}

		
		public void PauseProgress(){
			isPause = true;
		}
		public void ContinueProgress(){
			isPause = false;
		}



		/// <summary>
		/// Set status this timer at completed.
		/// </summary>
		public void ForceComplete( bool isObserver = true ){
			current = length;

			if( isObserver == true ){
				Completed();
			
			}else{
				dCompleted = null;
			}
			
		}

		public void Completed(){
			current = length;

			if( dCompleted != null ){
				dCompleted();
				dCompleted = null;
			}
		}

	}




	
	private static TimerExecutor singleton_ = null;
	private static TimerExecutor singleton{
		get{
			if( singleton_ == null ){
				GameObject goNew = new GameObject( typeof(TimerExecutor).Name );
				singleton_ = goNew.AddComponent<TimerExecutor>();
			}
			return singleton_;
		}
	}

	// check in OnDestroy for use property 'singleton'?
	public static bool IsInstance{ get{ return singleton_ != null; } }

	private List<Item> items = null;
	

	void Awake(){
		items = new List<Item>();
	}

	void Update(){
		
		float timeDelta = Time.deltaTime;

		for( int i = 0; i < items.Count; i++ ){
			
			if( items[i].IsCompleted == true ){
				items[i].Completed();
				items.RemoveAt( i-- );
				continue;
			}

			items[i].Update( timeDelta );

		}

	}

	
	/// <summary>
	/// Add timer by every freequency.
	/// </summary>
	/// <param name="length">Duration time (seconds).</param>
	/// <param name="freequency">Step execution callback.</param>
	/// <param name="dExecution">Execution on step (send progress).</param>
	/// <param name="dCompleted">Execution on end timer.</param>
	public static Item Add( float length, float freequency, System.Action<float> dExecution, System.Action dCompleted ){

		Item item = new Item( length, freequency, dExecution, dCompleted );
		singleton.items.Add( item );

		return item;
	}
	/// <summary>
	/// Add timer by every freequency.
	/// </summary>
	/// <param name="length">Duration time (seconds).</param>
	/// <param name="freequency">Step execution callback.</param>
	/// <param name="dExecution">Execution on step (send progress).</param>
	public static Item Add( float length, float freequency, System.Action<float> dExecution ){
		return Add( length, freequency, dExecution, null );
	}


	/// <summary>
	/// Add timer by every Update.
	/// </summary>
	/// <param name="length">Duration time (seconds).</param>
	/// <param name="dExecution">Execution on step (send progress).</param>
	/// <param name="dCompleted">Execution on end timer.</param>
	public static Item Add( float length, System.Action<float> dExecution, System.Action dCompleted ){
		return Add( length, 0, dExecution, dCompleted );
	}
	/// <summary>
	/// Add timer by every Update.
	/// </summary>
	/// <param name="length">Duration time (seconds).</param>
	/// <param name="dExecution">Execution on step (send progress).</param>
	public static Item Add( float length, System.Action<float> dExecution ){
		return Add( length, dExecution, null );
	}

	
	/// <summary>
	/// Add timer for only check completed.
	/// </summary>
	/// <param name="length">Duration time (seconds).</param>
	/// <param name="dCompleted">Execution on end timer.</param>
	public static Item Add( float length, System.Action dCompleted ){
		return Add( length, 0, null, dCompleted );
	}

	/// <summary>
	/// Add timer for only check completed.
	/// Executed at next frame.
	/// </summary>
	/// <param name="dCompleted">Execution on end timer.</param>
	public static Item Add( System.Action dCompleted ) {
		return Add( 0, 0, null, dCompleted );
	}

}
