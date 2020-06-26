
namespace PrintApp
{
    class Document
    {
        public string Color { get; set; }
        public string Format { get; set; }
        public string Orientation { get; set; }

        public Document()
        {

        }

        public Document(string color, string format, string orientation)
        {
            Color = color;
            Format = format;
            Orientation = orientation;
        }
    }
}
