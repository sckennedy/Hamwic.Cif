using System;

namespace Hamwic.Core.Attributes
{
    /// <inheritdoc />
    /// <summary>
    ///     Place on an enum member to prevent it being included in
    ///     the list of members displayed e.g. in a user selectable list
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class OmitFromCopyProperties : Attribute
    {
    }
}