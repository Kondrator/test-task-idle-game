using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace UIWindowManager{

	/// <summary>
	/// Loader beetwen scene at enum.
	/// </summary>
	/// <typeparam name="T">Template at Enum!</typeparam>
	public abstract class UILoaderBetweenScenesEnum<T> : UILoaderBetweenScenes where T : struct {



		/// <summary>
		/// Load scene at enum.
		/// </summary>
		public static void LoadScene( T sceneEnum ){
		
			UILoaderBetweenScenes.LoadScene( GetIndexEnum( sceneEnum ) );
			
		}



		/// <summary>
		/// Get index scene at enum.
		/// </summary>
		public static int GetIndexEnum( T sceneEnum ){

			Type typeEnum = typeof(T);

			if( typeEnum.IsEnum ){
				return (int)Enum.ToObject( typeEnum, sceneEnum );
			}

			return -1;
		}
		/// <summary>
		/// Get name scene at enum.
		/// </summary>
		public string GetNameEnum( T sceneEnum ){
			return UILoaderBetweenScenes.GetName( GetIndexEnum( sceneEnum ) );
		}



		/// <summary>
		/// Get enum scene at index.
		/// </summary>
		public static T GetEnumIndex( int scene ){
			return (T)Enum.ToObject( typeof(T), scene );
		}

		/// <summary>
		/// Get current scene type.
		/// </summary>
		public static T GetCurrentEnum(){
			return GetEnumIndex( GetCurrent() );
		}


		/// <summary>
		/// Get preview scene type.
		/// </summary>
		public static T GetPrevEnum(){
			return GetEnumIndex( GetPrev() );
		}


	}

	/*
	// NAME_ITEM = INDEX_IN_BUILD
	public enum ExampleSceneType{
		Menu = 0,
		Game = 1
	}
	*/
}