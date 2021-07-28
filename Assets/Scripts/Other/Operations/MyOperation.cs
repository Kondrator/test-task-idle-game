using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


/// <summary>
/// Выполняет различные опреации.
/// </summary>
public static class MyOperation{




	/// <summary>
	/// Convert enum to string.
	/// </summary>
	public static string ConvertEnumToString<T>( List<T> items ) where T : struct, System.IConvertible {

		if( typeof(T).IsEnum == false ){
			return "T must be an enumerated type";
		}

		string data = "";

		for( int i = 0; i < items.Count; i++ ){
			data += (int)(object)items[i];
			if( i + 1 < items.Count ){
				data += "|";
			}
		}

		return data;
	}

	/// <summary>
	/// Convert enum from string.
	/// </summary>
	public static List<T> ConvertEnumFromString<T>( string data ) where T : struct, System.IConvertible {
		
		if( typeof(T).IsEnum == false ){
			return new List<T>();
		}


		string[] array = data.Split( '|' );

		List<T> items = new List<T>();
		items.Clear();
		for( int i = 0; i < array.Length; i++ ){
			if( string.IsNullOrEmpty( array[i] ) == true ){
				continue;
			}
			items.Add( (T)(object)System.Convert.ToInt32( array[i] ) );
		}

		return items;
	}










	/// <summary>
	/// Get random item from array.
	/// </summary>
	public static T RandomItem<T>( this T[] array, int indexFrom = 0 ){
		if( array == null
			|| array.Length == 0
		){
			return default(T);
		}
		return array[Random.Range( indexFrom, array.Length )];
	}

	/// <summary>
	/// Get random item from list.
	/// </summary>
	public static T RandomItem<T>( this List<T> list, int indexFrom = 0 ) {
		if( list == null
			|| list.Count == 0
		){
			return default(T);
		}
		return list[Random.Range( indexFrom, list.Count )];
	}



	/// <summary>
	/// Get random item from array.
	/// </summary>
	public static T LastItem<T>( this T[] array ) {
		if( array == null
			|| array.Length == 0
		) {
			return default( T );
		}
		return array[array.Length - 1];
	}

	/// <summary>
	/// Get random item from list.
	/// </summary>
	public static T LastItem<T>( this List<T> list ) {
		if( list == null
			|| list.Count == 0
		) {
			return default( T );
		}
		return list[list.Count - 1];
	}





	/// <summary>
	/// Contains point in polygon ?
	/// </summary>
	public static bool IsPointInPolygon( Vector2 point, Vector2[] polygon ){
		int polygonLength = polygon.Length, i=0;
		bool inside = false;
		// x, y for tested point.
		float pointX = point.x, pointY = point.y;
		// start / end point for the current polygon segment.
		float startX, startY, endX, endY;
		Vector2 endPoint = polygon[polygonLength-1];           
		endX = endPoint.x; 
		endY = endPoint.y;
		while (i<polygonLength) {
			startX = endX;           startY = endY;
			endPoint = polygon[i++];
			endX = endPoint.x;       endY = endPoint.y;
			//
			inside ^= ( endY > pointY ^ startY > pointY ) /* ? pointY inside [startY;endY] segment ? */
					&& /* if so, test if it is under the segment */
					( (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY) ) ;
		}
		return inside;
	}




	public static bool Contains( this Rect target, Rect rect, bool isExtra = false ){
		
		// top left
		if( target.Contains( new Vector2( rect.x, rect.y ) ) == true ){
			return true;
		}
		// top right
		if( target.Contains( new Vector2( rect.x + rect.width, rect.y ) ) == true ){
			return true;
		}

		// bottom left
		if( target.Contains( new Vector2( rect.x, rect.y + rect.height ) ) == true ){
			return true;
		}
		// bottom right
		if( target.Contains( new Vector2( rect.x + rect.width, rect.y + rect.height ) ) == true ){
			return true;
		}

		if( isExtra == true ){

			// left middle
			if( target.Contains( new Vector2( rect.x, rect.y + rect.height / 2 ) ) == true ){
				return true;
			}
			// top middle
			if( target.Contains( new Vector2( rect.x + rect.width / 2, rect.y ) ) == true ){
				return true;
			}
			// right middle
			if( target.Contains( new Vector2( rect.x + rect.width, rect.y + rect.height / 2 ) ) == true ){
				return true;
			}
			// bottom middles
			if( target.Contains( new Vector2( rect.x + rect.width / 2, rect.y + rect.height ) ) == true ){
				return true;
			}
			// center
			if( target.Contains( new Vector2( rect.x + rect.width / 2, rect.y + rect.height / 2 ) ) == true ){
				return true;
			}

		}

		return false;

	}
	
