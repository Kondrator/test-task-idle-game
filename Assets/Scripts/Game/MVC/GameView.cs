using Kondrat.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameView : View {


    private Spawner spawner = null;
    public Spawner Spawner { get { return spawner = Find( spawner ); } }


}
