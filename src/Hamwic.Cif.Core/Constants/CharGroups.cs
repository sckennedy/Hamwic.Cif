using System;

namespace Hamwic.Cif.Core.Constants
{
    [Flags]
    public enum CharGroups
    {
        UpperCaseLetters = 1,
        LowerCaseLetters = 2,
        Letters = LowerCaseLetters | UpperCaseLetters,
        Numbers = 4,
        LettersAndNumbers = Numbers | Letters,
        SpecialCharacters = 8,
        All = SpecialCharacters | LettersAndNumbers,
    }
}