	public static string ToStringWithMark( this int number ){
		if( number == 0 ){
			return "0";
		}
		if( number < 0 ){
			return number.ToString();
		}
		return "+" + number;
	}

	/// <summary>
	/// Splite "ExampleWordText" to "Example World Text".
	/// </summary>
	public static string ToStringSplitWords( this string words, string symbol = " " ){
		
		string result = "";

		for( int i = 0; i < words.Length; i++ ){
			if( i > 0
				&&	(	char.IsUpper( words[i] ) == true
						|| char.IsNumber( words[i] ) == true
					)
			){
				result += symbol;
			}
			result += words[i];
		}

		return result;
	}





	public static float GetDistanceIgnoreY( Vector3 from, Vector3 to ){
		from.y = to.y = 0;
		return Vector3.Distance( from, to );
	}
	public static float GetDistanceIgnoreY( Transform from, Transform to ){
		return GetDistanceIgnoreY( from.position, to.position );
	}





	public static int LayerMaskToLayer( this LayerMask layerMask ){
		int layerNumber = 0;
		int layer = layerMask.value;
		while(layer > 0) {
			layer = layer >> 1;
			layerNumber++;
		}
		return layerNumber - 1;
	}


	/// <summary>
	/// Convert seconds to string formar: hh:mm:ss.
	/// </summary>
	public static string SecondsToStringTime( this int secondsAll, bool isAutoHideZerosHour = true, bool isAutoHideZerosMinute = true ){

		bool isPositive = secondsAll >= 0;

		secondsAll = secondsAll >= 0 ? secondsAll : secondsAll * -1;

		long seconds = secondsAll % 60;
		long minutes = secondsAll / 60 % 60;
		long hours = secondsAll / 60 / 60;



		System.Text.StringBuilder builder = new System.Text.StringBuilder();

		builder.Append( seconds < 10 ? "0" + seconds : seconds.ToString() );
		
		if( hours > 0
			|| minutes > 0
			|| isAutoHideZerosMinute == false
		) {
			builder.Insert( 0, STRING_TIME_COLON );
			builder.Insert( 0, minutes < 10 ? "0" + minutes : minutes.ToString() );
		}

		if( hours > 0
			|| isAutoHideZerosHour == false
		){
			builder.Insert( 0, STRING_TIME_COLON );
			builder.Insert( 0, hours < 10 ? "0" + hours : hours.ToString() );
		}

		builder.Append( isPositive == true ? string.Empty : "-" );

		return builder.ToString();
	}
	/// <summary>
	/// Convert seconds to string formar: hh:mm:ss.
	/// </summary>
	public static string SecondsToStringTime( this long secondsAll, bool isAutoHideZeros = true ){
		return ((int)secondsAll).SecondsToStringTime();;
	}

	private static string STRING_TIME_COLON = ":";







	/// <summary>
	/// Execute operation after frame.
	/// </summary>
	/// <param name="monoBehaviour">For run coroutine wait.</param>
	/// <param name="execute">Operation.</param>
	/// <param name="countUpdates">Extra wait after frame.</param>
	public static void ExecuteAtNextUpdate( MonoBehaviour monoBehaviour, System.Action execute, float extraSeconds = 0 ){
		if( monoBehaviour != null
			//&& monoBehaviour.isActiveAndEnabled == true 
		){
			monoBehaviour.StartCoroutine( CoroutineExecuteAtNextUpdate( execute, extraSeconds ) );
		}
	}
	public static IEnumerator CoroutineExecuteAtNextUpdate( System.Action execute, float extraSeconds ){
		
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds( extraSeconds );

		if( execute != null ){
			execute();
		}
	}





	/// <summary>
	/// Fix text to float/double.
	/// </summary>
	public static string GetStringFixToFloat( string text ){

		bool isDot = false;

		string result = "";

		for( int i = 0; i < text.Length; i++ ){

			if( text[i] >= '0' && text[i] <= '9' ){
				result += text[i];

			}else if( text[i] == '.' || text[i] == ',' ){
				if( isDot == false && result.Length > 0 ){
					isDot = true;
					result += '.';
				}
			}
			
		}

		return result;

	}

