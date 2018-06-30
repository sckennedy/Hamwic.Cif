using System;

namespace Hamwic.Cif.Core.Attributes
{
    /// <summary>
    ///     Place on an enum member to prevent it being included in
    ///     the list of members displayed e.g. in a user selectable list
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class OmitFromReflectedListAttribute : Attribute
    {
    }
}