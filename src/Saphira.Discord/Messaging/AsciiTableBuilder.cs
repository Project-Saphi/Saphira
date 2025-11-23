using System.Text;

namespace Saphira.Discord.Messaging;

public class AsciiTableBuilder
{
    private readonly List<string> _headers = [];
    private readonly List<List<string>> _rows = [];
    private readonly List<ColumnAlignment> _alignments = [];
    private char _columnSeparator = '|';
    private char _headerSeparator = '-';
    private bool _wrapInCodeBlock = true;

    public enum ColumnAlignment
    {
        Left,
        Right,
        Center
    }

    public AsciiTableBuilder AddHeader(params string[] headers)
    {
        _headers.AddRange(headers);

        while (_alignments.Count < _headers.Count)
        {
            _alignments.Add(ColumnAlignment.Left);
        }

        return this;
    }

    public AsciiTableBuilder AddRow(params object[] cells)
    {
        var row = cells.Select(c => c?.ToString() ?? "").ToList();

        while (row.Count < _headers.Count)
        {
            row.Add("");
        }

        _rows.Add(row);
        return this;
    }

    public AsciiTableBuilder SetColumnAlignment(int columnIndex, ColumnAlignment alignment)
    {
        if (columnIndex >= 0 && columnIndex < _alignments.Count)
        {
            _alignments[columnIndex] = alignment;
        }
        return this;
    }

    public AsciiTableBuilder SetColumnSeparator(char separator)
    {
        _columnSeparator = separator;
        return this;
    }

    public AsciiTableBuilder SetHeaderSeparator(char separator)
    {
        _headerSeparator = separator;
        return this;
    }

    public AsciiTableBuilder SetWrapInCodeBlock(bool wrap)
    {
        _wrapInCodeBlock = wrap;
        return this;
    }

    public string Build()
    {
        if (_headers.Count == 0)
        {
            return _wrapInCodeBlock ? "```\nNo data\n```" : "No data";
        }

        var columnWidths = CalculateColumnWidths();
        var table = new StringBuilder();

        if (_wrapInCodeBlock)
        {
            table.AppendLine("```");
        }

        table.AppendLine(BuildRow(_headers, columnWidths));
        table.AppendLine(BuildSeparatorLine(columnWidths));

        foreach (var row in _rows)
        {
            table.AppendLine(BuildRow(row, columnWidths));
        }

        if (_wrapInCodeBlock)
        {
            table.Append("```");
        }
        else
        {
            if (table.Length > 0 && table[table.Length - 1] == '\n')
            {
                table.Length--;
            }
        }

        return table.ToString();
    }

    private List<int> CalculateColumnWidths()
    {
        var widths = new List<int>();

        for (int i = 0; i < _headers.Count; i++)
        {
            int maxWidth = _headers[i].Length;

            foreach (var row in _rows)
            {
                if (i < row.Count)
                {
                    maxWidth = Math.Max(maxWidth, row[i].Length);
                }
            }

            widths.Add(maxWidth);
        }

        return widths;
    }

    private string BuildRow(List<string> cells, List<int> columnWidths)
    {
        var row = new StringBuilder();
        row.Append(_columnSeparator);

        for (int i = 0; i < cells.Count; i++)
        {
            row.Append(' ');

            var cell = cells[i];
            var width = columnWidths[i];
            var alignment = i < _alignments.Count ? _alignments[i] : ColumnAlignment.Left;

            row.Append(AlignText(cell, width, alignment));
            row.Append($" {_columnSeparator}");
        }

        return row.ToString();
    }

    private string BuildSeparatorLine(List<int> columnWidths)
    {
        var separator = new StringBuilder();
        separator.Append(_columnSeparator);

        for (int i = 0; i < columnWidths.Count; i++)
        {
            separator.Append(_headerSeparator);
            separator.Append(new string(_headerSeparator, columnWidths[i]));
            separator.Append($"{_headerSeparator}{_columnSeparator}");
        }

        return separator.ToString();
    }

    private string AlignText(string text, int width, ColumnAlignment alignment)
    {
        return alignment switch
        {
            ColumnAlignment.Right => text.PadLeft(width),
            ColumnAlignment.Center => PadCenter(text, width),
            _ => text.PadRight(width)
        };
    }

    private string PadCenter(string text, int width)
    {
        int spaces = width - text.Length;
        int padLeft = spaces / 2 + text.Length;
        return text.PadLeft(padLeft).PadRight(width);
    }
}