	/// <summary>
	/// Fix text to int.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="empty">Return this string if result length == 0.</param>
	public static string GetStringFixToInt( string text, string empty = "0" ){

		string result = "";

		for( int i = 0; i < text.Length; i++ ){
			if( text[i] >= '0' && text[i] <= '9' ){
				result += text[i];
			}
		}

		if( result.Length == 0 ){
			result = empty;
		}

		return result;

	}
	/// <summary>
	/// Get only numbers from text.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="empty">Return this string if result length == 0.</param>
	public static int GetIntFix( string text, string empty = "0" ){
		return System.Convert.ToInt32( GetStringFixToInt( text, empty ) );
	}

	/// <summary>
	/// Fix text to (RU|ENG) and numbers.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="isSpace">Check spaces? True - stay spaces.</param>
	public static string TextToString( string text, bool isSpace = true ){

		string result = "";
		string space = isSpace ? " " : "";

		foreach( Match match in Regex.Matches( text, "[A-ZА-я0-9" + space + "]+", RegexOptions.IgnoreCase ) ){
			result += match.Value;
		}

		return result;

	}


	/// <summary>
	/// Example: 12500 -> 12 500.
	/// </summary>
	/// <param name="number">Number.</param>
	public static string ToStringSplitSymbol( this long number, string symbol = " " ){
		
		string numberStr = number.ToString();

		System.Text.StringBuilder builder = new System.Text.StringBuilder();

		if( numberStr.Length <= 3 ){
			return numberStr;
		}

		for( int i = numberStr.Length - 1, iSpace = 0;		i >= 0;		i--, iSpace++ ){

			if( iSpace % 3 == 0 ) {
				builder.Insert( 0, symbol );
			}

			builder.Insert( 0, numberStr[i] );

		}

		return builder.ToString();

	}
	/// <summary>
	/// Example: 12500 -> 12 500.
	/// </summary>
	/// <param name="number">Number.</param>
	public static string ToStringSplitSymbol( this int number, string symbol = " " ){
		return ToStringSplitSymbol( (long)number, symbol );
	}
	/// <summary>
	/// Float to string with need count numbers after dot.
	/// </summary>
	/// <param name="number">Nubmer.</param>
	/// <param name="numbersAfterDot">Count numbers after dot.</param>
	public static string FloatToString( float number, int numbersAfterDot = 2 ){
		
		string numberStr = number.ToString().Replace( ',', '.' );

		int iDot = numberStr.Length;

		for( int i = 0;	i < numberStr.Length; i++ ){

			if( numberStr[i] == '.' ){
				iDot = i;
			}

			if( i - iDot == numbersAfterDot ){
				return numberStr.Substring( 0, i + 1 );
			}

		}

		return numberStr;

	}
	/// <summary>
	/// Float to string with need count numbers after dot.
	/// </summary>
	/// <param name="number">Nubmer.</param>
	/// <param name="numbersAfterDot">Count numbers after dot.</param>
	public static string ToString( this float number, int numbersAfterDot = 2 ){
		return FloatToString( number, numbersAfterDot );
	}

	/// <summary>
	/// Преобразование float в строку с указанием количества цифр после запятой.
	/// </summary>
	/// <param name="number">Число.</param>
	/// <param name="numbersAfterDot">Количество цифр после запятой.</param>
	public static string ConvertToString( float number, int numbersAfterDot = 2 ){
		
		if( Mathf.Abs( number ) * (10 * numbersAfterDot) < 1 ){
			number = 0;
		}

		string numberStr = number.ToString().Replace( ',', '.' );

		int iDot = numberStr.Length;

		for( int i = 0;	i < numberStr.Length; i++ ){

			if( numberStr[i] == '.' ){
				iDot = i;
			}

			if( i - iDot == numbersAfterDot ){
				return numberStr.Substring( 0, i + 1 );
			}

		}

		return numberStr;

	}
	/// <summary>
	/// Преобразование float в строку с указанием количества цифр после запятой.
	/// </summary>
	/// <param name="number">Число.</param>
	/// <param name="numbersAfterDot">Количество цифр после запятой.</param>
	public static string ToStringMini( this float number, int numbersAfterDot = 2 ){
		string result = ConvertToString( number, numbersAfterDot );
		
		if( result.Contains( "." ) ){
			//result = result.TrimEnd( '0' ).TrimEnd( '.' );
		}

		return result;
	}

	public static string ToStringMini( this bool value ){
		return value == true ? "1" : "0";
	}



	#region Extended

	/// <summary>
	/// Return sub array.
	/// </summary>
	/// <typeparam name="T">Type item array.</typeparam>
	/// <param name="data">Source array.</param>
	/// <param name="index">Index, from get array.</param>
	/// <param name="length">Length array from 'index'.</param>
	public static T[] SubArray<T>( this T[] data, int index, int length ) {
		index = Mathf.Min( index, data.Length - 1 );
		length = Mathf.Min( length, data.Length );

		T[] result = new T[length];
		System.Array.Copy( data, index, result, 0, length );
		return result;
	}

