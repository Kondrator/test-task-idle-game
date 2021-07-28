#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Editor<T> : Editor where T : class {

    protected T component = null;

    protected virtual void OnEnable() {
        component = base.target as T;
    }

}
#endif