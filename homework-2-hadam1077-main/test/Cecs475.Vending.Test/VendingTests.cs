using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using FluentAssertions;
using Cecs475.Vending;
using ItemKey = Cecs475.Vending.VendingMachine.ItemKey;

namespace Cecs475.Vending.Test {
	public class Program {
		readonly static ItemKey[] ALL_KEYS = [
			ItemKey.A1, ItemKey.A2, ItemKey.A3, ItemKey.A4, ItemKey.A5, ItemKey.A6,
			ItemKey.B1, ItemKey.B2, ItemKey.B3, ItemKey.B4, ItemKey.B5, ItemKey.B6,
			ItemKey.C1, ItemKey.C2, ItemKey.C3, ItemKey.C4, ItemKey.C5, ItemKey.C6,
		];

		[Fact]
		public void OnlyPrivateMembers() {
			Type t = typeof(VendingMachine);
			// Make sure all fields are private.
			var publicFields = t.GetFields().Select(f => f.Name);
			publicFields.Should().BeEmpty("you should not have any public fields in VendingMachine");

			// Make sure only the allowed properties are public.
			var publicProperties = t.GetProperties().Select(p => p.Name);
			publicProperties.Should().BeEquivalentTo(["SerialNumber", "TotalCash", "NeedsEmptying"]);

			// Make sure only the allowed methods are public, ignoring property getters and methods inherited from Object.
			var publicMethods = t.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).Select(m => m.Name);
			publicMethods.Should().BeEquivalentTo(["AddInventory", "SetPrice", "InventoryOf", "PriceOf", "CanPurchase", "Purchase"]);
		}

		[Fact]
		public void Constructor() {
			VendingMachine m = new(999, 100.0m);
			m.SerialNumber.Should().Be(999);
			m.TotalCash.Should().Be(100.0m);
			m.NeedsEmptying.Should().BeFalse();
		}

		[Fact]
		public void InitialPrices() {
			VendingMachine m = new(12, 0.0m);
			foreach (ItemKey k in ALL_KEYS) {
				m.PriceOf(k).Should().Be(0.0m, "the default price of each item should be $0.00");
			}
		}

		[Fact]
		public void InitialInventories() {
			VendingMachine m = new(12, 0.0m);
			foreach (ItemKey k in ALL_KEYS) {
				m.InventoryOf(k).Should().Be(0, "the default inventory of each item should be 0");
			}
		}

		[Fact]
		public void SetPrice() {
			VendingMachine m = new(12, 0.0m);
			m.SetPrice(ItemKey.B4, 4.92m);
			m.PriceOf(ItemKey.B4).Should().Be(4.92m);
		}

		[Fact]
		public void SetPrices() {
			VendingMachine m = new(12, 0.0m);
			const decimal multiplier = 0.66m;
			for (int i = 0; i < ALL_KEYS.Length; i++) {
				m.SetPrice(ALL_KEYS[i], i * multiplier);
			}
			for (int i = 0; i < ALL_KEYS.Length; i++) {
				m.PriceOf(ALL_KEYS[i]).Should().Be(i * multiplier);
			}
		}

		[Fact]
		public void AddInventory() {
			VendingMachine m = new(12, 0.0m);
			m.AddInventory(ItemKey.B4, 42);
			m.InventoryOf(ItemKey.B4).Should().Be(42);
		}

		[Fact]
		public void AddInventories() {
			VendingMachine m = new(12, 0.0m);
			const int multiplier = 4;
			for (int i = 0; i < ALL_KEYS.Length; i++) {
				m.AddInventory(ALL_KEYS[i], i * multiplier);
			}
			for (int i = 0; i < ALL_KEYS.Length; i++) {
				m.InventoryOf(ALL_KEYS[i]).Should().Be(i * multiplier);
			}
		}

		[Fact]
		public void NotEnoughMoney() {
			VendingMachine m = new(12, 0.0m);
			m.SetPrice(ItemKey.C5, 1.25m);
			m.AddInventory(ItemKey.C5, 5);
			m.CanPurchase(ItemKey.C5, 1.00m).Should().BeFalse("not enough money to buy the item");
		}

		[Fact]
		public void NotEnoughInventory() {
			VendingMachine m = new(12, 0.0m);
			m.SetPrice(ItemKey.C5, 1.25m);
			m.CanPurchase(ItemKey.C5, 2.00m).Should().BeFalse("the item is not in stock");
		}

