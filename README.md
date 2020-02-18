# RelaScript
A scripting language for .NET, compiled with expression trees for high performance.

# Premise
- You're using CSharp or another .NET language
- Your project has a need for a scripting language
- You need scripts to run at high performance once they are loaded

# Strategy
- During runtime, scripts are compiled to .NET IL using lambda expressions
- This approach means we pay a large cost upfront (compilation) 
- In exchange, we get fast runtime thereafter (close or equal to compiled C#, for instance)
- This strategy is highly applicable when we can afford loading screens but need high in-process performance
- In other words, the benefits are more pronounced the more times you expect to run a given script

# The Language
- Lightweight: semi-colons, tabs, new lines, etc, are all unnecessary.
- Implicit Returns: the last statement in a block is the resulting value of that block.
- Type Loose: vars can be assigned values of differing types. Functions can return different types on different code paths.
  - This isn't a design goal per-se, I would like to introduce type-safety in the future.
- Function Loose: funcs can be re-defined.
- Array Loose: anything can be made into an array by inserting commas.

(WIP: getting started instructions to come)

(WIP: installing vscode extension instructions to come)

(WIP: samples to come)