	#endregion




	/// <summary>
	/// Возвращает нужное склонение по номеру.
	/// Номеры склонений: Например ячейка: 0 - ячейка, 1 - ячейки, 2 - ячеек.
	/// </summary>
	/// <param name="number">Число.</param>
	public static int GetDeclension( int number ){
		number = Mathf.Abs( number );

		if( number % 10 == 1 ) return 0;

		if( number >= 10 && number <= 20 ) return 1;

		if( number % 10 >= 2 && number % 10 <= 4 ) return 2;

		return 1;
	}
	/// <summary>
	/// Возвращает нужную строку по определенному склонению.
	/// Номеры склонений: Например ячейка: 0 - одна, 1 - много, 2 - две.
	/// </summary>
	public static string GetDeclension( int number, params string[] variants ){
		
		int declension = GetDeclension( number );

		if( variants.Length >= 0 && declension < variants.Length ) return variants[declension];
		
		return declension.ToString();
	}
	








	/// <summary>
	/// Copy Transform lcoal.
	/// </summary>
	/// <param name="target">Copy to this.</param>
	/// <param name="source">Copy from.</param>
	public static void FixTransformLocal( Transform target, Transform source ){

		if( source == null ) return;

		target.localPosition = source.localPosition;
		target.localRotation = source.localRotation;
		target.localScale = source.localScale;

	}
	
	/// <summary>
	/// Copy Transform.
	/// </summary>
	/// <param name="target">Copy to this.</param>
	/// <param name="source">Copy from.</param>
	public static void FixTransform( Transform target, Transform source ){

		if( source == null ) return;

		target.position = source.position;
		target.rotation = source.rotation;
		target.localScale = source.localScale;

	}

	/// <summary>
	/// Reset Transform. local/position = Vector3.zero. local/rotation = Vector3.zero. localScale = need value.
	/// </summary>
	/// <param name="target">Transform, reset this.</param>
	public static void ResetTransform( this Transform target, Vector3 scale ){
		
		target.position = Vector3.zero;
		target.localPosition = Vector3.zero;

		target.rotation = Quaternion.Euler( Vector3.zero );
		target.localRotation = Quaternion.Euler( Vector3.zero );

		target.localScale = scale;
		
	}

	/// <summary>
	/// Reset Transform. local/position = Vector3.zero. local/rotation = Vector3.zero. localScale = Vector3.one.
	/// </summary>
	/// <param name="target">Transform, reset this.</param>
	public static void ResetTransform( this Transform target ){
		ResetTransform( target, Vector3.one );
	}
	







#region Destroy All Child

	/// <summary>
	/// Destroy all childs.
	/// </summary>
	/// <param name="gameObject">Parent.</param>
	public static void DestroyAllChild( GameObject gameObject, bool isDestroyImmediate = false ){

		DestroyAllChild( gameObject.transform, isDestroyImmediate );

	}
	/// <summary>
	/// Destroy all childs.
	/// </summary>
	/// <param name="monoBehaviour">Parent.</param>
	public static void DestroyAllChild( MonoBehaviour monoBehaviour, bool isDestroyImmediate = false ){

		DestroyAllChild( monoBehaviour.transform, isDestroyImmediate );

	}
	/// <summary>
	/// Destroy all childs.
	/// </summary>
	/// <param name="transform">Parent.</param>
	public static void DestroyAllChild( Transform transform, bool isDestroyImmediate = false ){

		DestroyAllChild( transform, 0, int.MaxValue, isDestroyImmediate );

	}
	/// <summary>
	/// Destroy all childs in range.
	/// </summary>
	/// <param name="transform">Parent.</param>
	/// <param name="from">Index from. Inclusive!</param>
	/// <param name="to">Index to. Inclusive!</param>
	public static void DestroyAllChild( Transform transform, int from, int to = int.MaxValue, bool isDestroyImmediate = false ){

		if( transform == null ) return;


		// count childs
		int count = transform.childCount;

		// get childs
		GameObject[] childs = new GameObject[count];
		for( int i = from; i < count && i <= to; i++ ){
			childs[i] = transform.GetChild( i ).gameObject;
		}

		// destroy childs
		for( int i = 0; i < childs.Length; i++ ){

			if( childs[i] == null ){
				continue;
			}

			if( isDestroyImmediate == true ){
				Object.DestroyImmediate( childs[i] );

			}else{
				Object.Destroy( childs[i] );
			}

		}

	}

