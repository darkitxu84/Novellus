using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.Package
{
    public class PackageMetadata
    {
        public string Name { get; set; } = "";
        public string Id { get; set; } = "";
        public string Author { get; set; } = "";
        public string Version { get; set; } = "";
        public string Link { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Dependencies { get; set; } = [];
    }
}
