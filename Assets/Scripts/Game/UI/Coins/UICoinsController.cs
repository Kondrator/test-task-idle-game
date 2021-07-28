using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UICoinsController : Controller<UICoinsModel, UICoinsView> {


    protected override void PreInitiate() { }

    protected override void Initiate() {


        Add(
            SceneController.NOTIFY_LOADED,
            data => {
                View.TextCount.SetText( Model.Game.Coins.ToString() );
            }
        );


        Add(
            GameModel.NOTIFY.COINS.CHANGED.NAME,
            data => {
                int coins = data.GetParam<int>( GameModel.NOTIFY.COINS.PARAM_VALUE );
                View.TextCount.SetText( coins.ToStringSplitSymbol() );
            }
        );


    }

}
