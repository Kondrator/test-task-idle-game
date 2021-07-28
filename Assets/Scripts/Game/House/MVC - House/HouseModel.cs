using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HouseModel : Model {

    public class NOTIFY {

        public class UPGRADED {

            public const string NAME = "house.model.upgraded";

        }

    }



    [SerializeField]
    private string _name = "House";
    public string Name { get { return _name; } }



    [Header( "Reward" )]

    [SerializeField]
    [Range( 1, 100 )]
    private int rewardBase = 10;

    [SerializeField]
    [Range( 0.1f, 1f )]
    private float powerRewardByLevel = 1f;



    [Header( "Upgrade" )]

    [SerializeField]
    [Range( 10, 1000 )]
    private int priceUpgrade = 50;

    [SerializeField]
    [Range( 0.1f, 1f )]
    private float powerPriceByLevel = 1f;




    private int level = 1;
    public int Level => level;



    public void Upgrade() {

        level++;

    }


    /// <summary>
    /// Reward by current level
    /// </summary>
    public int GetReward() {
        return (int)Mathf.Pow( rewardBase, level * powerRewardByLevel );
    }


    /// <summary>
    /// Price for upgrade
    /// </summary>
    public int GetPriceUpgrade() {
        return (int)Mathf.Pow( priceUpgrade, level * powerPriceByLevel );
    }


}
