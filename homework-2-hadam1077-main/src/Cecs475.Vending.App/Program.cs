using Cecs475.Vending;

internal class Program {
	private static void Main(string[] args) {
		// Construct a vending machine with the serial number 1, and $5.00 of total cash inside.
		VendingMachine machine = new VendingMachine(1, 5.00m);
		// Set up the machine's inventory.
		machine.AddInventory(VendingMachine.ItemKey.A1, 2);
		machine.SetPrice(VendingMachine.ItemKey.A1, 1.00m);
		// The "m" suffix in "1.00m" creates a 'decimal' type value, which 
		// is a *precise* fractional-number type that is best for money amounts.
		// "double"/"float" types are not good for money amounts in a serious application
		// because they are imprecise; your $100.00 bank account is really $99.9999979949959, 
		// which is *not* the same thing.

		Console.WriteLine("Ctrl+C to quit");
		while (true) {
			Console.WriteLine();
			Console.WriteLine($"The machine has ${machine.TotalCash:F2} in cash."); // :F2 prints with 2 digits after the decimal.
			Console.Write("Choose a menu item (A1 through C6): ");
			string? input = Console.ReadLine();
			if (input is null) {
				Console.WriteLine("Could not read from console. Terminating application.");
				return;
			}

			// Parse the string input and convert it to an ItemKey.
			VendingMachine.ItemKey chosenItem = (VendingMachine.ItemKey)Enum.Parse(typeof(VendingMachine.ItemKey), input);

			decimal price = machine.PriceOf(chosenItem);
			Console.WriteLine($"That item is ${price:F2}.");
			Console.Write("How much money do you insert? $");
			input = Console.ReadLine();
			if (input is null) {
				Console.WriteLine("Could not read from console. Terminating application.");
				return;
			}
			decimal cashInput = decimal.Parse(input);

			if (machine.CanPurchase(chosenItem, cashInput)) {
				decimal changeReturned = machine.Purchase(chosenItem, cashInput);
				Console.WriteLine($"You get ${changeReturned:F2} in change.");
			}
			else if (machine.InventoryOf(chosenItem) == 0) {
				Console.WriteLine("Machine is all out of that item, sorry!");
			}
			else {
				Console.WriteLine($"That item costs ${price:F2} but you only inserted ${cashInput:F2}.");
			}
		}
	}
}