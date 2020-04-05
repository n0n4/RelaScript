using RelaScript.Objects;
using RelaScript.Printers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RelaScript
{
    public class InputContext
    {
        private Dictionary<string, InputVar> Vars = new Dictionary<string, InputVar>();
        private Dictionary<string, Exline> Funcs = new Dictionary<string, Exline>();
        private Dictionary<string, ILibrary> Libs = new Dictionary<string, ILibrary>();
        private Dictionary<string, InputClass> Classes = new Dictionary<string, InputClass>();
        private Dictionary<string, InputObject> Objects = new Dictionary<string, InputObject>();

        private int AnonLibCount = 0;

        private List<string> ArgList = new List<string>();

        public List<ILibraryProvider> LibraryProviders = new List<ILibraryProvider>();
        public IRandomProvider Random = null;

        public InputContext ParentScope = null;
        public int ScopeId = 0;

        public PrintChannel PrintChannel = null;

        public bool Freed = false;

        // TODO: rewrite all these methods so they don't spam .ToLower all the time

        public InputContext()
        {
            PrintChannel = new PrintChannel();
        }

        public InputContext(InputContext parent, int scopeid)
        {
            ParentScope = parent;
            Random = parent.Random;
            LibraryProviders = parent.LibraryProviders;
            ScopeId = scopeid;
            PrintChannel = parent.PrintChannel;
        }

        public void Free()
        {
            if (!Freed)
            {
                Freed = true;
                
                Vars = null;
                Funcs = null;
                Libs = null;
                Classes = null;
                Objects = null;
                ArgList = null;
                Random = null;
                LibraryProviders = null;
                ParentScope = null;
            }
        }

        public void InsertVar(InputVar var)
        {
            Vars[var.Name] = var;
        }

        public void DefineVar(string name, object value)
        {
            name = name.ToLower();
            if (Vars.ContainsKey(name))
                SetVar(name, value);
            else
                Vars[name] = new InputVar(name, value, ScopeId);
        }

        public bool SetVar(string name, object value, bool originator = true)
        {
            if(originator)
                name = name.ToLower();
            if(!Vars.TryGetValue(name, out InputVar var))
            {
                // if we don't find a var in our own scope, check further up the scope tree
                if (ParentScope != null && ParentScope.SetVar(name, value, false))
                    return true;
                if (originator) // if it isn't found anywhere, define it
                    DefineVar(name, value);
                return false;
            }
            var.Value = value;
            return true;
        }

        public InputVar GetVar(string name, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Vars.TryGetValue(name, out InputVar var))
            {
                if(ParentScope != null)
                {
                    InputVar pvar = ParentScope.GetVar(name, false);
                    if (pvar != null)
                        return pvar;
                }
                if (originator) // not found, return an undefined var (the reason we do this is so that
                    return DefineAndReturnVar(name, null); // new var will work properly, otherwise it finds no name)
                    // ^ I will probably regret this design decision when people have errors from undefined
                    // variables and can't figure out what's going on
                return null;
            }
            return var;
        }

        public bool FreeVar(string name, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Vars.TryGetValue(name, out InputVar var))
            {
                if (ParentScope != null)
                {
                    return ParentScope.FreeVar(name, false);
                }
                return false;
            }
            var.Free();
            Vars[name] = null;
            return true;
        }

        public InputVar DefineAndReturnVar(string name, object value)
        {
            DefineVar(name, value);
            return GetVar(name);
        }
    
        public InputVar SetAndReturnVar(string name, object value)
        {
            SetVar(name, value);
            return GetVar(name);
        }

        public void CompileLine(Exline line)
        {
            line.Compile(this);
        }

        public void DefineFunc(string name, string source)
        {
            name = name.ToLower();
            if (Funcs.ContainsKey(name))
            {
                SetFunc(name, source);
                return;
            }
            //if (Funcs.ContainsKey(name))
            //    throw new Exception("InputContext already contains custom function '" + name + "'.");

            // TODO: this approach cannot handle recursion (or can it?)
            Exline exl = new Exline(source, funcname: name);
            CompileLine(exl);
            Funcs.Add(name, exl);
        }

        public bool SetFunc(string name, string source, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Funcs.TryGetValue(name, out Exline func))
            {
                // if any parent scope has the func, set it there
                // pass originator as false so that if they don't have the func,
                // we won't try to define it in a parent scope.
                if (ParentScope != null && ParentScope.SetFunc(name, source, false))
                    return true;
                // if we are the calling scope, just define the func 
                if(originator)
                    DefineFunc(name, source);
                return false;
            }

            func.ChangeLine(source);
            CompileLine(func);
            return true;
        }

        public void DefineFuncCompiled(string name, CompileContext precompile)
        {
            name = name.ToLower();
            if (Funcs.ContainsKey(name))
            {
                SetFuncCompiled(name, precompile);
                return;
            }
            //if (Funcs.ContainsKey(name))
            //    throw new Exception("InputContext already contains custom function '" + name + "'.");

            // TODO: this approach cannot handle recursion
            Exline exl = new Exline("COMPILED", funcname: name);
            exl.Compile(precompile);
            Funcs.Add(name, exl);
        }

        public bool SetFuncCompiled(string name, CompileContext precompile, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Funcs.TryGetValue(name, out Exline func))
            {
                if (ParentScope != null && SetFuncCompiled(name, precompile, false))
                    return true;
                if(originator)
                    DefineFuncCompiled(name, precompile);
                return false;
            }

            func.ChangeLine("COMPILED");
            func.Compile(precompile);
            return true;
        }

        public void DefineFuncFromExline(string name, Exline line)
        {
            name = name.ToLower();
            // TODO: don't support redefining an existing function this way currently
            Exline ex = new Exline("COMPILED", funcname: name);
            ex.Compile(line, this);
            Funcs.Add(name, ex);
        }

        public Exline GetFunc(string name, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Funcs.TryGetValue(name, out Exline func))
            {
                if (ParentScope != null)
                {
                    Exline pfunc = ParentScope.GetFunc(name, false);
                    if (pfunc != null)
                        return pfunc;
                }
                return null;
            }
            return func;
        }

        public void SetLibrary(string asname, ILibrary lib)
        {
            // note: libraries are currently unlike vars/funcs
            //       with respect to scoping. If you set a library in 
            //       one scope, it will only affect that scope and its children
            //       (unlike vars, where if you set a var that's in a parent scope
            //       the change affects the original var in the parent scope)
            // TODO: decide if this is what we want to happen?
            //       or should they function like vars/funcs?
            asname = asname.ToLower();

            // special case: if anon, add the functions to the context itself
            if(asname == "anon")
            {
                lib.Inject(this, GenerateAnonLibraryName());
                AnonLibCount++;
                return;
            }

            if (!Libs.ContainsKey(asname))
            {
                Libs.Add(asname, lib);
            }
            else
            {
                Libs[asname] = lib;
            }
        }

        private string GenerateAnonLibraryName()
        {
            return "l:anonlib" + AnonLibCount;
        }

        public ILibrary GetLibrary(string asname, bool originator = true)
        {
            if (originator)
                asname = asname.ToLower();
            if (!Libs.TryGetValue(asname, out ILibrary lib))
            {
                if(ParentScope != null)
                {
                    ILibrary plib = ParentScope.GetLibrary(asname, false);
                    if (plib != null)
                        return plib;
                }
                return null;
            }
            return lib;
        }

        public InputClass DefineClass(string name, InputContext defn)
        {
            name = name.ToLower();
            if (Classes.ContainsKey(name))
            {
                // TODO: should we allow this behavior? I think it's technically possible
                throw new Exception("Tried to redefine class '" + name + "'");
            }
            InputClass nc = new InputClass(name, defn);
            Classes.Add(name, nc);
            return nc;
        }

        public bool SetClass(string name, InputClass defn, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Classes.TryGetValue(name, out InputClass c))
            {
                // if we don't find a var in our own scope, check further up the scope tree
                if (ParentScope != null && ParentScope.SetClass(name, defn, false))
                    return true;
                if (originator) // if it isn't found anywhere, define it
                    Classes[name] = defn;
                return false;
            }
            Classes[name] = defn;
            return true;
        }

        public InputClass GetClass(string name, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Classes.TryGetValue(name, out InputClass c))
            {
                if (ParentScope != null)
                {
                    InputClass pc = ParentScope.GetClass(name, false);
                    if (pc != null)
                        return pc;
                }
                return null;
            }
            return c;
        }

        public InputObject DefineObject(string name, string className, object[] initArgs)
        {
            // first, try to find this class
            className = className.ToLower();
            InputClass c = GetClass(className);
            if (c == null)
                throw new Exception("Class '" + className + "' not found when constructing object '" + name + "'.");

            name = name.ToLower();
            InputObject o = c.PrintObject(name);
            if(name != "anon")
                Objects[name] = o;

            // if it has an initializer, run it now
            if (o.Implementation.Funcs.TryGetValue("f:init", out Exline init))
                init.Execute(initArgs);

            return o;
        }

        public bool SetObject(string name, InputObject obj, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Objects.TryGetValue(name, out InputObject var))
            {
                // if we don't find a var in our own scope, check further up the scope tree
                if (ParentScope != null && ParentScope.SetObject(name, obj, false))
                    return true;
                if (originator) // if it isn't found anywhere, define it
                    Objects[name] = obj;
                return false;
            }
            Objects[name] = obj;
            return true;
        }

        public InputObject GetObject(string name, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Objects.TryGetValue(name, out InputObject c))
            {
                if (ParentScope != null)
                {
                    InputObject pc = ParentScope.GetObject(name, false);
                    if (pc != null)
                        return pc;
                }
                return null;
            }
            return c;
        }

        public bool FreeObject(string name, bool originator = true)
        {
            if (originator)
                name = name.ToLower();
            if (!Objects.TryGetValue(name, out InputObject c))
            {
                if (ParentScope != null)
                {
                    return ParentScope.FreeObject(name, false);
                }
                return false;
            }
            c.Free();
            Objects[name] = null;
            return true;
        }

        public void CopyFull(InputContext target)
        {
            CopyVars(target);
            CopyFuncs(target);
            CopyLibraries(target);
            CopyClasses(target);
            CopyObjects(target);
        }

        public void CopyVars(InputContext target)
        {
            foreach(var kvp in Vars)
            {
                target.DefineVar(kvp.Key, kvp.Value.Value);
            }
        }

        public void CopyFuncs(InputContext target)
        {
            foreach(var kvp in Funcs)
            {
                target.DefineFuncFromExline(kvp.Key, kvp.Value);
            }
        }

        public void CopyLibraries(InputContext target)
        {
            foreach(var kvp in Libs)
            {
                target.SetLibrary(kvp.Key, kvp.Value);
            }
        }

        public void CopyClasses(InputContext target)
        {
            foreach (var kvp in Classes)
            {
                target.SetClass(kvp.Key, kvp.Value);
            }
        }

        public void CopyObjects(InputContext target)
        {
            foreach (var kvp in Objects)
            {
                target.SetObject(kvp.Key, kvp.Value);
            }
        }

        public int PushArg(string name)
        {
            ArgList.Add(name);
            return ArgList.Count - 1;
        }

        public int GetArg(string name)
        {
            for(int i = 0; i < ArgList.Count; i++)
            {
                if(ArgList[i] == name)
                {
                    return i;
                }
            }
            //throw new Exception("Arg '" + name + "' unspecified.");
            // as a convenience... if an arg is unspecified, we push it to the end of the list.
            return PushArg(name);
        }

        public void PopArg()
        {
            ArgList.RemoveAt(ArgList.Count - 1);
        }
    }
}
