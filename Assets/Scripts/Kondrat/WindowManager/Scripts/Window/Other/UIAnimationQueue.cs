using UnityEngine;
using System.Collections;


namespace UIWindowManager{

	public class UIAnimationQueue : UIAnimation {

		[System.Serializable]
		public class UIAnimationQueueItem{
			[SerializeField]
			public UIAnimation animation;
			[SerializeField]
			public float intervalSec = 0.5f;

			public UIAnimationQueueItem( UIAnimation animation ){
				this.animation = animation;
				intervalSec = 0.5f;
			}
			public UIAnimationQueueItem( UIAnimation animation, float intervalSec ){
				this.animation = animation;
				this.intervalSec = intervalSec;
			}
		}

		[SerializeField]
		public UIAnimationQueueItem[] queue;

		private IEnumerator coroutineQueue;


		/// <summary>
		/// Показать анимацию показа очереди.
		/// </summary>
		public override void Show( bool isCycle = true, bool isFast = false ){
			MyOperation.StopCoroutine( GetMonoBehaviour(), ref coroutineQueue );
			if( isActiveAndEnabled == false ){
				return;
			}

			coroutineQueue = CoroutineQueueShow( isCycle );
			GetMonoBehaviour().StartCoroutine( coroutineQueue );
		}

		/// <summary>
		/// Показать анимацию скрытия очереди.
		/// </summary>
		public override void Hide( bool isCycle = true, bool isFast = false ){
			MyOperation.StopCoroutine( GetMonoBehaviour(), ref coroutineQueue );
			if( isActiveAndEnabled == false ){
				return;
			}

			coroutineQueue = CoroutineQueueHide( isCycle );
			GetMonoBehaviour().StartCoroutine( coroutineQueue );
		}

		/// <summary>
		/// Stopped animation.
		/// </summary>
		public override void Stop(){
			MyOperation.StopCoroutine( GetMonoBehaviour(), ref coroutineQueue );
			for( int i = 0; i < queue.Length; i++ ){
				queue[i].animation.Stop();
			}
		}

		public override void SetGameObjectActive( bool isActive ){
			for( int i = 0; i < queue.Length; i++ ){
				queue[i].animation.SetGameObjectActive( isActive );
			}
		}

	
		/// <summary>
		/// Анимация показа очереди.
		/// </summary>
		private IEnumerator CoroutineQueueShow( bool isCycle ){
			for( int i = 0; i < queue.Length; i++ ){
				yield return new WaitForSeconds( queue[i].intervalSec );
				queue[i].animation.Show();
			}
			if( isCycle == true ){
				Cycle( true );
			}
		}

		/// <summary>
		/// Анимация скрытия очереди.
		/// </summary>
		private IEnumerator CoroutineQueueHide( bool isCycle ){
			for( int i = 0; i < queue.Length; i++ ){
				queue[i].animation.Hide( false );
				yield return new WaitForEndOfFrame();
			}
			if( isCycle == true ){
				Cycle( false );
			}
		}

	}

}