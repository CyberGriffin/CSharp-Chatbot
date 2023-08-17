/*
ToolBox is a system that contains tools (functions) that can be used by the chatbot.

Some limitations of this system:
    - Decreased accuracy as more tools are added.
    - Tools are not context-aware.
    - Currently, functions must return a string due to me
      not wanting to deal with generics and invoking the different delegate types.
    - No support for parameters at the moment.
    - Responses from the tools are not stored in memory.
*/
namespace Project_ToolBox
{
    
    internal class Tool
    {
        public string? name { get; }
        public string? desc { get; }
        public Func<string>? func { get; }
        public bool allowCallback { get; }

        public Tool(string name, string desc, Func<string> func, bool allowCallback = false)
        {
            this.name = name;
            this.desc = desc;
            this.func = func;
            this.allowCallback = allowCallback;
        }
    }

    static class ToolBox
    {
        public static string Example()
        {
            return "[Example function executed successfully!]";
        }

        public static Tool[] tools { get; private set; }
        static ToolBox()
        {
            tools = new Tool[]
            {
                new Tool(
                    "example",
                    "useful for when the user wants to test the toolbox.",
                    Example,
                    true
                ),
            };
        }
    }
}