using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace SQLStructDiff.Core.Services;

/// <summary>Tipo de mudança de uma linha no diff lado a lado.</summary>
public enum DiffLineKind
{
    Unchanged,
    Inserted,
    Deleted,
    Modified,
    Imaginary
}

/// <summary>Uma linha em um dos lados do diff.</summary>
public sealed record DiffLine(int? Number, string Text, DiffLineKind Kind);

/// <summary>Par de linhas (esquerda = A, direita = B) alinhadas.</summary>
public sealed record DiffRow(DiffLine Left, DiffLine Right);

/// <summary>Gera diff lado a lado estilo WinMerge usando DiffPlex.</summary>
public sealed class DiffEngine
{
    private readonly SideBySideDiffBuilder _builder = new(new Differ());

    public IReadOnlyList<DiffRow> BuildSideBySide(string? textA, string? textB)
    {
        var model = _builder.BuildDiffModel(textA ?? string.Empty, textB ?? string.Empty);
        var rows = new List<DiffRow>();

        int count = Math.Max(model.OldText.Lines.Count, model.NewText.Lines.Count);
        for (int i = 0; i < count; i++)
        {
            var left = i < model.OldText.Lines.Count ? Map(model.OldText.Lines[i]) : Empty();
            var right = i < model.NewText.Lines.Count ? Map(model.NewText.Lines[i]) : Empty();
            rows.Add(new DiffRow(left, right));
        }
        return rows;
    }

    private static DiffLine Empty() => new(null, string.Empty, DiffLineKind.Imaginary);

    private static DiffLine Map(DiffPiece piece)
    {
        var kind = piece.Type switch
        {
            ChangeType.Inserted => DiffLineKind.Inserted,
            ChangeType.Deleted => DiffLineKind.Deleted,
            ChangeType.Modified => DiffLineKind.Modified,
            ChangeType.Imaginary => DiffLineKind.Imaginary,
            _ => DiffLineKind.Unchanged
        };
        return new DiffLine(piece.Position, piece.Text ?? string.Empty, kind);
    }
}
