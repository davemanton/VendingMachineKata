using System;
using System.Linq;
using Application;
using static System.Console;

namespace ConsoleApp
{
    public class VendingApplication
    {
        private readonly IVendingMachine _vendingMachine;

        public VendingApplication(IVendingMachine vendingMachine)
        {
            _vendingMachine = vendingMachine;
        }

        public void Run()
        {
            string option;

            do
            {
                DisplayOptions();
                WriteLine("Pick an option, or quit/exit, ? to re-list: ");
                option = ReadLine();

                switch (option)
                {
                    case "1":
                        CheckDisplay();
                        break;

                    case "2":
                        InsertCoin();
                        break;

                    case "3":
                        SelectProduct();
                        break;

                    case "4":
                        CheckCoinReturn();
                        break;

                    case "5":
                        CheckDispenseBox();
                        break;

                    case "?":
                        DisplayOptions();
                        break;

                    case "cls":
                        Clear();

                        break;
                }
            } while (!string.IsNullOrWhiteSpace(option)
                  && option != "quit"
                  && option != "exit");
        }

        

        private void DisplayOptions()
        {
            WriteLine();
            WriteLine("(1) Check Display");
            WriteLine("(2) Insert Coin");
            WriteLine("(3) Select Product");
            WriteLine("(4) Check Coin Return");
            WriteLine("(5) Check Product Hopper");
            WriteLine();
        }

        private void CheckDisplay()
        {
            var display = _vendingMachine.Display();

            WriteLine();
            WriteLine(display);
        }

        private void InsertCoin()
        {
            WriteLine();
            WriteLine("Insert a coin - nickel, dime or quarter or try your luck");
            WriteLine();

            var coin = ReadLine();
            _vendingMachine.InsertCoin(coin ?? string.Empty);

            CheckDisplay();
        }

        private void SelectProduct()
        {
            WriteLine();
            WriteLine("(a) Cola,  $1.00");
            WriteLine("(b) Chips, $0.50");
            WriteLine("(c) Candy, $0.65");
            WriteLine("(x) Go Back");
            WriteLine();

            var selection = ReadLine();

            if (selection?.ToLowerInvariant() == "x")
                return;

            _vendingMachine.SelectProduct(selection ?? string.Empty);

            CheckDisplay();
        }

        private void CheckCoinReturn()
        {
            WriteLine();
            WriteLine("In the coin return there is...");

            var returnedCoins = _vendingMachine.CheckCoinReturn();

            WriteLine(returnedCoins.Any() ? string.Join(", ", returnedCoins) : "...nothing");
        }

        private void CheckDispenseBox()
        {
            WriteLine();
            WriteLine("In the product dispenser there is...");

            var dispensedProducts = _vendingMachine.CheckDispenserHopper();

            WriteLine(dispensedProducts.Any() ? string.Join(", ", dispensedProducts) : "...nothing");
        }
    }
}