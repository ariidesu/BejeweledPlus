using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

using TImport = System.Array;

namespace BinaryPipeline
{
    [ContentImporter(".*", DisplayName = "Binary Importer - ariidesu", DefaultProcessor = nameof(BinaryProcessor))]
    public class BinaryImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            return File.ReadAllBytes(filename);
        }
    }
    }
