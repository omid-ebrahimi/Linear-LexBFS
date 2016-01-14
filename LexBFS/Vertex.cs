using System.Collections.Generic;

namespace LexBFS
{
    class Vertex
    {
        public ushort number;
        public LinkedListNode<Bucket> bucketNode;
        public List<Vertex> neighbors = new List<Vertex>();

        public Vertex(ushort number, LinkedListNode<Bucket> bucketNode)
        {
            this.number = number;
            this.bucketNode = bucketNode;
        }
    }
}
