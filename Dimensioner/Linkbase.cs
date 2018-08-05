using Dimensioner.Components;

namespace Dimensioner
{
    public class Linkbase : Modulable
    {
        public XbrlSchema Schema { get; set; }
        public string Path { get; set; }
        public string Role { get; set; }

        public Linkbase(XbrlSchema schema, string path, string role)
        {
            Schema = schema;
            Path = path;
            Role = role;
        }
    }
}
