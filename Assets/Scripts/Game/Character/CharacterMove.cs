using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent( typeof(NavMeshAgent) )]
public class CharacterMove : Element {


	private NavMeshAgent agent = null;
	private NavMeshAgent Agent { get { return agent ??= GetComponent<NavMeshAgent>(); } }


	public bool IsEndOfPath {
		get {
			if( !Agent.pathPending ) {
				if( Agent.remainingDistance <= Agent.stoppingDistance ) {
					if( !Agent.hasPath || Agent.velocity.sqrMagnitude == 0f ) {
						return true;
					}
				}
			}

			return false;
		}
	}



	public void MoveTo( Vector3 position ) {
		Agent.SetDestination( position );
	}


}
