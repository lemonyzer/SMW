using UnityEngine;
using System.Collections;

public class TeamColor
{

	public static int mColorCount = 4;
	public static int mColorIntensityCount = 3;

	public static void ChangeColors(Color32[] newColors, Texture2D texture)
	{

		// check if Sprite can be accessed
		//TODO
		if (texture.format != TextureFormat.ARGB32 &&
		    texture.format != TextureFormat.RGBA32 &&
		    texture.format != TextureFormat.RGBA4444)
		{
			Debug.LogError("texture format cant be accessed!!!");
            return;
        }
        
        
        // Anzahl der veränderten Pixel
		int fPixelChanged = 0;

//		texture.filterMode = FilterMode.Bilinear;
//		texture.wrapMode = TextureWrapMode.Clamp;
		
		for (int y = 0; y < texture.height; y++)
		{
			#if UNITY_EDITOR
			//Debug.Log("y: " + y);
			#endif
			for (int x = 0; x < texture.width; x++)
			{
				Color32 currentColor = texture.GetPixel (x,y);
				

				
				Color32 newColor = new Color32();
				bool pixelHasReferenceColor = false;
				// schleife:
				// schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
				for (int iColor = 0; iColor < mColorCount; iColor++)
				{
					Color32 refColor;
					for (int iColorIntensity = 0; iColorIntensity < mColorIntensityCount; iColorIntensity++)
					{
						refColor = TeamColor.referenceColors[iColor,iColorIntensity];

						if(currentColor.Equals(refColor))
						{
//							newColor = TeamColor.referenceColors[fColorId,iColorIntensity];
							newColor = newColors[iColorIntensity];
                            pixelHasReferenceColor = true;
                            break;
                        }
                    }
                    if(pixelHasReferenceColor)
                        break;
                }
                
                if(pixelHasReferenceColor)
                {
                    texture.SetPixel (x, y, newColor);
                    fPixelChanged++;
                }
                
            }
        }
        Debug.Log("Anzahl an geänderten Pixel = " + fPixelChanged);
        texture.Apply();
    }
    
    public void ModifyTexture2D(Texture2D texture, int fColorId)
    {
        
    }
    
    //	public static Color32[] referenceColors = new Color32[] {
    //		new Color32 ((byte)128, (byte)0, (byte)0, (byte)255),
    //		new Color32 ((byte)192, (byte)0, (byte)0, (byte)255),
    //		new Color32 ((byte)255, (byte)0, (byte)0, (byte)255) };
    
    //	public static Color32[] teamGreenColors = new Color32[] {
	//		new Color32 ((byte)90, (byte)146, (byte)0, (byte)255),
	//		new Color32 ((byte)123, (byte)178, (byte)0, (byte)255),
	//		new Color32 ((byte)165, (byte)219, (byte)0, (byte)255) };
	
	//Color = [R]ed [G]reen [B]lue [A]lpha Format
	//Color (1f, 0f, 0f, 1f) => red
	
	//	public static Color redDark = new Color(255f,0f,0f,1f);
	//	public static Color redMiddel = new Color(192f,0f,0f,1f);
	//	public static Color redLight = new Color(128f,0f,0f,1f);
	//	
	//	public static Color greeDark = new Color(255f,0f,0f,1f);
	//	public static Color greeMiddel = new Color(192f,0f,0f,1f);
	//	public static Color greeLight = new Color(128f,0f,0f,1f);
	//	
	//	public static Color yellowDark = new Color(255f,0f,0f,1f);
	//	public static Color yellowMiddel = new Color(192f,0f,0f,1f);
	//	public static Color yellowLight = new Color(128f,0f,0f,1f);
	//	
	//	public static Color blueDark = new Color(255f,0f,0f,1f);
	//	public static Color blueMiddel = new Color(192f,0f,0f,1f);
	//	public static Color blueLight = new Color(128f,0f,0f,1f);
	
