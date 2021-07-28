using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class MyOperationTransform {


    public static Transform FindSimple( this Transform parent, string name ) {
        for( int i = 0; i < parent.childCount; i++ ) {
            if( parent.GetChild( i ).name == name ) {
                return parent.GetChild( i );
            }
        }
        return null;
    }





    /// <summary>
    /// Craete objects in tree by path
    /// </summary>
    /// <param name="parent">Base parent</param>
    /// <param name="path">Path (example: first/second/third)</param>
    public static void CreatePath( this Transform parent, string path ) {
        string[] parts = path.Split( '/' );
        CreatePath( parent, parts, 0 );
    }

    private static void CreatePath( Transform parent, string[] parts, int index ) {
        if( index >= parts.Length ) {
            return;
        }

        if( parts[index].Length == 0 ) {
            CreatePath( parent, parts, index + 1 );
            return;
        }

        Transform transformFind = parent.FindSimple( parts[index] );
        if( transformFind != null ) {
            CreatePath( transformFind.transform, parts, index + 1 );
            return;
        }

        GameObject go = new GameObject( parts[index] );
        go.transform.SetParent( parent );
        go.transform.ResetTransform();

        CreatePath( go.transform, parts, index + 1 );
    }






    /// <summary>
    /// Get path to this object
    /// </summary>
    public static string GetPath( this Transform target ) {
        if( target == null ) {
            return "";
        }

        if( target.parent == null ) {
            return target.name;
        }

        return string.Format( "{0} / {1}", target.parent.GetPath(), target.name );
    }




    /// <summary>
    /// Get object in tree by path
    /// </summary>
    /// <param name="parent">Base parent</param>
    /// <param name="path">Path (example: first/second/third)</param>
    public static Transform GetPath( Transform parent, string path ) {
        string[] parts = path.Split( '/' );
        CreatePath( parent, parts, 0 );
        return GetPath( parent, parts, 0 );
    }

    private static Transform GetPath( Transform parent, string[] parts, int index ) {
        if( index >= parts.Length ) {
            return parent;
        }

        if( parts[index].Length == 0 ) {
            return GetPath( parent, parts, index + 1 );
        }

        return GetPath( parent.FindSimple( parts[index] ), parts, index + 1 );
    }


}
