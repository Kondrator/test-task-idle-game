using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMoveBehaviorVisitHouse : CharacterMoveBehaviorState {

    public class NOTIFY {

        public class VISITED {

            public const string NAME = "character.move.behavior.visit.house";

        }


        /// <summary>
        /// Type = House
        /// </summary>
        public const string PARAM_HOUSE = "house";

    }




    private HouseController target = null;

    public CharacterMoveBehaviorVisitHouse( HouseController target ) :base() {
        this.target = target;
    }





    public override CharacterMoveBehaviorState Update() {

        character.Notify(
            NOTIFY.VISITED.NAME,
            new NotifyData.Param( NOTIFY.PARAM_HOUSE, target )
        );

        return new CharacterMoveBehaviorToBack();
    }


}
