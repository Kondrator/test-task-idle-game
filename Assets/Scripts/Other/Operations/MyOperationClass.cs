using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Linq;



public static class MyOperationClass{

	public static bool HasFlag( this Enum input, Enum matchTo ){

		int valueInput = Convert.ToInt32( input );
		if( valueInput == -1 ){
			return true;
		}

		return ( valueInput & Convert.ToInt32( matchTo ) ) != 0;
	}




	public static T CastMy<T>( this object target ){   
		return (T)target;   
	}

	public static object CastMy( this object target, Type type ){
		return Convert.ChangeType( target, type );
	}

	public static List<T> CastMy<T>( this List<object> target, Type type ){

		List<T> result = new List<T>();

		for( int i = 0; i < target.Count; i++ ){
			result.Add( (T)target[i] );
		}

		return result;
	}
	public static object CastMy( this List<object> target, Type type ){

		List<object> result = new List<object>();

		for( int i = 0; i < target.Count; i++ ){
			result.Add( target[i].CastMy( type ) );
		}

		return result;
	}




	

	/// <summary>
	/// Have need attribute?
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	/// <param name="obj">Target.</param>
	public static bool IsHaveAttribute<T>( object obj ) where T : Attribute{

		if( obj.GetType().GetCustomAttributes( typeof(T), true ).Length > 0 ) return true;

		return false;

	}
	/// <summary>
	/// Have need attribute?
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	/// <param name="field">Target.</param>
	public static bool IsHaveAttribute<T>( FieldInfo field ) where T : Attribute{
		
		if( field.GetCustomAttributes( typeof(T), true ).Length > 0 ) return true;

		return false;

	}
	/// <summary>
	/// Get need attribute.
	/// Not have attribute - return NULL.
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	/// <param name="field">Target.</param>
	public static T GetAttribute<T>( FieldInfo field ) where T : Attribute{
		
		if(	field == null ) return null;

		object[] attributes = field.GetCustomAttributes( typeof(T), true );

		for( int i = 0; i < attributes.Length; ){
			return (T)attributes[i];
		}

		return null;
	}
	/// <summary>
	/// Get need attribute.
	/// Not have attribute - return NULL.
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	/// <param name="obj">Target.</param>
	public static T GetAttribute<T>( object obj ) where T : Attribute{
		
		if(	obj == null ) return null;

		return Attribute.GetCustomAttribute( obj.GetType(), typeof (T) ) as T;
	}

	/// <summary>
	/// Get need attribute.
	/// Not have attribute - return NULL.
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	public static T GetAttributeField<T>( this FieldInfo taget ) where T : Attribute{
		return GetAttribute<T>( taget );
	}


	/// <summary>
	/// Get need attributes.
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	/// <param name="obj">Target.</param>
	public static T[] GetAttributes<T>( object obj ) where T : Attribute{
		
		FieldInfo[] fields = GetFields<T>( obj );

		T[] attributes = new T[fields.Length];

		for( int i = 0; i < fields.Length; i++ ){
			
			attributes[i] = fields[i].GetAttributeField<T>();

		}

		return attributes;
	}


	/// <summary>
	/// Get fields.
	/// </summary>
	/// <param name="obj">Объект.</param>
	public static FieldInfo[] GetFields( object obj ){

		if( obj == null ) {
			return new FieldInfo[0];
		}


		Type type = obj is Type ? (Type)obj : obj.GetType();
		List<FieldInfo> result = new List<FieldInfo>();


		// read need fields
		FieldInfo[] fields = type.GetFields(	BindingFlags.Instance 
												| BindingFlags.Public 
												| BindingFlags.NonPublic 
												| BindingFlags.FlattenHierarchy
												| BindingFlags.GetProperty
												| BindingFlags.SetProperty
											);
		result.AddRange( fields );


		// get pivate fields in base type
		type = type.BaseType;
		do {
			fields = type.GetFields(	BindingFlags.Instance
										| BindingFlags.NonPublic
									);
			result.AddRange( fields );

			type = type.BaseType;
		} while( type != null );


		return result.ToArray();

	}


	/// <summary>
	/// Get field with need attribute.
	/// </summary>
	/// <typeparam name="T">Type attribute.</typeparam>
	/// <param name="obj">Target.</param>
	public static FieldInfo[] GetFields<T>( object obj ) where T : Attribute{

		// get feidls
		FieldInfo[] fields = GetFields( obj );

		// filter fields
		List<FieldInfo> fieldsFilter = new List<FieldInfo>();
		for( int i = 0; i < fields.Length; i++ ){

			if( IsHaveAttribute<T>( fields[i] ) == true ){
				fieldsFilter.Add( fields[i] );
			}

		}


		return fieldsFilter.ToArray();

	}




	/// <summary>
	/// Get field.
	/// </summary>
	/// <param name="obj">Target.</param>
	/// <param name="name">Name field.</param>
	public static FieldInfo GetField( object obj, string name ){
		
		FieldInfo[] fields = GetFields( obj );

		for( int i = 0; i < fields.Length; i++ ){

			if( fields[i].Name == name ){
				return fields[i];
			}

		}

		return null;

	}

