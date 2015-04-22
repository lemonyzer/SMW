#define SDL_LITTLE_ENDIAN
#define SDL_BYTEORDER
//#define SDL_BIG_ENDIAN
#undef SDL_BIG_ENDIAN
//	#define SDL_BYTEORDER = SDL_LITTLE_ENDIAN

using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using System;
using System.IO;

public class FileIO {


	public static bool ReadBool(BinaryReader binReader)
	{
		bool b;
		//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadBoolean();
		
		return b;
	}
	
	public static short ReadByteAsShort(BinaryReader binReader)
	{
		byte b;
		//		char b;
		
		//		fread(&b, sizeof(Uint8), 1, inFile);
		b = binReader.ReadByte();
		//		Debug.LogWarning(b.ToString());
		return (short)b;
	}
	
	/// <summary>
	/// Reads the int.
	/// </summary>
	/// <returns>The int.</returns>
	/// <param name="inFile">In file.</param>
	public static int ReadInt(BinaryReader binReader)
	{
		int inValue;
		//		fread(&inValue, sizeof(Uint32), 1, inFile);
		inValue = (int) binReader.ReadUInt32();
		
		#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
		// kopiere value zum bearbeiten der byte reihenfolge
		int t = inValue;
		
		inValue = (int) ReverseBytes((UInt32)t);
		
		//		((char*)&inValue)[0] = ((char*)&t)[3];
		//		((char*)&inValue)[1] = ((char*)&t)[2];
		//		((char*)&inValue)[2] = ((char*)&t)[1];
		//		((char*)&inValue)[3] = ((char*)&t)[0];
		#endif
		
		return inValue;
	}
	
	
	/// <summary>
	/// Reads the int chunk. (Datenblock)
	/// </summary>
	#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
	public static void ReadIntChunk(int[] mem, uint iQuantity, BinaryReader binReader)
	{
		for(uint i=0; i<iQuantity; i++)
		{
			mem[i] = (int) binReader.ReadUInt32();
			
			// kopiere value
			int t = mem[i];
			
			// Reverse Byte Order - reordner the 4 bytes in Integer (32 bit)
			mem[i] = (int) ReverseBytes((uint)t);
		}
	}
	
