using UnityEngine;
using System.Collections;

public class ItemLibrary : MonoBehaviour {

	
	public static int starID = 0;
	public static int flowerID = 1;
	public static int bombID = 2;
	public static int icewandID = 3;

	public static Item[] items = new Item[4] {
		new StarItem(starID),
		new FlowerItem(flowerID),
		new BombItem(bombID),
		new IceWandItem(icewandID)};

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
