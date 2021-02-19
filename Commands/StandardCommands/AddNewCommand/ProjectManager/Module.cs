using Newtonsoft.Json;

namespace Commands
{
    public class Module
    {
        [JsonProperty]
        public string[] FilesPaths { get; private set; }
        [JsonProperty]
        public string[] DllsPaths { get; private set; }
        [JsonProperty]
        public string Name { get; private set; }

        public Module(string[] files, string[] dlls, string name)
        {
            FilesPaths = files;
            DllsPaths = dlls;
            Name = name;
        }
    }
}
