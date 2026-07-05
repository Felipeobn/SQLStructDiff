namespace SQLStructDiff.Core.Models;

/// <summary>Parâmetros que controlam como a comparação é feita.</summary>
public sealed class ComparisonParameters
{
    /// <summary>
    /// Quando true, a ordem dos campos importa: tabelas com as mesmas colunas
    /// em ordem diferente são consideradas DIFERENTES.
    /// Quando false, a ordem é ignorada: se os campos existem em ambos (mesmo
    /// que em ordem diferente), o objeto é considerado IGUAL.
    /// </summary>
    public bool ConsiderColumnOrder { get; set; } = true;

    /// <summary>
    /// Quando true (padrão), permite gerar script nos dois sentidos (A→B e B→A).
    /// Quando false (unidirecional), só é possível gerar do banco da esquerda (A)
    /// para o da direita (B).
    /// </summary>
    public bool Bidirectional { get; set; } = true;
}
