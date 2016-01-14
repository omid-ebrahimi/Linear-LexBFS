using System.Collections.Generic;

namespace LexBFS
{
    class Bucket
    {
        public string label = "0";
        public LinkedList<Vertex> vertices = new LinkedList<Vertex>();

        public Bucket() { }

        public Bucket(string label)
        {
            this.label = label;
        }
    }
}
