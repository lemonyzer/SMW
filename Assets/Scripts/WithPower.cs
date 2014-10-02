using UnityEngine;
using System.Collections;

public abstract class WithPower : Item {

	public abstract override void Collecting(GameObject itemGO, PlatformCharacter collector);
	public abstract override void Collected(PlatformCharacter collector, NetworkMessageInfo info);

}
