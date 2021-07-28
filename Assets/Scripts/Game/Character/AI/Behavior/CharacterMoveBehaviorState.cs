using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CharacterMoveBehaviorState {


	protected CharacterMove character = null;


	public CharacterMoveBehaviorState() {
		
	}


	public void SetCharacter( CharacterMove character ) {
		this.character = character;
	}



	public virtual void Enable() { }

	public abstract CharacterMoveBehaviorState Update();

	public virtual void Disable() { }


}
