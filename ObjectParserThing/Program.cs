using System.Text.Json;

Console.Write("Enter source file path: ");
var line = Console.ReadLine();

var outVal = new ObjectParserThing.ObjectParserThing().ParseFile(line);

Console.WriteLine(JsonSerializer.Serialize(outVal, new JsonSerializerOptions {
    WriteIndented = true,
}));
