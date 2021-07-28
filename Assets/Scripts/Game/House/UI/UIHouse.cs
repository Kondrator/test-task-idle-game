using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UIWindowManager;
using UnityEngine;
using UnityEngine.UI;


public class UIHouse : UIWindow {


    [SerializeField]
    private Text textTitle = null;

    [SerializeField]
    private Text textLevel = null;

    [SerializeField]
    private Text textReward = null;

    [SerializeField]
    private Text textPrice = null;


    [SerializeField]
    private Button buttonUpgrade = null;


    private HouseController target = null;



    protected override void OnAwake() {
        base.OnAwake();

        buttonUpgrade.onClick.AddListener( () => {
            Notify(
                HousesController.NOTIFY.TRY_UPGRADE.NAME,
                new NotifyData.Param( HousesController.NOTIFY.TRY_UPGRADE.PARAM_HOUSE, target )
            ); 
        } );
    }


    public void Show( HouseController target ) {

        this.target = target;

        textTitle.SetText( target.Model.Name );
        textLevel.SetText( target.Model.Level.ToString() );
        textReward.SetText( target.Model.GetReward().ToStringSplitSymbol() );
        textPrice.SetText( target.Model.GetPriceUpgrade().ToStringSplitSymbol() );

        Open();

    }


}
