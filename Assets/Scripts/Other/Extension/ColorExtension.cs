using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension{

	/// <summary>
	/// Get color in the format RRGGBB.
	/// </summary>
	public static string ToHtmlStringRGB( this Color color ){
		return ColorUtility.ToHtmlStringRGB( color );
	}
	

	/// <summary>
	/// Generate color from html string code color.
	/// </summary>
	public static bool TryParseHtmlString( this Color color, string htmlString ){
		return ColorUtility.TryParseHtmlString( htmlString, out color );
	}




	public static Color Clone( this Color color ){
		return new Color( color.r, color.g, color.b, color.a );
	}
	public static Color Clone( this Color color, float alpha ){
		return new Color( color.r, color.g, color.b, alpha );
	}
	public static Color Clone( this Color color, Color alpha ){
		return new Color( color.r, color.g, color.b, alpha.a );
	}


	
	/// <param name="coefficientLightning">value leff 1 - dark color. value more 1 - light color.</param>
	public static Color CloneEffect( this Color color, float coefficientLightning ){
		return new Color( color.r * coefficientLightning, color.g * coefficientLightning, color.b * coefficientLightning, color.a );
	}



	
	/// <summary>
	/// Get the red channel color.
	/// </summary>
	public static int GetRed( this Color color ){
		int red = (int)(color.r * 255f);

		if( red > 255 ){
			red = 255;
		}

		return red;
	}
	/// <summary>
	/// Get the green channel color.
	/// </summary>
	public static int GetGreen( this Color color ){
		int green = (int)(color.g * 255f);

		if( green > 255 ){
			green = 255;
		}

		return green;
	}
	/// <summary>
	/// Get the blue channel color.
	/// </summary>
	public static int GetBlue( this Color color ){
		int blue = (int)(color.b * 255f);

		if( blue > 255 ){
			blue = 255;
		}

		return blue;
	}

}
