using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kondrat.MVC {

    public abstract class Controller : Element {

        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="name">Name of notify</param>
        /// <param name="callback">Callback executing notify</param>
        /// <param name="priority">Callback by priority (queue)</param>
        /// <param name="filter">Filter notify</param>
        private void Add( string name, DNotify callback, Core.NotifyPriority priority, System.Func<NotifyData, bool> filter = null ) {
            Core.Singleton.Add( this, name, callback, priority, filter );
        }


        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="name">Name of notify</param>
        /// <param name="callback">Callback executing notify</param>
        protected void Add( string name, DNotify callback, System.Func<NotifyData, bool> filter = null ) {
            Add( name, callback, Core.NotifyPriority.Normal, filter );
        }


        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="name">Name of notify</param>
        /// <param name="priority">Callback by priority (queue)</param>
        /// <param name="callback">Callback executing notify</param>
        protected void Add( string name, Core.NotifyPriority priority, DNotify callback, System.Func<NotifyData, bool> filter = null ) {
            Add( name, callback, priority, filter );
        }





        /// <summary>
        /// Add observers
        /// </summary>
        /// <param name="names">Names of notifies</param>
        /// <param name="callback">Callback executing notify</param>
        /// <param name="priority">Callback by priority (queue)</param>
        private void Add( string[] names, DNotify callback, Core.NotifyPriority priority, System.Func<NotifyData, bool> filter = null ) {
            for( int i = 0; i < names.Length; i++ ) {
                Core.Singleton.Add( this, names[i], callback, priority, filter );
            }
        }


        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="names">Names of notifies</param>
        /// <param name="callback">Callback executing notify</param>
        protected void Add( string[] names, DNotify callback, System.Func<NotifyData, bool> filter = null ) {
            Add( names, callback, Core.NotifyPriority.Normal, filter );
        }


        /// <summary>
        /// Add observer
        /// </summary>
        /// <param name="names">Names of notifies</param>
        /// <param name="priority">Callback by priority (queue)</param>
        /// <param name="callback">Callback executing notify</param>
        protected void Add( string[] names, Core.NotifyPriority priority, DNotify callback, System.Func<NotifyData, bool> filter = null ) {
            Add( names, callback, priority, filter );
        }





        protected abstract void PreInitiate();
        protected abstract void Initiate();




        protected override void OnEnable() {
            base.OnEnable();

            PreInitiate();
            Initiate();
        }

        protected override void OnDisable() {
            base.OnDisable();

            Core.Singleton.Remove( this );
        }

        /// <summary>
        /// Re invoke Per/Initaite permanent
        /// </summary>
        public void ReInit(){
            Core.Singleton.Remove( this );

            PreInitiate();
            Initiate();
        }

    }



    public abstract class Controller<TModel, TView> : Controller where TModel : Model where TView : View {

        private TModel model = null;
        public TModel Model {
            get {
                if( model == null ) {
                    model = GetComponent<TModel>();
                    if( model == null ) {
                        model = Find( model );
                    }
                }
                return model;
            }
        }

        private TView view = null;
        public TView View {
            get {
                if( view == null ) {
                    view = GetComponent<TView>();
                    if( view == null ) {
                        view = Find( view );
                    }
                }
                return view;
            }
        }

    }

    public abstract class Controller<TView> : Controller where TView : View {

        private TView view = null;
        public TView View {
            get {
                if( view == null ) {
                    view = GetComponent<TView>();
                    if( view == null ) {
                        view = Find( view );
                    }
                }
                return view;
            }
        }

    }

}