	/// <summary>
	/// Deactivate childs.
	/// </summary>
	/// <param name="transform">Parent.</param>
	/// <param name="from">Index from. Inclusive!</param>
	/// <param name="to">Index to. Inclusive!</param>
	/// <param name="isUnparent">Unpurent childs?</param>
	public static void DeactivateAllChild( this Transform transform, int from, int to = int.MaxValue, bool isUnparent = true ){

		if( transform == null ) return;


		// count childs
		int count = transform.childCount;

		// get childs
		GameObject[] childs = new GameObject[count];
		for( int i = from; i < count && i <= to; i++ ){
			childs[i] = transform.GetChild( i ).gameObject;
		}

		// desctivate
		for( int i = 0; i < childs.Length; i++ ){

			if( childs[i] == null ){
				continue;
			}

			childs[i].Deactivate( isUnparent );

		}

	}
	/// <summary>
	/// Deactivate childs.
	/// </summary>
	/// <param name="transform">Parent.</param>
	/// <param name="isUnparent">Unpurent childs?</param>
	public static void DeactivateAllChild( this Transform transform, bool isUnparent = true ){

		DeactivateAllChild( transform, 0, int.MaxValue, isUnparent );

	}
	

	
	/// <summary>
	/// Set active TRUE.
	/// </summary>
	public static void Activate( this GameObject target ){
		target.SetActive( true );
	}

	/// <summary>
	/// Set active FALSE and set parent NULL if need this.
	/// </summary>
	/// <param name="isUnparent">True - set parent NULL.</param>
	public static void Deactivate( this GameObject target, bool isUnparent = true ){
		if( isUnparent == true ){
			if( target.transform is RectTransform ){
				target.transform.SetParent( PoolGameObject.TransformForUI );

			}else{
				target.transform.SetParent( PoolGameObject.TransformForOther );
			}
		}
		target.SetActive( false );
	}
	/// <summary>
	/// Set active FALSE and set parent NULL if need this.
	/// </summary>
	/// <param name="isUnparent">True - set parent NULL.</param>
	public static void Deactivate( this MonoBehaviour target, bool isUnparent = true ){
		if( target != null ){
			target.gameObject.Deactivate( isUnparent );
		}
	}

	/// <summary>
	/// Wait time and:
	/// Set active FALSE and set parent NULL if need this.
	/// </summary>
	/// <param name="isUnparent">True - set parent NULL.</param>
	public static void Deactivate( this GameObject goTarget, MonoBehaviour target, float time, bool isUnparent = true ){
		if( target != null ){
			target.StartCoroutine( CoroutineDeactivate( target, time, isUnparent ) );
		}
	}
	private static IEnumerator CoroutineDeactivate( MonoBehaviour target, float time, bool isUnparent ){
		yield return new WaitForSeconds( time );
		if( target != null ){
			target.Deactivate( isUnparent );
		}
	}



	/// <summary>
	/// 1) Set active FALSE.
	/// 2) Set active TRUE.
	/// </summary>
	public static void ActivateForcibly( this GameObject target ){
		target.SetActive( false );
		target.SetActive( true );
	}

	public static void SetActive( this Transform target, bool isActive ){
		if( target != null ){
			target.gameObject.SetActive( isActive );
		}
	}

#endregion




	/// <summary>
	/// Destroy scripts (MonoBehaviour) on target.
	/// </summary>
	/// <param name="gameObject">Taret.</param>
	/// <param name="isChildren">True - destroy scripts in childs. False - destroy scripts only target.</param>
	public static void RemoveScripts( GameObject gameObject, bool isChildren = false ){

		if( gameObject == null ) return;

		// get
		MonoBehaviour[] scripts = new MonoBehaviour[0];
		if( isChildren == true ) scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
		else scripts = gameObject.GetComponents<MonoBehaviour>();

		// destroy
		for( int i = 0; i < scripts.Length; i++ ){

			GameObject.Destroy( scripts[i] );

		}

	}

	




	/// <summary>
	/// Stop coroutine.
	/// </summary>
	/// <param name="isSetNULL">Set NULL to coroutine?</param>
	public static void StopCoroutine( MonoBehaviour monoBehaviour, ref IEnumerator coroutine, bool isSetNULL = true ){

		if( monoBehaviour != null && coroutine != null ){
			monoBehaviour.StopCoroutine( coroutine );
		}

		if( isSetNULL == true ){
			coroutine = null;
		}

	}





	#region Instantiate

