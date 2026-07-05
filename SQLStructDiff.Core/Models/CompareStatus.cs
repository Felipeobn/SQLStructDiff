namespace SQLStructDiff.Core.Models;

/// <summary>Resultado da comparação de um objeto entre os bancos A e B.</summary>
public enum CompareStatus
{
    /// <summary>Existe nos dois e é idêntico.</summary>
    Equal,

    /// <summary>Existe nos dois, porém com diferenças.</summary>
    Different,

    /// <summary>Existe apenas no banco A.</summary>
    OnlyInA,

    /// <summary>Existe apenas no banco B.</summary>
    OnlyInB
}
