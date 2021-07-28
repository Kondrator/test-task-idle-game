using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameModel : Model {

	public class NOTIFY {

		public class COINS {

			public class CHANGED {

				public const string NAME = "game.model.coins.changed";

			}


			/// <summary>
			/// Type = int (current value)
			/// </summary>
			public const string PARAM_VALUE = "value";

			/// <summary>
			/// Type = int (added value)
			/// </summary>
			public const string PARAM_VALUE_ADDED = "value.added";

		}

	}


	[SerializeField]
	[Range( 1f, 10f )]
	private float delaySpawnMin = 1f;

	[SerializeField]
	[Range( 1f, 20f )]
	private float delaySpawnMax = 5f;





	private int coins = 0;
	public int Coins { get { return coins; } }


	public void AddCoins( int count ) {

		coins += count;

		Notify(
			NOTIFY.COINS.CHANGED.NAME,
			new NotifyData.Param( NOTIFY.COINS.PARAM_VALUE, coins ),
			new NotifyData.Param( NOTIFY.COINS.PARAM_VALUE_ADDED, count )
		);

	}



	public bool UseCoins( int count ) {

		if( coins < count ) {
			return false;
		}

		AddCoins( -count );

		return true;
	}




	public float GetDelaySpawn() {
		return Random.Range( delaySpawnMin, delaySpawnMax );
	}


}
