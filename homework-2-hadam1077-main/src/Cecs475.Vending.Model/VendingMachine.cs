using System;
using System.Collections.Generic;

namespace Cecs475.Vending {
	public class VendingMachine {
		public enum ItemKey { //lists refrences of all item keys in order
			A1, A2, A3, A4, A5, A6, 
			B1, B2, B3, B4, B5, B6, 
			C1, C2, C3, C4, C5, C6
		}
		public int SerialNumber { get; } // refrences public variable serial number 
    public decimal TotalCash { get; private set; } //refrences pubnlic vairable total cash
    public bool NeedsEmptying => TotalCash >= 1000; // refrences public variable needs emptyinh when exceeds 1000
// these var must be public because they are accessed by the user 
    private Dictionary<ItemKey, decimal> itemPrices; //maps item key to item price
    private Dictionary<ItemKey, int> itemInventory; //maps item key to item inventory
//these var must be private because the user does not need ot modify them
	 // constructor to make the vending machine class
    public VendingMachine(int serialNumber, decimal totalCash) {//parameters serial number and total cash to initalize later in code
        SerialNumber = serialNumber;
        TotalCash = totalCash;
        itemPrices = new Dictionary<ItemKey, decimal>();
        itemInventory = new Dictionary<ItemKey, int>();
// sets prices and inventory to a new dictionary for the loop

        foreach (ItemKey key in Enum.GetValues(typeof(ItemKey))) {// loop iterates over each key in the itemkey(a1--c6) and sets price to 0 and inventory to 0
            itemPrices[key] = 0.00m;  
            itemInventory[key] = 0;   
        }//set these to 0 so when we add using setrpice and add invenotry we have a clean state
    }
	public void SetPrice(ItemKey key, decimal price) {// uses key and price to set price
    itemPrices[key] = price;
}
public void AddInventory(ItemKey key, int amount) {// uses key and amount to set amount 
    itemInventory[key] += amount;
}
public decimal PriceOf(ItemKey key) { // returns price using key
    return itemPrices[key];
}

public int InventoryOf(ItemKey key) {// returns inventory using key
    return itemInventory[key];
}
public bool CanPurchase(ItemKey key, decimal cash) {// takes item key applies to ibentory and price, checks if inventory greater than 0 AND cash greater or equal to price 
    
     return itemInventory[key] > 0 && cash >= itemPrices[key];
}
public decimal Purchase(ItemKey key, decimal cash) { // if block to check if canpurchase will run, if so subract item from inventory, add the price of the item to total cash, and returns your cash minus the item price
   if (CanPurchase( key, cash)){
    itemInventory[key] --;
    TotalCash += itemPrices[key];
    return cash - itemPrices[key];


   }
    else return cash;  // if purchase is not allowed, return the full cash input
}


	}
}