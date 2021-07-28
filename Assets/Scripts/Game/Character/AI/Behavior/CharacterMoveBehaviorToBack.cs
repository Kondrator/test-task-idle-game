using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMoveBehaviorToBack : CharacterMoveBehaviorState {


    public CharacterMoveBehaviorToBack() :base() {
        
    }



    public override void Enable() {
        base.Enable();

        Spawner spawner = Element.Find<Spawner>();
        character.MoveTo( spawner.transform.position );
    }


    public override CharacterMoveBehaviorState Update() {

        if( character.IsEndOfPath ) {
            character.Deactivate();
            return null;
        }

        return this;
    }


}
