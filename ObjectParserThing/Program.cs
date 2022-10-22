﻿// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using ObjectParserThing;

Console.WriteLine("Hello, World!");

string StripComments(string line) { int idx = line.IndexOf('#'); return idx != -1 ? line[..idx] : line; }
Console.WriteLine(StripComments("testing #123 gogo"));

var outVal = new ExistingParser().ParseFile(@"C:\Users\johnp\RiderProjects\ObjectParserThing\ObjectParserThing\SourceData.txt");

Console.WriteLine(outVal);

Console.WriteLine(JsonSerializer.Serialize(outVal, new JsonSerializerOptions {
    WriteIndented = true,
}));
Console.WriteLine("Done!");