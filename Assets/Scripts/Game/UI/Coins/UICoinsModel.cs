using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICoinsModel : Model {


    private GameModel game = null;
    public GameModel Game { get { return game = Find( game ); } }


}
