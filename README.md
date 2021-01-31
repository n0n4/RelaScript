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

# Getting Setup
Reference RelaScript in your project. RelaScript is built against .NET Standard 1.1, so it is compatible with .NET Core 1.0 and up, and .NET Framework 4.5 and up. Then, create an InputContext to run RelaScript within:
```csharp
InputContext context = new InputContext();

// random must be provided if you want to use functions that require random numbers
context.Random = new RandomBasic(0); // replace 0 with your random seed

// load library providers-- this controls what libraries may be imported into scripts
// you may write your own libraries to allow RelaScript to interface with your project
context.LibraryProviders.Add(new LibraryProviderBasic());
```
Once an InputContext is set up, you may load and execute scripts:
```csharp
string scriptText = "a:0 * 2"; // load your script

// an Exline represents a script file
Exline exscript = new Exline(scriptText);

// the Exline must be compiled before it can be run
context.CompileLine(exscript);

// args are passed into the script upon execution
object[] args = new object[] { 2.0 };

object result = exscript.Execute(args);
// result --> 4.0 (double)
```

# VSCode Extension
RelaScript files are saved with the .rela extension. If you install our VSCode extension, you can edit these files in VSCode with proper syntax highlighting. To do so, simply copy the RelaScript.VSCExtension folder in this project to your <user>/.vscode/extensions folder.
  
# Learning RelaScript
- For a whirlwind tour of syntax and features, see our [Intro](https://github.com/n0n4/RelaScript/wiki/Intro).
- More documentation to come on our Wiki.
  
# TODO
- The basic libraries are missing many essential functions
- Support for writing libraries in RelaScript is incomplete
- The compiler needs to output more descriptive errors
- Type safety mechanics are under consideration