	/// <summary>
	/// Create GameObject.
	/// </summary>
	/// <param name="goSource">Need create this.</param>
	/// <param name="parent">Set parent for new GameObject.</param>
	public static GameObject Instantiate( GameObject goSource, Transform parent = null ){

		// fix
		if( goSource == null ) return null;


		// create
		GameObject goNew = GameObject.Instantiate( goSource ) as GameObject;

		if( parent != null ){
			// fix
			Vector3 scale = goNew.transform.localScale;
			// set parent
			goNew.transform.SetParent( parent );
			// reset transform
			ResetTransform( goNew.transform, scale );
		}

		return goNew;
	}
	/// <summary>
	/// Create GameObject.
	/// </summary>
	/// <param name="goSource">Need create this.</param>
	/// <param name="parent">Set parent for new GameObject.</param>
	/// <typeparam name="T">Get component from new GameObject.</typeparam>
	public static T Instantiate<T>( GameObject goSource, Transform parent = null ) where T : Component {

		// вернем объект
		return (T)(Object)Instantiate( goSource, parent ).GetComponent( typeof( T ) );

	}
	/// <summary>
	/// Create GameObject in Canvas with tag "UICanvasMain".
	/// Or first finded Canvas.
	/// </summary>
	/// <param name="goSource">Need create this.</param>
	/// <typeparam name="T">Get component from new GameObject.</typeparam>
	public static T InstantiateInCanvas<T>( GameObject target, bool isFixOffsetToZero = false, bool isFixScaleToOne = false ) where T : Component {
		
		Transform parent = null;
		// find canvas
		Canvas canvas = MyOperationUI.FindCanvas();
		if( canvas != null ) parent = canvas.transform;

		return InstantiateInCanvas<T>( target, parent, isFixOffsetToZero, isFixScaleToOne );
	}
	/// <summary>
	/// Create GameObject in need Canvas.
	/// </summary>
	/// <param name="goSource">Need create this.</param>
	/// <typeparam name="T">Get component from new GameObject.</typeparam>
	public static T InstantiateInCanvas<T>( GameObject target, Transform parent, bool isFixOffsetToZero = false, bool isFixScaleToOne = false ) where T : Component {
		
		GameObject goNew = Instantiate( target, parent );

		if( goNew != null ){
			if( isFixOffsetToZero == true ){
				goNew.FixOffsetAll();
			}
			if( isFixScaleToOne == true ){
				goNew.transform.localScale = Vector3.one;
			}
		}

		return goNew.GetComponent<T>();
	}
	/// <summary>
	/// Create GameObject.
	/// </summary>
	/// <param name="objSource">Need create this.</param>
	/// <param name="parent">Set parent for new GameObject.</param>
	/// <typeparam name="T">Get component from new GameObject.</typeparam>
	public static T InstantiateClone<T>( GameObject goSource, Transform parent = null ) where T : Object {

		// create
		GameObject goInstantiate = Instantiate( goSource, parent );
		
		// copy Transform
		FixTransform( goInstantiate.transform, goSource.transform );
		FixTransformLocal( goInstantiate.transform, goSource.transform );

		RectTransform rectSource = goSource.GetComponent<RectTransform>();
		if( rectSource != null ){
			RectTransform rectNew = goInstantiate.GetComponent<RectTransform>();
			if( rectNew != null ){
				rectNew.CopyFrom( rectSource );
			}
		}

		return goInstantiate.GetComponent<T>();
	}
	
	#endregion




