namespace ObjectParserThing;

public class ClassTech
{
    public ClassTech(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public int era { get; set; } = -1;
    public string? texture { get; set; } = "None";
    public string? category { get; set; } = "None";
    public bool canResearch { get; set; } = true;
    public List<string> modifiers { get; set; } = new();
    public Dictionary<string, string> restrictions { get; set; } = new();
}