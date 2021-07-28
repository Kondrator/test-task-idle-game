using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class AnimationCurveExtensions{

	/// <summary>
	/// Get value in range at need time, in curve.
	/// </summary>
	/// <param name="time">Current time.</param>
	/// <param name="from">Range from.</param>
	/// <param name="to">Range to.</param>
    public static float Evaluate( this AnimationCurve curve, float time, float from, float to ){
        return from + (to - from) * curve.Evaluate( time );
    }
    public static Vector2 Evaluate( this AnimationCurve curve, float time, Vector2 from, Vector2 to ){
		
		Vector2 result = Vector2.zero;
		result.x = curve.Evaluate( time, from.x, to.x );
		result.y = curve.Evaluate( time, from.y, to.y );

        return result;
    }
    public static Vector3 Evaluate( this AnimationCurve curve, float time, Vector3 from, Vector3 to ){
		
		Vector3 result = Vector3.zero;
		result.x = curve.Evaluate( time, from.x, to.x );
		result.y = curve.Evaluate( time, from.y, to.y );
		result.z = curve.Evaluate( time, from.z, to.z );

        return result;
    }

	public static AnimationCurve GetCurveSimple(){
		return new AnimationCurve( new Keyframe[]{ new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) } );
	}

	public static AnimationCurve GetCurve( float start, float end ){
		return new AnimationCurve( new Keyframe[]{ new Keyframe( 0, start ), new Keyframe( 1, end ) } );
	}

	public static AnimationCurve GetCurveDamageTake(){
		return new AnimationCurve( new Keyframe[]{ 
											new Keyframe( 0, 1 ),
											new Keyframe( 0, 0.5f ),
											new Keyframe( 0, 1 ),
											new Keyframe( 0, 0.5f ),
											new Keyframe( 0, 1 )
										} );
	}


	public static AnimationCurve GetCurveRandom( int countFrames, float rangeValueFrom, float rangeValueTo, bool isFirstValueZero = true, bool isLastValueZero = true ){

		Keyframe[] keyFrames = new Keyframe[countFrames + 1];
		for( int i = 0; i < keyFrames.Length; i++ ){
			if( i == 0 
				&& isFirstValueZero == true
			){
				keyFrames[i] = new Keyframe( 0, 0, 0, 0, 0, 0 );

			}else if(	i + 1 == keyFrames.Length
						&& isLastValueZero == true
			){
				keyFrames[i] = new Keyframe( 1f, 0, 0, 0, 0, 0 );

			}else{
				keyFrames[i] = new Keyframe( (float)i / (float)keyFrames.Length, Random.Range( rangeValueFrom, rangeValueTo ), 0, 0, 0, 0 );
			}
		}

		return new AnimationCurve( keyFrames );
	}

}