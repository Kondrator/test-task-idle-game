using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMoveBehaviorToHouseRandom : CharacterMoveBehaviorState {

    private HouseController target = null;



    public CharacterMoveBehaviorToHouseRandom() :base() {
        
    }



    public override void Enable() {
        base.Enable();

        target = Element.FindAll<HouseController>().RandomItem();
        character.MoveTo( target.View.Enter.position );
    }


    public override CharacterMoveBehaviorState Update() {

        if( character.IsEndOfPath ) {
            return new CharacterMoveBehaviorVisitHouse( target );
        }

        return this;
    }


}
