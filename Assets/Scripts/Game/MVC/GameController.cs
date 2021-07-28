using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : Controller<GameModel, GameView> {

    public class NOTIFY {

        public class SPAWN {
        
            public const string NAME = "game.controller.spawn";

        }

    }




    protected override void PreInitiate() { }

    protected override void Initiate() {

        Add(
            SceneController.NOTIFY_LOADED,
            data => {
                Notify( Model.GetDelaySpawn(), NOTIFY.SPAWN.NAME );
            }
        );


        Add(
            NOTIFY.SPAWN.NAME,
            data => {
                View.Spawner.Spawn();
            }
        );


        Add(
            Spawner.NOTIFY.SPAWNED.NAME,
            data => {
                GameObject instance = data.GetParam<GameObject>( Spawner.NOTIFY.PARAM_INSTANCE );
                CharacterMoveAI character = instance?.GetComponent<CharacterMoveAI>();
                if( character != null ) {
                    character.SetBehavior( new CharacterMoveBehaviorToHouseRandom() );
                }

                Notify( Model.GetDelaySpawn(), NOTIFY.SPAWN.NAME );
            }
        );



        Add(
            CharacterMoveBehaviorVisitHouse.NOTIFY.VISITED.NAME,
            data => {
                HouseController house = data.GetParam<HouseController>( CharacterMoveBehaviorVisitHouse.NOTIFY.PARAM_HOUSE );
                Model.AddCoins( house.Model.GetReward() );
            }
        );

    }


}
