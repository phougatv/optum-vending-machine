# Optum.VendingMachineApp
A console-based vending machine simulation that accepts coins, manages inventory, and dispenses products using a state-driven design.

## 1. Problem
You are tasked with building a vending machine that can accept coins and dispense products.

### 1.1 Accepting coins
As a vendor, I want the vending machine to accept coins so I can collect money from customers.

The machine should only accept valid coins: nickels ($0.05), dimes ($0.10), and quarters ($0.25). Any invalid coins, such as pennies ($0.01), should be rejected and returned to the customer. When a valid coin is inserted, its value should be added to the current balance, and the display should update accordingly. If no coins have been inserted, the display should read "INSERT COIN." Any rejected coins should go to the coin return.

> [!NOTE]
> _While it might seem natural to create Coin objects that store their value, real vending machines actually identify coins by their physical properties like weight and size, then determine their value. You should simulate this approach, perhaps using strings, constants, enums, or similar representations._

### 1.2 Product Selection
As a vendor, I want customers to be able to select products so they have a reason to use the machine.

The vending machine should offer three products: **cola** ($1.00), **chips** ($0.50), and **candy** ($0.65). When a customer presses a product button and has inserted enough money, the machine should dispense the product and display "THANK YOU." Afterward, the display should reset to "INSERT COIN" and the balance should return to $0.00. If the customer hasn’t inserted enough money, the machine should display the price of the selected item. On subsequent checks, the display should show either "INSERT COIN" or the current balance, as appropriate.

## 2. Solution
I have used the State design pattern to organize the vending machine’s behavior. Each state, like no coins (initial state) or selecting a product, is handled by its own class. This makes the code easier to understand, test, and update. Coins are identified by their physical properties, not by value, to better match how real vending machines work.
