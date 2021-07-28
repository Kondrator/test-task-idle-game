using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kondrat.MVC {

    public abstract class Element : MonoBehaviour {


        private static Dictionary<System.Type, List<Element>> Elements {
            get {
                if( elements == null ) {
                    elements = new Dictionary<System.Type, List<Element>>();
                }
                return elements;
            }
        }
        private static Dictionary<System.Type, List<Element>> elements = null;



        private System.Type Type { get { return type = type ?? this.GetType(); } }
        private System.Type type = null;


#if UNITY_EDITOR
        private static Dictionary<System.Type, object> cacheFindEditor = null;
        private static Dictionary<System.Type, object> CacheFindEditor { get { return cacheFindEditor = cacheFindEditor ?? new Dictionary<System.Type, object>(); } }
#endif


        public static T Find<T>( T element, System.Func<Element, bool> filter = null ) where T : Element {

#if UNITY_EDITOR
            if( Application.isPlaying == false ) {
                System.Type typeCacheEditor = typeof(T);
                if( CacheFindEditor.ContainsKey( typeCacheEditor ) == false
                    || CacheFindEditor[typeCacheEditor] == null
                ) {
                    CacheFindEditor[typeCacheEditor] = UnityEditor.MyOperationEditor.FindAssetByType<T>();
                }
                return CacheFindEditor[typeCacheEditor] as T;
            }
#endif

            if( element != null ) {
                return element;
            }

            System.Type type = typeof( T );
            if( Elements.ContainsKey( type ) ) {
                for( int i = 0; i < Elements[type].Count; i++ ) {
                    if( filter?.Invoke( Elements[type][i] ) == false ) {
                        continue;
                    }

                    return Elements[type][i] as T;
                }
            }

            return null;
        }

        public static T Find<T>() where T : Element {
            return Find<T>( null );
        }





        public static T Find<T>( T element, bool withChildClasses, System.Func<Element, bool> filter = null ) where T : Element {

            if( withChildClasses == false ) {
                return Find( element, filter );
            }


            if( element != null ) {
                return element;
            }

            System.Type type = typeof( T );
            foreach( System.Type key in Elements.Keys ) {
                if( key.IsSubclassOf( type ) ) {
                    for( int i = 0; i < Elements[key].Count; i++ ) {
                        if( filter?.Invoke( Elements[type][i] ) == false ) {
                            continue;
                        }

                        return Elements[key][i] as T;
                    }
                }
            }

            return null;
        }

        public static T Find<T>( bool withChildClasses ) where T : Element {
            return Find<T>( null, withChildClasses );
        }









        public static T[] FindAll<T>( System.Func<T, bool> filter = null ) where T : Element {

            List<T> result = new List<T>();

            System.Type type = typeof( T );
            if( Elements.ContainsKey( type ) ) {
                for( int i = 0; i < Elements[type].Count; i++ ) {
                    if( filter?.Invoke( (T)Elements[type][i] ) == false ) {
                        continue;
                    }

                    result.Add( Elements[type][i] as T );
                }
            }

            return result.ToArray();
        }







        protected virtual void OnEnable() {
            if( Elements.ContainsKey( Type ) == false ) {
                Elements[Type] = new List<Element>();
            }
            Elements[Type].Add( this );
        }

        protected virtual void OnDisable() {
            if( Elements.ContainsKey( Type ) == true ) {
                Elements[Type].Remove( this );
            }
        }






        /// <summary>
        /// Send notify
        /// </summary>
        /// <param name="name">Name of notify</param>
        /// <param name="data">Params of notify</param>
        public void Notify( string name, params NotifyData.Param[] data ) {
            Core.Singleton.Notify( name, new NotifyData( this, data ) );
        }



        /// <summary>
        /// Send notify
        /// </summary>
        /// <param name="delay">Wait for send notify (seconds)</param>
        /// <param name="name">Name of notify</param>
        /// <param name="data">Params of notify</param>
        public void Notify( float delay, string name, params NotifyData.Param[] data ) {
            if( this.enabled == false ) {
                return;
            }

            TimerExecutor.Add( delay, () => {
                Notify( name, data );
            } );
        }


    }



    public abstract class Element<T> : Element where T : Element {

        public T Owner {
            get {
                return owner = Find( owner );
            }
        }
        private T owner = null;

    }



    public static class ElementExtension {

        public static void InvokeOperaton( this Element target, System.Action operation ) {
            target.enabled = false;
            operation();
            target.enabled = true;
        }

    }

}