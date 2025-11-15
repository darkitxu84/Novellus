using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.Package
{
    public class Package
    {
        public PackageMetadata Metadata { get; set; } = new PackageMetadata();
        public bool Enabled { get; set; } = true;
        public string Path { get; set; } = string.Empty;
    }
}
