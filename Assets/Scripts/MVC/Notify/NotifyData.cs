using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kondrat.MVC {

    public class NotifyData {

        public class Param {

            /// <summary>
            /// Name of value
            /// </summary>
            public string Key { get { return key; } }
            private string key = "";

            /// <summary>
            /// Value
            /// </summary>
            public object Value { get { return value; } }
            private object value = "";


            public Param( string key, object value ) {
                this.key = key;
                this.value = value;
            }

        }



        /// <summary>
        /// While is TRUE - notify is executing
        /// </summary>
        public bool IsExecuting { get { return isExecuting; } }
        private bool isExecuting = false;

        /// <summary>
        /// This object send notify
        /// </summary>
        public MonoBehaviour Initiator { get { return initiator; } }
        private MonoBehaviour initiator = null;

        /// <summary>
        /// Params of notify
        /// </summary>
        private Dictionary<string, object> Data { get { return data; } }
        private Dictionary<string, object> data = null;



        /// <summary>
        /// Get param of notify by name
        /// </summary>
        /// <typeparam name="T">Type of param</typeparam>
        /// <param name="name">Name of param</param>
        /// <param name="valueDefault">Return this value if not find param or incorrect type</param>
        public T GetParam<T>( string name, T valueDefault = default(T) ) {
            if( Data.ContainsKey( name )
                && Data[name] is T
            ) {
                return (T)Data[name];
            }

            return valueDefault;
        }

        /// <summary>
        /// Get param of notify by index
        /// </summary>
        /// <typeparam name="T">Type of param</typeparam>
        /// <param name="index">Index of param</param>
        /// <param name="valueDefault">Return this value if not find param or incorrect type</param>
        public T GetParam<T>( int index, T valueDefault = default( T ) ) {
            if( index < 0
                && index >= Data.Values.Count
            ) {
                return valueDefault;
            }

            int find = 0;
            foreach( object value in Data.Values ) {
                if( find++ == index ) {
                    if( value is T ) {
                        return (T)value;
                    }
                    break;
                }
            }

            return valueDefault;
        }

        /// <summary>
        /// Get first param of type
        /// </summary>
        /// <typeparam name="T">Type of param</typeparam>
        /// <param name="valueDefault">Return this value if not find param or incorrect type</param>
        public T GetParam<T>( T valueDefault = default( T ) ) {

            foreach( object value in Data.Values ) {
                if( value is T ) {
                    return (T)value;
                }
            }

            return valueDefault;
        }

        /// <summary>
        /// Get count of params
        /// </summary>
        public int GetCountParams() {
            return data.Count;
        }

        /// <summary>
        /// Has params ?
        /// </summary>
        public bool HasParams() {
            return GetCountParams() > 0;
        }





        /// <param name="data">Params</param>
        public NotifyData( params Param[] data ) {
            isExecuting = true;
            SetData( data );
        }

        /// <param name="initiator">This object is send notify</param>
        /// <param name="data">Params</param>
        public NotifyData( MonoBehaviour initiator, params Param[] data ) {
            isExecuting = true;
            this.initiator = initiator;
            SetData( data );
        }


        /// <summary>
        /// Set data
        /// </summary>
        /// <param name="data">Data</param>
        private void SetData( Param[] data ) {
            this.data = new Dictionary<string, object>();
            for( int i = 0; i < data.Length; i++ ) {
                this.data.Add( data[i].Key, data[i].Value );
            }
        }

        /// <summary>
        /// Get data
        /// </summary>
        public Param[] GetData() {
            Param[] parameters = new Param[this.data.Count];
            int index = 0;

            foreach( string name in this.data.Keys ) {
                parameters[index++] = new Param( name, this.data[name] );
            }

            return parameters;
        }


        /// <summary>
        /// Stop executing
        /// </summary>
        public void Stop() {
            isExecuting = false;
        }
        
    }

}