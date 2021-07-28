using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIWindowManager{

	/// <summary>
	/// This is auto generate items Monobehaviors. Extra version with use array.
	/// </summary>
	/// <typeparam name="T">Type of generate item.</typeparam>
	/// <typeparam name="K">Type of user item in array for generate item.</typeparam>
	public abstract class UIWindowListArray<T, K> : UIWindowList<T> where T : MonoBehaviour {


		public sealed override int GetCount(){
			if( items == null ){
				return 0;
			}
			return items.Length;
		}
	
		protected sealed override void SettingItem( T container, int index ){
			SettingItemExtra( container, items[index], index );
		}
		protected abstract void SettingItemExtra( T container, K item, int index );



		K[] items = null;
		/// <summary>
		/// Get used items for generate.
		/// </summary>
		protected abstract K[] GetItems();

		protected override void BeforeGenerate(){
			items = GetItems();
		}

		protected override void AfterGenerate(){
			//items = null;
		}


		public K GetItemData( int index ){
			if( items == null ){
				return default(K);
			}

			return items[index];
		}


		


		public override void CopyFieldsFrom( UIWindow window ){
			base.CopyFieldsFrom( window );

			if( window is UIWindowListArray<T, K> ){
				UIWindowListArray<T, K> windowArray = (UIWindowListArray<T, K>)window;
				this.items = windowArray.items;
			}
		}

	}

}