		[Fact]
		public void PurchaseNoChange() {
			VendingMachine m = new(12, 10.0m);
			m.SetPrice(ItemKey.C5, 1.25m);
			m.AddInventory(ItemKey.C5, 10);
			m.Purchase(ItemKey.C5, 1.25m).Should().Be(0.0m, "there should be no change when purchasing with the exact amount");
			m.InventoryOf(ItemKey.C5).Should().Be(9, "a successful purchase should decrease the inventory count");
			m.TotalCash.Should().Be(11.25m, "a successful purchase should increase the total cash");
			m.NeedsEmptying.Should().BeFalse();
		}

		[Fact]
		public void PurchaseWithChange() {
			VendingMachine m = new(12, 0.0m);
			m.SetPrice(ItemKey.C5, 100.50m);
			m.AddInventory(ItemKey.C5, 10);
			m.Purchase(ItemKey.C5, 120.75m).Should().Be(20.25m, "there should be change when purchasing with excess amount");
			m.InventoryOf(ItemKey.C5).Should().Be(9, "a successful purchase should decrease the inventory count");
			m.TotalCash.Should().Be(100.50m, "a successful purchase should increase the total cash");
			m.NeedsEmptying.Should().BeFalse();
		}

		[Fact]
		public void PurchaseMany() {
			VendingMachine m = new(12, 10.0m);
			m.SetPrice(ItemKey.C5, 1.25m);
			m.AddInventory(ItemKey.C5, 10);
			m.Purchase(ItemKey.C5, 1.25m).Should().Be(0.0m, "there should be no change when purchasing with the exact amount");
			m.Purchase(ItemKey.C5, 1.25m).Should().Be(0.0m, "there should be no change when purchasing with the exact amount");
			m.Purchase(ItemKey.C5, 1.25m).Should().Be(0.0m, "there should be no change when purchasing with the exact amount");
			m.InventoryOf(ItemKey.C5).Should().Be(7, "a successful purchase should decrease the inventory count");
			m.TotalCash.Should().Be(13.75m, "a successful purchase should increase the total cash");
			m.NeedsEmptying.Should().BeFalse();
		}

		[Fact]
		public void AttemptBadPurchaseNoInventory() {
			VendingMachine m = new(12, 0.0m);
			m.SetPrice(ItemKey.C5, 1.00m);
			m.Purchase(ItemKey.C5, 3.00m).Should().Be(3.00m, "there is no inventory, so the full amount should be refunded");
			m.InventoryOf(ItemKey.C5).Should().Be(0, "there was no purchase, so the inventory should not decrease");
			m.TotalCash.Should().Be(0.0m, "there was no purchase, so the total cash should not increase");
			m.NeedsEmptying.Should().BeFalse();
		}

		[Fact]
		public void AttemptBadPurchaseNoMoney() {
			VendingMachine m = new(12, 50.0m);
			m.SetPrice(ItemKey.C5, 1.00m);
			m.AddInventory(ItemKey.C5, 100);
			m.Purchase(ItemKey.C5, 0.99m).Should().Be(0.99m, "there is no inventory, so the full amount should be refunded");
			m.InventoryOf(ItemKey.C5).Should().Be(100, "there was no purchase, so the inventory should not decrease");
			m.TotalCash.Should().Be(50.0m, "there was no purchase, so the total cash should not increase");
			m.NeedsEmptying.Should().BeFalse();
		}

		[Fact]
		public void NeedsEmptyingConstructor() {
			VendingMachine m = new(12, 1000.0m);
			m.NeedsEmptying.Should().BeTrue("the machine starts with $1000 in total cash");
		}

		[Fact]
		public void NeedsEmptyingBigPurchase() {
			VendingMachine m = new(12, 100);
			m.SetPrice(ItemKey.A2, 901.00m);
			m.AddInventory(ItemKey.A2, 1);
			m.Purchase(ItemKey.A2, 901.00m);
			m.NeedsEmptying.Should().BeTrue("machine now has $1001 in total cash");
		}

		[Fact]
		public void NeedsEmptyingManyPurchases() {
			VendingMachine m = new(12, 99.0m);
			m.SetPrice(ItemKey.A2, 100.00m);
			m.AddInventory(ItemKey.A2, 10);
			for (int i = 0; i < 10; i++) {
				m.NeedsEmptying.Should().BeFalse($"machine should only have {i * 100.0m + 99.0m} cash");
				m.Purchase(ItemKey.A2, 100.0m);
			}
			m.NeedsEmptying.Should().BeTrue("machine now has $1099 in total cash");
		}
	}
}
