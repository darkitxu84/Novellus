using NovellusLib.Logging;
using System.Xml.Serialization;

namespace NovellusLib.Packages;

// for compatibility reasons with aemulus
// don't change the class name
public class Metadata
{
    public string? name { get; set; }
    public string? id { get; set; }
    public string? author { get; set; }
    public string? version { get; set; }
    public string? link { get; set; }
    public string? description { get; set; }
    public string? skippedVersion { get; set; }
}
