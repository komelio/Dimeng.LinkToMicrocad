using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Business
{
    public class Configuration
    {
        public Configuration()
        {
        }

        private string pathToLibrary = @"~/ImportSources/Library/";
        private string pathToSubassemblies = @"~/ImportSources/Subassemblies/";
        private string pathToGraphics = "~/ImportSources/Microvellum Data/Graphics/";
        private string pathToOutput = "~/Output/Dms/";

        public string PathToLibrary
        {
            get { return pathToLibrary; }
            set { pathToLibrary = value; }
        }

        public string PathToSubassemblies
        {
            get { return pathToSubassemblies; }
            set { pathToSubassemblies = value; }
        }

        public string PathToGraphics
        {
            get { return pathToGraphics; }
            set { pathToGraphics = value; }
        }

        public string PathToOutput
        {
            get { return pathToOutput; }
            set { pathToOutput = value; }
        }
    }
}
