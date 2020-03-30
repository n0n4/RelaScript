using RelaScript.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript
{
    /// <summary>
    /// Internal representation of a variable in the input system.
    /// </summary>
    public class InputVar : IEquatable<InputVar>
    {
        public string Name = string.Empty;
        public object Value = 0;
        public int ScopeId = 0; // used for class / object creation

        public InputVar(string name, object value, int scopeid = 0)
        {
            Name = name;
            Value = value;
            ScopeId = scopeid;
        }

        public InputVar SetAndReturn(object value)
        {
            Value = value;
            return this;
        }

        public void Free()
        {
            if(Value != null)
            {
                if(Value is InputObject)
                {
                    (Value as InputObject).Free();
                }
                Value = null;
            }
        }


        // Handle equality
        public override bool Equals(object obj)
        {
            return this.Equals(obj as InputVar);
        }

        public bool Equals(InputVar p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            if(Value == null)
            {
                return p.Value == null;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return Value.Equals(p.Value) && Name == p.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(InputVar lhs, InputVar rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(InputVar lhs, InputVar rhs)
        {
            return !(lhs == rhs);
        }
    }
}
