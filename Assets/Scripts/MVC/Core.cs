using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kondrat.MVC {

    public class Core {

        public enum NotifyPriority {
            Low = 1,
            Normal = 2,
            High = 3,
        }

        private class ObserverData {

            /// <summary>
            /// Owner of observer
            /// </summary>
            public Controller Owner { get { return owner; } }
            private Controller owner = null;

            /// <summary>
            /// Callback of notify executing
            /// </summary>
            public DNotify Callback { get { return callback; } }
            private DNotify callback = null;

            /// <summary>
            /// Priority for callback (executing in order priority)
            /// </summary>
            public NotifyPriority Priority { get { return priority; } }
            private NotifyPriority priority = NotifyPriority.Normal;

            /// <summary>
            /// Filter notify
            /// </summary>
            private System.Func<NotifyData, bool> filter = null;






            /// <param name="owner">Owner of observer</param>
            /// <param name="callback">Callback of notify executing</param>
            public ObserverData( Controller owner, DNotify callback, NotifyPriority priority, System.Func<NotifyData, bool> filter ) {
                this.owner = owner;
                this.callback = callback;
                this.priority = priority;
                this.filter = filter;
            }



            /// <summary>
            /// Is succes filtering this observer ?
            /// </summary>
            public bool IsFilter( NotifyData data ) {
                if( filter == null ) {
                    return true;
                }

                return filter( data );
            }

        }



        
        public static Core Singleton {
            get {
                if( singleton == null ) {
                    singleton = new Core();
                }
                return singleton;
            }
        }
        private static Core singleton = null;



        private Dictionary<string, List<ObserverData>> observers = null;



        private Core() {
            observers = new Dictionary<string, List<ObserverData>>();
        }



        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="owner">Owner of observer</param>
        /// <param name="name">Name of notify</param>
        /// <param name="callback">Callback executing notify</param>
        /// <param name="priority">Callback by priority (queue)</param>
        /// <param name="filter">Filter notify</param>
        public void Add( Controller owner, string name, DNotify callback, NotifyPriority priority = NotifyPriority.Normal, System.Func<NotifyData, bool> filter = null ) {
            if( observers.ContainsKey( name ) == false ) {
                observers[name] = new List<ObserverData>();
            }
            observers[name].Add( new ObserverData( owner, callback, priority, filter ) );
            observers[name].Sort( ( ObserverData a, ObserverData b ) => b.Priority.CompareTo( a.Priority ) );
        }

        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="owner">Owner of observer</param>
        /// <param name="name">Name of notify</param>
        /// <param name="callback">Callback executing notify</param>
        /// <param name="filter">Filter notify</param>
        public void Add( Controller owner, string name, DNotify callback, System.Func<NotifyData, bool> filter ) {
            Add( owner, name, callback, NotifyPriority.Normal, filter );
        }


        /// <summary>
        /// Remove observer
        /// </summary>
        /// <param name="owner">Owner of observer</param>
        /// <param name="name">Name of notify</param>
        public void Remove( Controller owner ) {
            foreach( string key in observers.Keys ) {
                for( int i = 0; i < observers[key].Count; i++ ) {
                    if( observers[key][i].Owner == owner ) {
                        observers[key].RemoveAt( i-- );
                    }
                }
            }
        }

        /*
        /// <summary>
        /// Remove observer
        /// </summary>
        /// <param name="owner">Owner of observer</param>
        /// <param name="name">Name of notify</param>
        public void Remove( Element owner, string name ) {
            if( observers.ContainsKey( name ) == true ) {
                for( int i = 0; i < observers[name].Count; i++ ) {
                    if( observers[name][i].Owner == owner ) {
                        observers[name].RemoveAt( i-- );
                    }
                }
            }
        }
        */


        /// <summary>
        /// Send notify
        /// </summary>
        /// <param name="name">Name of notify</param>
        /// <param name="data">Params of notify</param>
        public void Notify( string name, NotifyData data ) {
            if( observers.ContainsKey( name ) == true ) {
                for( int i = 0; i < observers[name].Count; i++ ) {

                    if( observers[name][i].Owner == null ) {
                        observers[name].RemoveAt( i-- );
                        continue;
                    }

                    if( observers[name][i].IsFilter( data ) ) {
                        observers[name][i].Callback( data );
                        if( data.IsExecuting == false ) {
                            break;
                        }
                    }
                }
            }
        }

    }

    public delegate void DNotify( NotifyData data );

}