	public static Color32 redDark = new Color32(255,0,0,255);
	public static Color32 redDark32 = new Color32((byte)255, (byte)0, (byte)0, (byte)255);
	public static Color32 redMiddel = new Color32(192,0,0,255);
	public static Color32 redLight = new Color32(128,0,0,255);
	
	public static Color32 greenDark = new Color32(90,146,0,255);
	public static Color32 greenMiddel = new Color32(128,178,0,255);
	public static Color32 greenLight = new Color32(165,219,0,255);
	
	public static Color32 yellowDark = new Color32(214,101,0,255);
	public static Color32 yellowMiddel = new Color32(247,158,0,255);
	public static Color32 yellowLight = new Color32(255,211,16,255);
	
	public static Color32 blueDark = new Color32(41,73,132,255);
	public static Color32 blueMiddel = new Color32(57,113,173,255);
	public static Color32 blueLight = new Color32(74,154,214,255);
	
	public static Color32[] redColors = new Color32[] {
		redLight,
		redMiddel,
		redDark };
	public static Color32[] greenColors = new Color32[] {
		greenLight,
		greenMiddel,
		greenDark };
	public static Color32[] yellowColors = new Color32[] {
		yellowLight,
		yellowMiddel,
		yellowDark };
	public static Color32[] blueColors = new Color32[] {
		blueLight,
		blueMiddel,
		blueDark };
	
	//	public static Color32[] referenceColor = new Color32[] {
	//		new Color32 ((byte)128, (byte)0, (byte)0, (byte)255),
	//		new Color32 ((byte)192, (byte)0, (byte)0, (byte)255),
	//		new Color32 ((byte)255, (byte)0, (byte)0, (byte)255) };
	
	
	public static Color32[,] referenceColors = new Color32[,] {
		{
			redLight, redMiddel, redDark
		},
		{
			greenLight, greenMiddel, greenDark
		},
		{
			yellowLight, yellowMiddel, yellowDark
		},
		{
			blueLight, blueMiddel, blueDark
		}
	};
	
	// Verzweigtes Array																					Verzweigtes Arrays!
	public static Color32[][] referenceColorsVerzweigt = new Color32[][] {
		redColors,
		greenColors,
		yellowColors,
		blueColors
	};
	
	
	//	public static Color32[,] referenceColors3 = new Color32[,] {
	//		{
	//			new Color32(255,0,0,255), new Color32(255,0,0,255), new Color32(255,0,0,255)
	//		},
	//		{
	//			new Color32(255,0,0,255), new Color32(255,0,0,255), new Color32(255,0,0,255)
	//		},
	//		{
	//			new Color32(255,0,0,255), new Color32(255,0,0,255), new Color32(255,0,0,255)
	//		},
	//		{
	//			new Color32(255,0,0,255), new Color32(255,0,0,255), new Color32(255,0,0,255)
	//		}
	//	};
	
	// Verzweigtes Array																					Verzweigtes Arrays!
	//	public static Color32[][] referenceColors4 = new Color32[][] {
	//		new Color32[] {
	//			new Color32 ((byte)128, (byte)0, (byte)0, (byte)255),
	//			new Color32 ((byte)192, (byte)0, (byte)0, (byte)255),
	//			new Color32 ((byte)255, (byte)0, (byte)0, (byte)255)
	//		},
	//		new Color32[] {
	//			new Color32 ((byte)128, (byte)0, (byte)0, (byte)255),
	//			new Color32 ((byte)192, (byte)0, (byte)0, (byte)255),
	//			new Color32 ((byte)255, (byte)0, (byte)0, (byte)255)
	//		},
	//		new Color32[] {
	//			new Color32 ((byte)128, (byte)0, (byte)0, (byte)255),
	//			new Color32 ((byte)192, (byte)0, (byte)0, (byte)255),
	//			new Color32 ((byte)255, (byte)0, (byte)0, (byte)255)
	//		}
	//	};
	
	
}
