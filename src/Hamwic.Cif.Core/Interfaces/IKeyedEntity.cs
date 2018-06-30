using System;

namespace Hamwic.Cif.Core.Interfaces
{
    public interface IKeyedEntity<T> : IEquatable<IKeyedEntity<T>>
    {
        T Id { get; }
        bool IsNew { get; }
    }
}