	/// <summary>
	/// Set value to need field.
	/// </summary>
	/// <param name="obj">Target.</param>
	/// <param name="name">Name field in target.</param>
	/// <param name="value">Value.</param>
	public static void SetField( object obj, string name, object value ){

		try{

			FieldInfo fieldInfo = GetField( obj, name );
		
			fieldInfo.SetValue( obj, value );

		}catch( Exception exc ){
			Debug.Log( "Fail writed field: " + name + " = " + value + "\n<b>" + exc.Message +"</b>" );
		}

	}



	/// <summary>
	/// Get value field.
	/// </summary>
	/// <param name="obj">Target.</param>
	/// <param name="name">Name field.</param>
	public static T GetFieldValue<T>( object obj, string name ){

		// get field
		FieldInfo field = (obj is Type ? (Type)obj : obj.GetType()).GetField( name );
		if( field != null ) {
			return (T)field.GetValue( obj );
		}

		return default(T);
	}






	/// <summary>
	/// Target type is user.
	/// </summary>
	/// <param name="type">Target.</param>
	public static bool IsUserDefined( Type type ){

		var td = TypeDescriptor.GetConverter( type) ;

		if( td.GetType() == typeof(TypeConverter) ){
			return true;
		}

		return false;
	}



	/// <summary>
	/// Get type from string.
	/// </summary>
	/// <param name="strFullyQualifiedName">Full name type.</param>
	public static Type GetType( string strFullyQualifiedName ){

		Type type = Type.GetType( strFullyQualifiedName );
		if( type != null ){
			return type;
		}

		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for( int i = 0; i < assemblies.Length; i++ ){
			type = assemblies[i].GetType( strFullyQualifiedName );
			if( type != null ){
				return type;
			}
		}

		return null;
	}
	
	/// <summary>
	/// Create object class at need type.
	/// </summary>
	/// <param name="strFullyQualifiedName">Full name type.</param>
	/// <param name="parameters">For constructor.</param>
	/// <returns>Object class target.</returns>
	public static object GetInstance( string strFullyQualifiedName, params object[] parameters ){
		
		Type type = GetType( strFullyQualifiedName );
		if( type != null ){
			
			Type[] types = new Type[parameters.Length];
			for( int i = 0; i < parameters.Length; i++ ){
				types[i] = parameters[i].GetType();
			}

			BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			ConstructorInfo ctor = type.GetConstructor( flags, null, types, null );

			object instance = null;
			if( ctor != null ) instance = ctor.Invoke( parameters );
			else instance = FormatterServices.GetUninitializedObject( type );

			return instance;
		}

		return null;
	}
	/// <summary>
	/// Create object class at need type.
	/// </summary>
	/// <param name="parameters">For constructor.</param>
	/// <returns>Object class target.</returns>
	public static T GetInstance<T>( params object[] parameters ){
		return (T)GetInstance( typeof(T).FullName, parameters );
	}

	/// <summary>
	/// Create list at need type.
	/// </summary>
	/// <param name="type">Type.</param>
	public static IList GetInstanceList( Type type ){
		Type genericListType = typeof(List<>).MakeGenericType( type );
		return (IList)Activator.CreateInstance( genericListType );
	}

	/// <summary>
	/// Create list at need type.
	/// </summary>
	/// <param name="strFullyQualifiedName">Full name type.</param>
	public static IList GetInstanceList( string strFullyQualifiedName ){
		Type type = GetType( strFullyQualifiedName );
		if( type != null ){
			return GetInstanceList( type );
		}
		
		return null;
	}





	/// <summary>
	/// Get name of type without generics
	/// </summary>
	/// <param name="t">Target type</param>
	public static string GetNameWithoutGenericArity( this Type t ) {
		string name = t.Name;
		int index = name.IndexOf( '`' );
		return index == -1 ? name : name.Substring( 0, index );
	}




	public static Type GetTypeBase<T>( this Type type ) where T : class {
		return GetTypeBase( typeof(T), type );
	}

	private static Type GetTypeBase( Type check, Type target ) {
		if( target == null ) {
			return null;
		}

		if( check == target ) {
			return target;
		}

		if( check.GetNameWithoutGenericArity() == target.GetNameWithoutGenericArity() ) {
			Type[] checkGenericArguments = check.GetGenericArguments();
			Type[] targetGenericArguments = target.GetGenericArguments();

			if( checkGenericArguments.Length == targetGenericArguments.Length ) {
				for( int i = 0; i < checkGenericArguments.Length; i++ ) {

					if( checkGenericArguments[i] == targetGenericArguments[i] ) {
						continue;
					}

					if( targetGenericArguments[i].IsSubclassOf( checkGenericArguments[i] ) ) {
						continue;
					}

					goto GENERIC_FAILED;
				}

				return target;
			}
			GENERIC_FAILED:;
		}

		return GetTypeBase( check, target.BaseType );
	}


	/// <summary>
	/// Get all types inherited by target
	/// </summary>
	/// <param name="type">Taret type</param>
	/// <returns></returns>
	public static Type[] GetAllBaseTypes( Type type ){

		List<Type> result = new List<Type>();

		Type[] types = type.Assembly.GetTypes();
		for( int i = 0; i < types.Length; i++ ) {
			if( types[i].IsSubclassOf( type ) ) {
				result.Add( types[i] );
			}
		}

		return result.ToArray();
	}

}