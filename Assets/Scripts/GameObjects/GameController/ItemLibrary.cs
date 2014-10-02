using UnityEngine;
using System.Collections;

public class ItemLibrary : MonoBehaviour {

	public static Item[] items = new Item[2] {new Star(), new Flower()};

	public static int starID = 0;
	public static int flowerID = 1;

	/**
	 * GameSettings: allow/disable items spawn...
	 * 
	 * 
	 **/

	void Reset()
	{
//		items = new Item[2];
//		items[starID] = new Star();
//		items[flowerID] = new Flower();
	}

	public static Item getItem(int itemId)
	{
		return items[itemId];
	}

}