	/// <summary>
	/// Get component at target.
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	/// <param name="target">Target.</param>
	/// <param name="isInherit">True - consider inherit. False - strong check.</param>
	public static T GetComponent<T>( GameObject target, bool isInherit = false ) where T : Component{

		T[] components = target.GetComponents<T>();

		for( int i = 0; i < components.Length; i++ ){
			if( isInherit == true ){
				if( components[i] is T ){
					return components[i];
				}

			}else{
				if( components[i].GetType() == typeof(T) ){
					return components[i];
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Add component to target and return her.
	/// If component allready have - return her (without created).
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	/// <param name="target">Target.</param>
	/// <param name="isInherit">True - consider inherit. False - strong check.</param>
	public static T AddComponent<T>( GameObject target, bool isInherit = false ) where T : Component{
		T component = GetComponent<T>( target, isInherit );
		if( component != null ) return component;

		return target.AddComponent<T>();
	}
	/// <summary>
	/// Add component to target and return her.
	/// If component allready have - return her (without created).
	/// </summary>
	public static T AddComponent<T>( this Component target, bool isInherit = false ) where T : Component{
		if( target == null ){
			return null;
		}
		return AddComponent<T>( target.gameObject, isInherit );
	}

	/// <summary>
	/// Check has component
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	public static bool HasComponent<T>( this GameObject target ) where T : Component {
		return target.GetComponent<T>() != null;
	}
	/// <summary>
	/// Check has component
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	public static bool HasComponent<T>( this Component target ) where T : Component {
		return target.gameObject.HasComponent<T>();
	}


	/// <summary>
	/// Remove component at taret.
	/// </summary>
	/// <typeparam name="T">Type remove component.</typeparam>
	/// <param name="target">Target.</param>
	/// <param name="isInherit">True - consider inherit. False - strong check.</param>
	/// <returns>True - remove success. False - not have component.</returns>
	public static bool RemoveComponent<T>( GameObject target, bool isInherit = false, bool isDestroyImmediate = false ) where T : Component{
		T component = GetComponent<T>( target, isInherit );
		if( component == null ) return false;

		if( isDestroyImmediate == true ){
			GameObject.DestroyImmediate( component );
		
		}else{
			GameObject.Destroy( component );
		}

		return true;
	}

	/// <summary>
	/// Have component at target?
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	/// <param name="target">Target.</param>
	/// <param name="isInherit">True - consider inherit. False - strong check.</param>
	public static bool IsHaveComponent<T>( GameObject target, bool isInherit = true ) where T : Component{
		T component = GetComponent<T>( target, isInherit );
		if( component == null ) return false;

		return true;
	}


	/// <summary>
	/// Enable / Disable need components at target.
	/// </summary>
	/// <param name="gameObject">Target..</param>
	/// <param name="isEnabled">Enable / Disable.</param>
	/// <param name="isChildren">Enable / Diable childs?</param>
	/// <typeparam name="T">Type component.</typeparam>
	public static void SetComponentsEnabled<T>( this GameObject gameObject, bool isEnabled, bool isChildren = false ) where T : Object{

		T[] components = new T[0];
		if( isChildren == true ) components = gameObject.GetComponentsInChildren<T>();
		else components = gameObject.GetComponents<T>();
		


		for( int i = 0; i < components.Length; i++ ){

			Behaviour behaviour = components[i] as Behaviour;
			if( behaviour != null ){
				behaviour.enabled = isEnabled;
				continue;
			}

			Renderer renderer = components[i] as Renderer;
			if( renderer != null ){
				renderer.enabled = isEnabled;
				continue;
			}

			Collider collider = components[i] as Collider;
			if( collider != null ){
				collider.enabled = isEnabled;
				continue;
			}
			
		}

	}




	/// <summary>
	/// Get component in parents from target.
	/// </summary>
	/// <param name="goSource">Target.</param>
	/// <param name="checkSelf">Check component at self?</param>
	/// <typeparam name="T">Type component.</typeparam>
	public static T GetComponentInParent<T>( this GameObject goSource, bool checkSelf = false ) where T : Component{

		if( checkSelf == true ){
			T component = goSource.GetComponent<T>();
			if( component != null ){
				return component;
			}
		}

		Transform parent = goSource.transform;
		while( ( parent = parent.transform.parent ) != null ){

			T component = parent.GetComponent<T>();

			if( component != null ){

				return component;

			}

		}

		return null;

	}


	/// <summary>
	/// Get childs at target.
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	/// <param name="transform">Parent.</param>
	/// <param name="isDeactivateNotTemplate">Deactivate childs not templated?</param>
	public static T[] GetComponentsInChildren<T>( Transform transform, bool isDeactivateNotTemplate = true ) where T : Component{
		if( transform == null ) return new T[0];

		// count childs
		int count = transform.childCount;

		// get childs
		Transform[] childs = new Transform[count];
		for( int i = 0; i < count; i++ ){
			childs[i] = transform.GetChild( i );
		}

		// return components
		List<T> components = new List<T>();
		for( int i = 0; i < count; i++ ){

			T component = childs[i].GetComponent<T>();

			if( component == null && isDeactivateNotTemplate == true ){
				childs[i].gameObject.Deactivate();

			}else{
				components.Add( component );
			}

		}

		return components.ToArray();
	}





	public static List<T> FindObjectsOfType<T>( bool isInactive = false ) where T : class{
		
		List<T> components = new List<T>();

		GameObject[] monos = null;
		if( isInactive == false ){
			monos = GameObject.FindObjectsOfType<GameObject>();

		}else{
			monos = GetAllObjectsInScene();
		}

		for( int i = 0; i < monos.Length; i++ ){
			T component = monos[i].GetComponent<T>();
			if( component != null ){
				components.Add( component );
			}
		}

		return components;
	}
	private static GameObject[] GetAllObjectsInScene(){

		List<GameObject> objectsInScene = new List<GameObject>();

		GameObject[] gosFinded = Resources.FindObjectsOfTypeAll<GameObject>();
		for( int i = 0; i < gosFinded.Length; i++ ){
			
			if( gosFinded[i].hideFlags == HideFlags.NotEditable
				|| gosFinded[i].hideFlags == HideFlags.HideAndDontSave
				|| gosFinded[i].activeInHierarchy == false
			){
				continue;
			}

			objectsInScene.Add( gosFinded[i] );
		}

		return objectsInScene.ToArray();
	}





	/// <summary>
	/// Get ange between two points.
	/// </summary>
	/// <param name="point1">Point 1.</param>
	/// <param name="point2">Point 2.</param>
	public static float GetAngle( Vector2 point1, Vector2 point2 ){

		float angle = Mathf.Atan2( point1.y - point2.y, point1.x - point2.x ) / Mathf.PI * 180;
		
		if( angle < 0 ){
			angle += 360;
		}

		return angle;
	}

	/// <summary>
	/// Get coordinations triangle in middle beetwen two points.
	/// </summary>
	/// <param name="point1">Point 1.</param>
	/// <param name="point2">Point 2.</param>
	public static Vector3[] GetTriangle( Vector2 point1, Vector2 point2, float height = 10, float width = 10 ){

		Vector3[] triangle = new Vector3[3];

		// middle beetwen points
		Vector2 posMiddle = point1 + (point2 - point1) / 2;

		// angle beetwen points
		float angle = MyOperation.GetAngle( point1, point2 );

		// left point
		float x = height * Mathf.Cos( (angle + 90) * 3.14f / 180f ); 
		float y = height * Mathf.Sin( (angle + 90) * 3.14f / 180f );
		triangle[0] = new Vector3( posMiddle.x + x, posMiddle.y + y );

		// right point
		x = height * Mathf.Cos( (angle - 90) * 3.14f / 180f ); 
		y = height * Mathf.Sin( (angle - 90) * 3.14f / 180f );
		triangle[1] = new Vector3( posMiddle.x + x, posMiddle.y + y );

		// top point
		x = width * Mathf.Cos( angle * 3.14f / 180f ); 
		y = width * Mathf.Sin( angle * 3.14f / 180f );
		triangle[2] = new Vector3( posMiddle.x + x, posMiddle.y + y );

		return triangle;
	}

	






	/// <summary>
	/// Get all colliders under cursor.
	/// </summary>
	/// <typeparam name="T">Need get type in raycast hits.</typeparam>
	public static List<T> GetRaycasts<T>() where T : Component{
		
		Camera camera = Camera.main;
		if( camera == null ){
			return new List<T>();
		}

		List<T> objs = new List<T>();

		Ray ray = camera.ScreenPointToRay( Input.mousePosition );
		RaycastHit[] hits = Physics.RaycastAll( ray );

		for( int i = 0; i < hits.Length; i++ ){
			T component = hits[i].collider.gameObject.GetComponent<T>();
			if( component != null ){
				objs.Add( component );
			}
		}

		return objs;
	}
	public static T GetRaycastHit<T>( string tag ) where T : Component{
		
		List<T> hits = GetRaycasts<T>();

		for( int i = 0; i < hits.Count; i++ ){
			if( hits[i].tag == tag ){
				return hits[i];
			}
		}

		return null;
	}
	public static T GetFirstRaycastHit<T>() where T : Component{
		
		List<T> hits = GetRaycasts<T>();

		if( hits.Count > 0 ){
			return hits[0];
		}

		return null;
	}
	/// <summary>
	/// Ray is hitting to GameObject with need tag.
	/// </summary>
	/// <param name="tag">Need tag.</param>
	public static bool IsRaycastHit( string tag ){
		
		if( Camera.main == null ){
			return false;
		}

		RaycastHit[] hits = Physics.RaycastAll( Camera.main.ScreenPointToRay( Input.mousePosition ) );

		for( int i = 0; i < hits.Length; i++ ){
			if( hits[i].collider.tag == tag ){
				return true;
			}
		}

		return false;
	}




	/// <summary>
	/// Use Application.Quit() and stop play in editor.
	/// </summary>
	public static void Quit(){
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		UnityEditor.EditorApplication.isPaused = false;
#endif
	}


}
