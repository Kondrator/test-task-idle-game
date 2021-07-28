using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Kondrat.MVC {

    public class NotifySender : Element {

        [SerializeField]
        private NotifyName notify = null;



        public void Send() {
            Notify( notify.GetValue() );
        }

    }

}