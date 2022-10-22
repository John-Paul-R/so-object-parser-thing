namespace ObjectParserThing;

public class ObjectParserThing
{
    public List<object> ParseFiles(string directoryPath)
    {
        string[] paths = Directory.GetFiles(directoryPath, "*.txt");

        List<object> rawResults = new();
        foreach (string filePath in paths) {
            rawResults.Add(ParseFile(filePath));
        }

        return rawResults;
    }

    public object ParseFile(string filePath)
    {
        using StreamReader sr = File.OpenText(filePath);

        return ParseObject(sr);
    }

    public enum ParserState
    {
        ObjectOrList,
        Object,
        List,
    }

    // Strip any trailing comments. This will catch '#' chars inside a quoted value,
    // which is probably bad, but I'm not solving that rn.
    private static string StripComments(string line) { int idx = line.IndexOf('#'); return idx != -1 ? line[..idx] : line; }

    private static (string Key, string Value) GetLineKeyValue(string line)
    {
        var cleanedLine = StripComments(line);

        var eqIdx = cleanedLine.IndexOf('=');
        var name = cleanedLine[..eqIdx].Trim();
        var restOfLine = cleanedLine[(eqIdx + 1)..].Trim();

        return (name, restOfLine);
    }

    public object ParseObject(StreamReader sr, ParserState initialState = ParserState.ObjectOrList, string? startLine = null)
    {
        // This will be a dictionary of string:{Dictionary<string,object?>|List<string>|object?}
        Dictionary<string, object> parsedObject = new();

        ParserState parserState = initialState;
        
        // These 2 props are exclusively for lists
        List<string>? currentList = null;

        string? line = startLine ?? sr.ReadLine();
        void NextLine() => line = sr.ReadLine();
        while (line is not null) {
            var trimmedLine = line.Trim();
            // skip comment & empty lines
            if (trimmedLine.StartsWith('#') || trimmedLine.Length == 0) {
                NextLine();
                continue;
            }

            switch (parserState) {
                case ParserState.ObjectOrList: {
                    return ParseObject(sr, trimmedLine.Contains('=') ? ParserState.Object : ParserState.List, line);
                }
                case ParserState.Object: {
                    // The start of a new object or list
                    if (trimmedLine.Contains('{')) {
                        // Gets the name
                        var (name, restOfLine) = GetLineKeyValue(trimmedLine);

                        if (restOfLine != "{") {
                            throw new InvalidOperationException("Parsing error. Unexpected characters after '{': " +
                                restOfLine);
                        }

                        parsedObject.Add(name, ParseObject(sr, ParserState.ObjectOrList));
                        NextLine();
                        continue;
                    }

                    if (trimmedLine.Contains('=')) {
                        var (name, restOfLine) = GetLineKeyValue(trimmedLine);

                        parsedObject.Add(name, restOfLine);
                        NextLine();
                        continue;
                    }

                    if (trimmedLine.Contains('}')) {
                        NextLine();
                        return parsedObject;
                    }

                    throw new InvalidOperationException("Nothing to do?");
                }
                case ParserState.List: {
                    if (trimmedLine.Contains('}')) {
                        return currentList;
                    }

                    (currentList ??= new List<string>())
                        .Add(StripComments(trimmedLine));
                    NextLine();
                    continue;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return parsedObject;
    }
}