using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public static class ReflectionExtension {



	public static List<System.Type> GetInheritedListTypes<T>() where T : class {

		List<System.Type> types = new List<System.Type>();

		foreach( Type type in	Assembly.GetAssembly( typeof(T) ).GetTypes()
								.Where(	myType => myType.IsClass
										&& !myType.IsAbstract
										&& myType.IsSubclassOf( typeof(T) )
								)
		){
			types.Add( type );
		}
		
		return types;
	}



}