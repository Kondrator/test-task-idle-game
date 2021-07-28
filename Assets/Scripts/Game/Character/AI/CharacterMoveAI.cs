using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof(CharacterMove) )]
public class CharacterMoveAI : MonoBehaviour, IMyManagerMonoBehaviour {


	private CharacterMove character = null;
	private CharacterMove Character { get { return character ??= GetComponent<CharacterMove>(); } }


	private CharacterMoveBehaviorState behavior = null;




	private void OnEnable() {
		MyManagerMonoBehaviour.Add( this );
	}

	private void OnDisable() {
		MyManagerMonoBehaviour.Remove( this );
	}




	public void SetBehavior( CharacterMoveBehaviorState state ) {

		behavior?.Disable();
		behavior?.SetCharacter( null );

		behavior = state;

		behavior?.SetCharacter( Character );
		behavior?.Enable();

	}



	public void UpdateMe( float timeDelta, MyManagerMonoBehaviourType type ) {

		if( this.behavior == null ) {
			return;
		}

		CharacterMoveBehaviorState behavior = this.behavior.Update();

		if( behavior != this.behavior ) {
			SetBehavior( behavior );
		}
	}

}
