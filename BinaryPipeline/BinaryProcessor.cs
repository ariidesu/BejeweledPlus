using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.ComponentModel;

using TInput = System.Array;
using TOutput = System.Array;

namespace BinaryPipeline
{
    [ContentProcessor(DisplayName = "BinaryProcessor - ariidesu")]
    internal class BinaryProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            return input;
        }
    }
}