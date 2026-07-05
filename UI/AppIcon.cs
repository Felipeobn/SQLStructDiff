namespace SQLStructDiff.UI;

/// <summary>
/// Ícone da aplicação para as janelas. Carrega o ícone embutido no executável
/// (definido por &lt;ApplicationIcon&gt; no csproj), em cache.
/// </summary>
internal static class AppIcon
{
    private static Icon? _icon;
    private static bool _tried;

    /// <summary>Ícone do app, ou null se não for possível carregá-lo (ex.: design-time).</summary>
    public static Icon? Value
    {
        get
        {
            if (_tried) return _icon;
            _tried = true;
            try
            {
                var exe = Environment.ProcessPath;
                if (!string.IsNullOrEmpty(exe))
                    _icon = Icon.ExtractAssociatedIcon(exe);
            }
            catch
            {
                _icon = null; // não impede a UI de abrir
            }
            return _icon;
        }
    }
}
