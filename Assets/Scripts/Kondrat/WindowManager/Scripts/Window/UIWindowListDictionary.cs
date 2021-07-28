using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace UIWindowManager{

	/// <summary>
	/// This is auto generate items Monobehaviors. Extra version with use dictionary.
	/// </summary>
	/// <typeparam name="T">Type of generate item.</typeparam>
	/// <typeparam name="DK">Type of key in dictionary.</typeparam>
	/// <typeparam name="DV">Type of value in dictionary.</typeparam>
	public abstract class UIWindowListDictionary<T, DK, DV> : UIWindowList<T> where T : MonoBehaviour {

		public sealed override int GetCount(){
			return dict.Count;
		}
	
		protected sealed override void SettingItem( T item, int index ){
			DK key = dictKeys[index];
			DV value = dict[key];
			SettingItemExtra( item, key, value, index );
		}
		protected abstract void SettingItemExtra( T item, DK key, DV value, int index );



		private Dictionary<DK, DV> dict = null;
		private DK[] dictKeys = null;
		/// <summary>
		/// Get used items for generate.
		/// </summary>
		protected abstract Dictionary<DK, DV> GetDict();

		protected override void BeforeGenerate(){
			dict = GetDict();
			dictKeys = dict.Keys.ToArray();
		}

		protected override void AfterGenerate(){
			dict = null;
			dictKeys = null;
		}


	
		public override void CopyFieldsFrom( UIWindow window ){
			base.CopyFieldsFrom( window );

			if( window is UIWindowListDictionary<T, DK, DV> ){
				UIWindowListDictionary<T, DK, DV> windowDict = (UIWindowListDictionary<T, DK, DV>)window;
				this.dict = windowDict.dict;
				this.dictKeys = windowDict.dictKeys;
			}
		}

	}

}