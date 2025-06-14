using System.Collections.Generic;

namespace MemoryBuilder.Generator;

internal interface IGenerable
{
    IEnumerable<string> Generate();
}