	// reverse byte order (32-bit)
	public static UInt32 ReverseBytes(UInt32 value)
	{
		return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
			(value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
	}
	
	#else
	public static void ReadIntChunk(int[] mem, uint iQuantity, BinaryReader binReader)
	{
		//		fread(mem, sizeof(Uint32), iQuantity, inFile);
		for(uint i=0; i<iQuantity; i++)
		{
			mem[i] = (int) binReader.ReadUInt32();
		}
	}
	#endif
	
	public static float ReadFloat(BinaryReader binReader)
	{
		//TODO ready ReadBytes(4), vielleicht konvertiert ReadSingle bereits falsch
		float inValue = binReader.ReadSingle();			// float ReadSingle()
		//		fread(&inValue, sizeof(float), 1, inFile);
		
		#if (SDL_BYTEORDER == SDL_BIG_ENDIAN)
		float t = inValue;
		
		inValue = (float) ReverseBytes((UInt32)t);
		#endif
		
		return inValue;
	}
	
	
	public static string ReadString(uint size, BinaryReader binReader)
	{
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
		//		Debug.Log("iLen = " + iLen + " --> char[] cstring = new char["+iLen+"]; --> arraylänge mit NULL Terminator");
		
		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return null;
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return null;
		}
		
		//		char * szReadString = new char[iLen];
		char[] szReadCString = new char[iLen];
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadCString = binReader.ReadChars(iLen);
		
		//		szReadString[iLen - 1] = 0;
		//		szReadCString[iLen - 1] = '\0';	 //cstring NULL Terminated ACHTUNG  BUG -> string wird dann null terminiert!!
		
		string[] debugString = new string[2];
		for(int i=0; i<iLen; i++)
		{
			debugString[0] += i +" ";
			debugString[1] += szReadCString[i] +" ";
		}
		debugString[0] += "|";
		debugString[1] += "|";
		Debug.LogError(iLen + "\n" + debugString[0] + "\n" + debugString[1]);
		
		//		szReadString[iLen - 1] = 0;
		//		szReadString[iLen - 1] = '\0';	 cstrin NULL Terminated ACHTUNG  BUG -> string wird dann null terminiert!!
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		
		string readString = new string(szReadCString).Trim('\0');		// WICHTIG entferne NULL Terminierung
		
		//		Debug.Log("readString = " + readString);
		
		return readString;
	}
	
	
	//	void ReadString(char * szString, short size, FILE * inFile)
	public static void ReadString(char[] szString, uint size, BinaryReader binReader)
	{
		Debug.LogError(" DON'T USE ME");
		
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		int iLen = ReadInt(binReader);
		Debug.Log("iLen = " + iLen + " (cstring länge)");
		
		if(iLen < 0)
		{
			Debug.LogError("string länge < 0!");
			return;
		}
		else if (iLen > Globals.TILESET_TRANSLATION_CSTRING_SIZE)
		{
			Debug.LogError("string länge > max. länge (" + Globals.TILESET_TRANSLATION_CSTRING_SIZE + ") ");
			return;
		}
		
		//		char * szReadString = new char[iLen];
		char[] szReadString = new char[iLen];
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadChars(iLen);
		
		//		szReadString[iLen - 1] = 0;
		szReadString[iLen - 1] = '\0';	//TODO check string/char line end in cpp 
		
		//Prevent buffer overflow  5253784 5253928
		//		strncpy(szString, szReadString, size - 1);		// -> size = TILESET_TRANSLATION_CSTRING_SIZE
		//		szString[size - 1] = 0;
		//TODO NOTE: szString hat im Struct eine länge von 128, nicht über disen Speicherbereich hinaus schreiben!
		/* copy to sized buffer (overflow safe): */ 
		//strncpy ( str2, str1, sizeof(str2) );
		szString = szReadString;					//TODO TODO szString zeigt auf die selbe reference
		szString = new char[iLen];					//TODO TODO szString muss eine eigene reference haben, nur der inhalt soll kopiert werden
		Array.Copy(szReadString, szString, iLen);	// char Array kopieren
		string test = new string(szString);			//TODO löscht diese anweisung den Inhalt aus szString?
		string test2 = new string(szString);		//TODO löscht diese anweisung den Inhalt aus szString?
		//		string test3 = string.Join("", szString);	//TODO löscht diese anweisung den Inhalt aus szString?
		//		string charToString = new string(CharArray, 0, CharArray.Count());
		Debug.Log("szString = " + new string(szString));	//TODO NEIN: Inhalt noch vorhanden
		Debug.Log("szString = " + test);	
		Debug.Log("szString = " + test2);	
		//		Debug.Log("szString = " + test3);	
		//		delete [] szReadString;
	}
	
	public static string ReadNativString(uint size, BinaryReader binReader)
	{
		// Funktioniert mit dieser Dateistruktur NICHT,
		// in der Datei steht ein 32-bit langer Integer-Wert
		// BinaryReader.ReadString() erwartet einen 7-Bit langen Interger-Wert
		
		string szString;
		// string länge auslesen
		//		int iLen = ReadInt(inFile);
		// TODO BinaryReader.ReadString() erwartet als erste Information die Stringlänge
		//		int iLen = ReadInt(binReader);
		//		Debug.Log("iLen = " + iLen + " (string länge)");
		
		
		//		char * szReadString = new char[iLen];
		string szReadString ;
		
		//		fread(szReadString, sizeof(Uint8), iLen, inFile);
		szReadString = binReader.ReadString();	// TODO achtung was macht es?
		
		szString = szReadString;
		return szString;
	}

}
