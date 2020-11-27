# Vending Machine Kata

Worked through the vending machine kata using test driven development from the beginning.

Built in dotnet 5.0, C# 9 with nullable reference types enabled.

There is a console app available to interact with the vending machine.  Ensure ConsoleApp assembly is setup as starting project if it doesn't run automatically.

Based on the kata found here [Vending Machine Kata](https://github.com/guyroyse/vending-machine-kata)

Completed to the point of returning coins

---

### Thoughts on the code

I enjoyed the process of using TDD as it's been a while and this was an interesting exercise. I was strict with myself and everything was written TDD from the off.  The console app was bolted on at the end so I could see if my classes actually worked - I didn't manually test as I went relying purely on unit tests.

My starting point was implementing a USA specific coin detector. This would enable me to switch in different coin detectors should the machine be deployed to a different kata locale.

The use of abstract records for the coins and products I felt was a good choice.  I tend to favour abstract classes over interfaces for data types and with the record type this makes sense as each coin/product would not be changeable and would be immutable at the point of sale.  This was my first use of records and I quite like it as it gives you a definite immutable type as opposed to enforcing it yourself.

The use of the abstract base also enabled the sub classes to define the properties on creation but gave a common polymorphic object by which they could be treated the same throughout any product or coin based operations.

The only part of the solution I wasn't entirely satisfied with was the product dispenser.  I felt it did too much.

A particular issue was the introduction of errors and the returning of change. Initially the out parameter felt like a good fit as the dispenser should be able to return its own errors. The change being returned was what pushed it over the edge.

Now as I write this I think what I should have done was have a common coin manager (or something better named) which was injected into the vending machine and the product dispenser and held that responsibility as a scoped or singleton instance. The Coin Manager would have been able to work out the change and hold the states of held coins whether in the initial hopper or in the coin return as well as understanding the coins inventory should it get that far. The product dispenser could have triggered it on dispense of a product and the vending machine would be able to control flow at the top level as it could query it's state at any time.