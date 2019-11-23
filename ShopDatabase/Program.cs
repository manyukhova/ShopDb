﻿using ShopDatabase.DbContext;
using ShopDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Food> groceries = new List<Food>
            {
                new Food("banana", 0.99),
                new Food("orange", 1.29),
                new Food("tomato", 0.98),
                new Food("apple", 0.55),
                new Food("cucumber", 1.7),
                new Food("hurma", 1.49),
                new Food("sushi", 12.5)

            };
            

            using (var db = new ShopDbContext())
            {
                
                List<Food> foods = db.Foods.ToList();
                foreach(var food in groceries)
                {
                    bool exists = false;
                    if (foods.FirstOrDefault(x => x.Name == food.Name) != null)
                        exists = true;
                    if(!exists)
                        db.Foods.Add(food);
                };
                db.SaveChanges();

                Console.WriteLine("Please enter your first name!");
                string firstname = Console.ReadLine();

                Console.WriteLine($"Hi, {firstname}! Please enter your last name!");
                string lastname = Console.ReadLine();

                Person client = db.Persons.FirstOrDefault(x => x.FirstName.ToLower() == firstname.ToLower() && x.LastName.ToLower() == lastname.ToLower());
                if (client == null)
                {
                    Console.WriteLine("Hello! You are our new client");
                    client = new Person(firstname, lastname);
                    db.Persons.Add(client);
                }
                else
                {
                    Console.WriteLine("Hello! You are existing client");
                }
               
                    
               
                Console.WriteLine($"Welcome to our shop, {client}!");
                ShoppingCart shoppingCart = new ShoppingCart(client);

                while (true)
                {
                    
                    Console.WriteLine($"What do you want to buy?");

                    ChooseFood(groceries, shoppingCart);


                   Console.WriteLine("Something else? Y/N");

                    if (Console.ReadLine().ToLower() != "Y")
                        break;
                }
                db.ShoppingCarts.Add(shoppingCart);
                db.SaveChanges();

                var people = db.Persons;
                foreach(var person in people)
                {
                    person.PrintItems(db);
                    Console.WriteLine("*********************************************************");
                    
                }
                Console.WriteLine($"Thanks for visiting our shop, {client}!");
                Console.ReadKey();
                
            }

             
        }

        public static void ChooseFood(List<Food> groceries, ShoppingCart cart)
        {

            string food = Console.ReadLine();
            Food chosenFood = groceries.FirstOrDefault(x => x.Name == food);

            if (chosenFood == null)
            {
                Console.WriteLine($"Excuse me, we don´t have {food} in our shop.");
            }
            else
            {
                Console.WriteLine($"How much would you like?");
                while (true)
                {
                    if (double.TryParse(Console.ReadLine(), out double amount))
                    {
                        ItemsInCart item = cart.Items.FirstOrDefault(x => x.Item == chosenFood);
                        if (item == null)
                            cart.Items.Add(new ItemsInCart(cart, chosenFood, amount));
                        else
                            item.amount += amount;
                        break;
                    }
                    else
                        Console.WriteLine("Write a number!");
                }
            }
        }
    }
}
