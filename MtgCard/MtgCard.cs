using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgCard
{
    public class LegalityObj
    {
        public string Format { get; set; }
        public string Legality { get; set; }
    }

    public class Card
    {
        public string Name { get; set; }
        public string Layout { get; set; }
        public string ManaCost { get; set; }
        public float Cmc { get; set; }
        public List<string> Colors { get; set; }
        public string Type { get; set; }
        public List<string> Types { get; set; }
        public string Text { get; set; }
        public string ImageName { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public List<string> Printings { get; set; }
        public List<LegalityObj> Legalities { get; set; }
        public List<string> ColorIdentity { get; set; }
    }
}
