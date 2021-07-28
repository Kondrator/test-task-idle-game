using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UIWindowManager;
using UnityEngine;



public class HousesController : Controller<HousesModel, HousesView> {

    public class NOTIFY {

        public class TRY_UPGRADE {

            public const string NAME = "houses.controller.try.upgrade";

            /// <summary>
            /// Type = HouseController
            /// </summary>
            public const string PARAM_HOUSE = "house";

        }

    }




    protected override void PreInitiate() { }

    protected override void Initiate() {

        Add(
            HouseController.NOTIFY.TOUCH.NAME,
            data => {
                HouseController house = data.Initiator as HouseController;
                View.Window.Show( house );
            }
        );


        Add(
            NOTIFY.TRY_UPGRADE.NAME,
            data => {

                HouseController house = data.GetParam<HouseController>( NOTIFY.TRY_UPGRADE.PARAM_HOUSE );
                int price = house.Model.GetPriceUpgrade();

                if( Model.Game.UseCoins( price ) ) {
                    View.Window.Close();
                    house.Model.Upgrade();

                } else {
                    UIMessageBox.CreateShow( string.Format( "Not enough coins : {0}", price - Model.Game.Coins ), UIMessageBoxButtons.Close );
                }
                
            }
        );

    }


}
