using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;

public static class MyOperationUI {


	/// <summary>
	/// Setting InputField for enter only float / double.
	/// </summary>
	public static void FixInputForFloat( InputField inputField ){


		if( inputField == null ){
			return;

		}


		inputField.onValueChanged.RemoveAllListeners();
		inputField.onValueChanged.AddListener( delegate {

			inputField.text = MyOperation.GetStringFixToFloat( inputField.text );


			if( inputField.text.Length == 0 ){
				inputField.text = "0";
			}

		} );

		// fix
		inputField.onValueChanged.Invoke( "" );

	}
	/// <summary>
	/// Setting InputField for enter only integer.
	/// </summary>
	public static void FixInputForInt( InputField inputField ){


		if( inputField == null ){
			return;

		}


		inputField.onValueChanged.RemoveAllListeners();
		inputField.onValueChanged.AddListener( delegate {

			inputField.text = MyOperation.GetStringFixToInt( inputField.text );


			if( inputField.text.Length == 0 ){
				inputField.text = "0";
			}

		} );

		// fix
		inputField.onValueChanged.Invoke( "" );

	}
	/// <summary>
	/// Setting InputField for input only (RU|EN) and numbers.
	/// </summary>
	/// <param name="isSpace">Check spaces? True - stay spaces.</param>
	public static void FixInputForText( InputField inputField, bool isSpace = true ){


		if( inputField == null ){
			return;

		}


		inputField.onValueChanged.RemoveAllListeners();
		inputField.onValueChanged.AddListener( delegate {

			inputField.text = MyOperation.TextToString( inputField.text, isSpace );

		} );

		// fix
		inputField.onValueChanged.Invoke( "" );

	}


	
	/// <summary>
	/// Set text to field of target control.
	/// If control is NULL - do nothing.
	/// </summary>
	public static void SetText( this Text target, string text ){
		if( target != null ){
			target.text = text;
		}
	}
	/// <summary>
	/// Set text to field of target control.
	/// If control is NULL - do nothing.
	/// </summary>
	public static void SetText( this InputField target, string text ){
		if( target != null ){
			target.text = text;
		}
	}

	/// <summary>
	/// Set sprite to field of target control.
	/// If control is NULL - do nothing.
	/// </summary>
	public static void SetSprite( this Image target, Sprite sprite ){
		if( target != null ){
			target.sprite = sprite;
		}
	}
	/// <summary>
	/// Set fill amount to field of target control.
	/// If control is NULL - do nothing.
	/// </summary>
	public static void SetFillAmount( this Image target, float fillAmount ){
		if( target != null ){
			target.fillAmount = fillAmount;
		}
	}

	
	public static void AddListenerOnClick( this Button target, UnityAction listener ){
		if( target != null ){
			target.onClick.AddListener( listener );
		}
	}
	public static void AddListenerOnClick( this Button[] targets, UnityAction listener ){
		if( targets != null ){
			for( int i = 0; i < targets.Length; i++ ){
				targets[i].AddListenerOnClick( listener );
			}
		}
	}




	/// <summary>
	/// Return Canvas with tag "UICanvasMain".
	/// If not have with tag - return first Canvas.
	/// </summary>
	public static Canvas FindCanvas(){

		GameObject goCanvas = null;
		try{
			goCanvas = GameObject.FindGameObjectWithTag( "UICanvasMain" );
		}catch{}

		Canvas canvas = null;
		if( goCanvas != null ){
			canvas = goCanvas.GetComponent<Canvas>();

		}else{
			canvas = GameObject.FindObjectOfType<Canvas>();
		}

		return canvas;

	}


	/// <summary>
	/// Set "interactable = isVisible" and "blocksRaycasts = isVisible".
	/// </summary>
	public static void Interactable( this CanvasGroup target, bool isVisible ){
		if( target != null ){
			target.interactable = isVisible;
			target.blocksRaycasts = isVisible;
		}
	}
	/// <summary>
	/// Set "interactable = isVisible" and "blocksRaycasts = isVisible".
	/// And set "alpha = isVisible ? 1 : 0".
	/// </summary>
	public static void Visible( this CanvasGroup target, bool isVisible ){
		target.Interactable( isVisible );
		target.alpha = isVisible ? 1 : 0;
	}




