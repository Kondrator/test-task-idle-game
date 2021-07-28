using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;


public static class MyOperationFile{



	/// <summary>
	/// Serialize to file.
	/// </summary>
	public static void Serialize<T>( T obj, string path ){

		FileStream fstream = null;

		try{
			
			fstream = new FileStream( path, FileMode.Create, FileAccess.Write, FileShare.None );
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize( fstream, obj );
			
		}catch( Exception exc ){

			Debug.Log( "Serialize not work (obj = " + obj + ").\n" + exc.Message );

		}

		if( fstream != null ) fstream.Close();

	}
	/// <summary>
	/// Serialize to file.
	/// </summary>
	public static void Serialize<T>( T obj, string folderPath, string fileName ){

		Serialize<T>( obj, folderPath + "/" + fileName );

	}



	/// <summary>
	/// Deserialize from file.
	/// </summary>
	public static T Deserialize<T>( string path ){

		T obj = default( T );
		
		FileStream fstream = null;

		try{
			
			fstream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.None );
			BinaryFormatter bf = new BinaryFormatter();
			obj = (T)bf.Deserialize( fstream );
			
		}catch( Exception exc ){

			Debug.Log( "Deserialize not work.\n" + exc.Message + "\n\n" + exc.StackTrace );

		}
		
		if( fstream != null ) fstream.Close();

		return obj;

	}
	/// <summary>
	/// Deserialize from file.
	/// </summary>
	public static T Deserialize<T>( string folderPath, string fileName ){

		return Deserialize<T>( folderPath + "/" + fileName );

	}




	/// <summary>
	/// Serialize to memory.
	/// </summary>
	public static byte[] Serialize( object data ){
		
		BinaryFormatter bformatter = new BinaryFormatter();
		MemoryStream streamReader = new MemoryStream();

		byte[] bytes = null;

		try{

			bformatter.Serialize( streamReader, data );
			bytes = streamReader.ToArray();

		}catch( Exception exc ){

			Debug.Log( "Serialize not work.\n" + exc.Message + "\n\n" + exc.StackTrace );

		}

		streamReader.Close();

		return bytes;

	}

	/// <summary>
	/// Deserialize from memory.
	/// </summary>
	/// <typeparam name="T">Type result deserialize.</typeparam>
	public static T Deserialize<T>( byte[] bytes ){         

		MemoryStream stream = new MemoryStream( bytes );

		T obj = default( T );

		try{

			BinaryFormatter bformatter = new BinaryFormatter();
			obj = (T)bformatter.Deserialize( stream );

		}catch( Exception exc ){

			Debug.Log( "Deserialize not work.\n" + exc.Message + "\n\n" + exc.StackTrace );

		}


		stream.Close();

		return obj;
	}



	
	/// <summary>
	/// Compress data.
	/// </summary>
	public static byte[] Compress( byte[] data ){

		byte[] compress = new byte[0];

		using( var outStream = new MemoryStream() ){
			using( var tinyStream = new GZipStream( outStream, CompressionMode.Compress ) )
			using( var mStream = new MemoryStream( data ) )
			mStream.CopyTo( tinyStream );

			compress = outStream.ToArray();
		}

		return compress;
	}
	/// <summary>
	/// Compress data.
	/// </summary>
	public static byte[] CompressFromString( string data ){
		return Compress( Encoding.UTF8.GetBytes( data ) );
	}


	/// <summary>
	/// Decompress data.
	/// </summary>
    public static byte[] Decompress( byte[] data ){

		byte[] decompress = new byte[0];

        using( var inStream = new MemoryStream( data ) )
		using( var bigStream = new GZipStream( inStream, CompressionMode.Decompress ) )
		using( var bigStreamOut = new MemoryStream()){
			bigStream.CopyTo( bigStreamOut );
			decompress = bigStreamOut.ToArray();
		}

		return decompress;
    }
	/// <summary>
	/// Decompress data.
	/// </summary>
	public static string DecompressToString( byte[] data ){
		return Encoding.UTF8.GetString( Decompress( data ) );
	}


	public static long CopyTo( this Stream source, Stream destination ) {
		byte[] buffer = new byte[2048];
		int bytesRead;
		long totalBytes = 0;
		while( ( bytesRead = source.Read( buffer, 0, buffer.Length ) ) > 0 ){
			destination.Write( buffer, 0, bytesRead );
			totalBytes += bytesRead;
		}
		return totalBytes;
	}
	
	









	/// <summary>
	/// Remove files and folders in target folder target.
	/// </summary>
	/// <param name="pathFolder">Path at folder target.</param>
	public static void RemoveAll( string pathFolder, bool isRecursice = false ){

		if( Directory.Exists( pathFolder ) == false ) return;

		// удаление файлов
		string[] files = Directory.GetFiles( pathFolder );
		foreach( string file in files ) File.Delete( file );

		if( isRecursice == false ) return;

		// удаление папок
		string[] folders = Directory.GetDirectories( pathFolder );
		foreach( string folder in folders ) Directory.Delete( folder, isRecursice );

	}

	/// <summary>
	/// Create folder.
	/// </summary>
	/// <param name="path">Path folder.</param>
	public static void CreateFolder( string path ){
		
		try{
			Directory.CreateDirectory( path );
		}catch{
			Debug.LogError( "Fail created folder at: " + path );
		}
		
	}

	/// <summary>
	/// Get list files in target folder.
	/// </summary>
	/// <param name="path">Path to folder.</param>
	/// <param name="mask">Filter files.</param>
	/// <param name="isNotGetInRoot">True - root files dont checked, False - check all files.</param>
	public static List<string> GetFiles( string path, string mask = null, bool isRecursice = false, bool isNotGetInRoot = false ){
		
		List<string> filesAll = new List<string>();
		
		DirectoryInfo dirInfo = new DirectoryInfo( path );
		FileInfo[] files = mask == null ? dirInfo.GetFiles() : dirInfo.GetFiles( mask );
		for( int i = 0;i < files.Length; i++ ) if( isNotGetInRoot == false ) filesAll.Add( files[i].FullName );

		// recursice?
		if( isRecursice == true ){
			string[] folders = Directory.GetDirectories( path );
			foreach( string folder in folders ) filesAll.AddRange( GetFiles( folder, mask, true, false ) );
		}

		return filesAll;

	}
	/// <summary>
	/// Get name file.
	/// </summary>
	/// <param name="path">path at file.</param>
	public static string GetFileName( string path ){

		FileInfo fileInfo = new FileInfo( path );

		if( fileInfo != null ) return fileInfo.Name;

		return "File Not Found";
	}
	/// <summary>
	/// Get parent folder name.
	/// </summary>
	/// <param name="path">Path at file.</param>
	public static string GetFolderNameParent( string path ){

		FileInfo fileInfo = new FileInfo( path );

		if( fileInfo != null ) return fileInfo.Directory.Name;

		return "File Not Found";
	}


	/// <summary>
	/// Get folders in folder target.
	/// </summary>
	/// <param name="path">Folder target.</param>
	public static List<string> GetFolders( string path, bool isRecursice = false ){
		
		List<string> dirsAll = new List<string>();
		
		DirectoryInfo dirInfo = new DirectoryInfo( path );
		DirectoryInfo[] dirs = dirInfo.GetDirectories();
		for( int i = 0;i < dirs.Length; i++ ) dirsAll.Add( dirs[i].FullName );

		// recursice?
		if( isRecursice == true ){
			string[] folders = Directory.GetDirectories( path );
			foreach( string folder in folders ) dirsAll.AddRange( GetFolders( folder, true ) );
		}

		return dirsAll;

	}


}
