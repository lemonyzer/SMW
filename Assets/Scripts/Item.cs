using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Item {

	// get only (read only) == field
	// get and set 			== propertie

//	public abstract PlatformCharacter collector { get;set; }
//	public abstract int itemId { get;set; }
//	public abstract Power powerScript { get;set; }
//	public abstract string powerScriptName { get;set; }





	// geerbte Methoden
	// statisch: nicht polymorph!!!!
	// in Superclasse vollständig implementiert
	// in Subclass mit new überschreiben

	/**
	 * POLYMORPHIE
	 **/

	// dynamisch: POLYMORPHIE
	// abstract und virtual

	// abstract
	// In abstract Superclasse: abstract method, nur methoden header kein code.
	// In Subclass: override

	// virtual
	// In Superclass: public virtual method, method kann code enthalten!
	// In Subclass: erben oder überschreiben mit new, override
	// Sie werden sich an dieser Stelle wahrscheinlich fragen, worin sich die beiden letztgenannten Varianten unterscheiden.
	// Diese Überlegung führt uns nach der Datenkapselung und der Vererbung zum dritten elementaren Konzept der
	// Objektorientierung: zur Polymorphie. Ehe wir uns aber mit der Polymorphie beschäftigen, müssen Sie vorher noch die
	// Typumwandlung in einer Vererbungshierarchie verstehen.


    /**
     * Mit dem Modifizierer new können die aus der Basisklasse geerbten Methoden in der ableitenden Klasse überdeckt werden.
     * Allerdings ist dieser Lösungsweg mit einem Nachteil behaftet, denn er garantiert nicht, dass alle ableitenden Klassen
     * die geerbten Methoden Starten und Landen überdecken und durch eine typspezifische Implementierung ersetzen.
     * Jede unserer abgeleiteten Klassen sollte aber hinsichtlich der Behandlung einer Basisklassenoperation gleichwertig sein.
     * Wird die Neuimplementierung beispielsweise in der Klasse Hubschrauber vergessen, ist dieser Typ mit einem möglicherweise
     * entscheidenden Fehler behaftet, weil er keine typspezifische Neuimplementierung hat.
     * 
     * Wie können wir aber alle Klassen, die von der Klasse Luftfahrzeug abgeleitet werden, dazu zwingen, die Methoden Starten 
     * und Landen neu zu implementieren? Gehen wir noch einen Schritt weiter, und stellen wir uns die Frage, ob wir überhaupt 
     * dann noch Code in den Methoden Starten und Landen der Klasse Luftfahrzeug benötigen. Anscheinend nicht. Dass wir die 
     * beiden Methoden in der Basisklasse definiert haben, liegt im Grunde genommen nur daran, dass wir diese Methoden in den 
     * ableitenden Klassen bereitstellen wollen.
     * 
     * Mit dieser Erkenntnis mag die Lösung der aufgezeigten Problematik im ersten Moment verblüffen: Tatsächlich werden Starten 
     * und Landen in der Basisklasse nicht implementiert – sie bleiben einfach ohne Programmcode. Damit wäre aber noch nicht 
     * sichergestellt, dass die Subklassen die geerbte »leere« Methode typspezifisch implementieren. Deshalb wird in solchen 
     * Fällen sogar auf den Anweisungsblock verzichtet, der durch die geschweiften Klammern beschrieben wird.
     * 
     * In der objektorientierten Programmierung werden Methoden, die keinen Anweisungsblock aufweisen, als abstrakte Methoden 
     * bezeichnet. Neben den Methoden, die das Verhalten eines Typs beschreiben, können auch Eigenschaften als abstrakt definiert werden.
     * 
     * Abstrakte Methoden werden durch die Angabe des abstract-Modifizierers in der Methodensignatur gekennzeichnet, am 
     * Beispiel unserer Methoden Starten und Landen also durch:
     **/

	public abstract void Collecting(GameObject itemGO, PlatformCharacter collector);	// Serverseitig (authoritative)
	public abstract void Collected(PlatformCharacter collector, NetworkMessageInfo info);	// Server- und Clientseitig (RPCMode.All)

}
