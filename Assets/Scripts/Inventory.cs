using UnityEngine;
using System;

public class Inventory
{
    public string invSlot1;
    public string invSlot2;
    public string invSlot3;
    public string offHand;
    public string mainHand;

    // Constructor
    public Inventory(string invSlot1, string invSlot2, string invSlot3, string offHand, string mainHand)
    {
        this.invSlot1 = invSlot1;
        this.invSlot2 = invSlot2;
        this.invSlot3 = invSlot3;
        this.offHand = offHand;
        this.mainHand = mainHand;
    }
}