	/// <summary>
	/// Return position at left top screen.
	/// </summary>
	public static Vector2 GetLeftTop( RectTransform rect ){
		
		Vector2 min = rect.anchorMin;
		min.x *= Screen.width;
		min.y *= Screen.height;
		min += rect.offsetMin;


		Vector2 max = rect.anchorMax;
		max.x *= Screen.width;
		max.y *= Screen.height;
		max += rect.offsetMax;


		return new Vector2 ( min.x, Screen.height - max.y );
	}
	/// <summary>
	/// Return position at right bottom screen.
	/// </summary>
	public static Vector2 GetRightBottom( RectTransform rect ){
		
		Vector2 min = rect.anchorMin;
		min.x *= Screen.width;
		min.y *= Screen.height;
		min += rect.offsetMin;


		Vector2 max = rect.anchorMax;
		max.x *= Screen.width;
		max.y *= Screen.height;
		max += rect.offsetMax;


		return new Vector2 ( Screen.width - max.x, min.y );
	}



	
	public static void FixStretch( this RectTransform target, float value = 0 ){
		if( target != null ){
			target.offsetMin = new Vector2( value, value );
			target.offsetMax = new Vector2( value, value );
		}
	}
	public static void FixOffsetAll( this GameObject target, float value = 0 ){
		if( target != null ){
			FixStretch( target.transform as RectTransform, value );
		}
	}
	public static void CopyFrom( this RectTransform target, RectTransform from ){
		if( target != null
			&& from != null
		){
			target.offsetMin =			from.offsetMin;
			target.offsetMax =			from.offsetMax;
			target.pivot =				from.pivot;
			target.anchorMin =			from.anchorMin;
			target.anchorMax =			from.anchorMax;
			target.anchoredPosition =	from.anchoredPosition;
		}
	}

	
	/// <summary>
	/// Set local scale to one and set offset for RectTransform.
	/// </summary>
	public static void FixSize( this Transform target, int offsetAll = 0 ){
		MyOperationUI.FixOffsetAll( target.gameObject );
		target.localScale = Vector3.one;
	}
	/// <summary>
	/// Set local scale to one and set offset for RectTransform.
	/// </summary>
	public static void FixSize( this GameObject target, int offsetAll = 0 ){
		target.transform.FixSize( offsetAll );
	}
	public static void FixScale( this Transform target, int scale = 1 ){
		target.localScale = new Vector3( scale, scale, scale );
	}
	public static void FixScale( this GameObject target, int scale = 1 ){
		target.transform.FixScale( scale );
	}




	/// <summary>
	/// Get UI-objects over cursor.
	/// </summary>
	public static List<GameObject> GetAllUIOverCursor(){
		
		if( EventSystem.current == null ){
			return new List<GameObject>();
		}

		// point
		PointerEventData pointer = new PointerEventData( EventSystem.current );
		pointer.position = Input.mousePosition;

		// list have items
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll( pointer, raycastResults );

		// list GameObjects
		List<GameObject> gameObjects = new List<GameObject>();
		for( int i = 0; i < raycastResults.Count; i++ ){
			gameObjects.Add( raycastResults[i].gameObject );
		}

		return gameObjects;
	}
	
	/// <summary>
	/// Get first UI-object over cursor.
	/// </summary>
	public static GameObject GetFirstUIUnderCursor(){
		
		List<GameObject> gameObjects = GetAllUIOverCursor();
		
		if( gameObjects.Count > 0 ){
			return gameObjects[0].gameObject;
		}

		return null;
	}
	/// <summary>
	/// Get need component from first UI-object over cursor.
	/// </summary>
	/// <typeparam name="T">Type component.</typeparam>
	public static T GetFirstUIUnderCursor<T>() where T : class{
		
		GameObject gameObject = GetFirstUIUnderCursor();
		
		if( gameObject != null ){
			return gameObject.GetComponent<T>();
		}

		return null;
	}

	/// <summary>
	/// Cursor over UI?
	/// </summary>
	public static bool IsCursorOverUI(){
		return GetAllUIOverCursor().Count > 0;
	}
	
	/// <summary>
	/// Cursor over need game object (check all object over cursor).
	/// </summary>
	public static bool IsCursorOverUI( GameObject go ){
		List<GameObject> objs = GetAllUIOverCursor();

		for( int i = 0; i < objs.Count; i++ ){
			if( objs[i] == go ){
				return true;
			}
		}
		
		return false;
	}
	
	/// <summary>
	/// Cursor over need game object (check only first game object).
	/// </summary>
	public static bool IsCursorUnderUI( GameObject go ){
		List<GameObject> objs = GetAllUIOverCursor();

		if( objs.Count > 0 && objs[0] == go ){
			return true;
		}

		return false;
	}

	public static bool IsCursorUnderUI<T>( GameObject go ) where T : Component{
		List<GameObject> objs = GetAllUIOverCursor();

		for( int i = 0; i < objs.Count; i++ ){
			if( objs[i].GetComponent<T>() != null ){
				continue;
			}
			if( objs[i] == go ){
				return true;
			}
			return false;
		}
		

		return false;
	}


	public static bool IsCursorUnderUIInParent<T>() where T : Component{
		GameObject goUI = MyOperationUI.GetFirstUIUnderCursor();
		if( goUI != null ){
			return goUI.GetComponentInParent<T>() != null;
		}
		return false;
	}


}
