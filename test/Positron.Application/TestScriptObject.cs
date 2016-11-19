using Positron.UI;

namespace Positron.Application
{
    class TestScriptObject : IGlobalScriptObject
    {
        public string Name => "test";

        public string Test(string input)
        {
            return $"!!{input}!!";
        }
    }
}
