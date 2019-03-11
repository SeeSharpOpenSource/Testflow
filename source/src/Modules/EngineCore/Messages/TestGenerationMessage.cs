using Testflow.EngineCore.Data.GenerationData;

namespace Testflow.EngineCore.Messages
{
    public class TestGenerationMessage : MessageBase
    {
         public TestGenerationInfo Info { get; set; }
    }
}