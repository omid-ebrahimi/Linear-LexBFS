using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LexBFS
{
    class Program
    {
        #region Variables and Constants
        static LinkedList<Bucket> bucketList = new LinkedList<Bucket>();
        static Bucket bucket;
        static bool[] vertexChecklist;
        const ushort firstVertexNumber = 0;
        static string inputPath;
        static string outputPath;
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("1. inputs/graph1.txt\n2. inputs/graph2.txt\nSelect your graph: (1/2)");
            if (Console.ReadLine() == "1")
            {
                inputPath = "inputs/graph1.txt";
                outputPath = "outputs/q1_graph1.out";
            }
            else
            {
                inputPath = "inputs/graph2.txt";
                outputPath = "outputs/q1_graph2.out";
            }
            Console.WriteLine("\nAlgorithm started...");

            DateTime startTime = DateTime.Now;

            readGraph();

            runLexBFS();

            Console.WriteLine("Result Exported to: " + outputPath);
            Console.WriteLine("\nDuration Time: " + (DateTime.Now - startTime).TotalSeconds + " seconds");
            Console.ReadLine();
        }

        static void readGraph()
        {
            ushort rightVertexNumber;
            bucket = new Bucket();

            //Put an empty bucket into the bucket list
            bucketList.AddFirst(bucket);

            Hashtable hashTable = new Hashtable();
            List<Vertex> vertexList;

            TextReader tr = new StreamReader(inputPath);
            vertexChecklist = new bool[UInt16.Parse(tr.ReadLine().Split('\t')[0])];

            //Create first vertex (We know it's number is 0)
            Vertex vertex = new Vertex(firstVertexNumber, bucketList.First);
            bucket.vertices.AddLast(vertex);
            vertexChecklist[firstVertexNumber] = true;

            string line = tr.ReadLine();
            string[] lineArray;
            while (line != null)
            {
                lineArray = line.Split('\t');
                vertexChecklist[UInt16.Parse(lineArray[0])] = vertexChecklist[UInt16.Parse(lineArray[1])] = true;
                if (UInt16.Parse(lineArray[0]) != vertex.number)
                {
                    vertex = new Vertex(UInt16.Parse(lineArray[0]), bucketList.First);
                    if (lineArray[0] == "5")
                        bucket.vertices.AddFirst(vertex);
                    else
                        bucket.vertices.AddLast(vertex);

                    if (hashTable.Contains(vertex.number))
                    {
                        foreach (Vertex neighbor in (List<Vertex>)hashTable[vertex.number])
                        {
                            vertex.neighbors.Add(neighbor);
                            neighbor.neighbors.Add(vertex);
                        }
                        hashTable.Remove(vertex.number);
                    }
                }

                rightVertexNumber = UInt16.Parse(lineArray[1]);
                if (hashTable.Contains(rightVertexNumber))
                {
                    vertexList = (List<Vertex>)hashTable[rightVertexNumber];
                    vertexList.Add(vertex);
                    hashTable[rightVertexNumber] = vertexList;
                }
                else
                {
                    vertexList = new List<Vertex>();
                    vertexList.Add(vertex);
                    hashTable[rightVertexNumber] = vertexList;
                }

                line = tr.ReadLine();
            }
            tr.Close();

            foreach (DictionaryEntry tableEntry in hashTable)
            {
                vertex = new Vertex((ushort)tableEntry.Key, bucketList.First);
                bucket.vertices.AddLast(vertex);
                foreach (Vertex neighbor in (List<Vertex>)tableEntry.Value)
                {
                    vertex.neighbors.Add(neighbor);
                    neighbor.neighbors.Add(vertex);
                }
            }
        }

        static void runLexBFS()
        {
            Vertex vertex;
            string label;
            LinkedListNode<Bucket> bucketNode;
            int bucketSize = bucketList.First.Value.vertices.Count;
            TextWriter tw = new StreamWriter(outputPath);
            tw.WriteLine(vertexChecklist.Length);

            for (ushort i = 0; i < bucketSize; i++)
            {
                vertex = bucketList.First.Value.vertices.First.Value;
                tw.WriteLine(vertex.number + "\t" + vertex.bucketNode.Value.label);

                bucketList.First.Value.vertices.RemoveFirst();
                vertex.bucketNode = null;
                if (bucketList.First.Value.vertices.Count == 0)
                {
                    bucketList.RemoveFirst();
                }

                foreach (Vertex neighbor in vertex.neighbors)
                {
                    if (neighbor.bucketNode != null)
                    {
                        neighbor.bucketNode.Value.vertices.Remove(neighbor);
                        if (neighbor.bucketNode.Value.label != "0")
                        {
                            label = neighbor.bucketNode.Value.label + "," + (bucketSize - i);
                        }
                        else
                        {
                            label = (bucketSize - i).ToString();
                        }

                        if (neighbor.bucketNode.Previous != null && neighbor.bucketNode.Previous.Value.label == label)
                        {
                            neighbor.bucketNode.Previous.Value.vertices.AddLast(neighbor);
                        }
                        else
                        {
                            bucket = new Bucket(label);
                            bucket.vertices.AddLast(neighbor);
                            bucketList.AddBefore(neighbor.bucketNode, bucket);
                        }

                        bucketNode = neighbor.bucketNode.Previous;
                        if (neighbor.bucketNode.Value.vertices.Count == 0)
                        {
                            bucketList.Remove(neighbor.bucketNode);
                        }
                        neighbor.bucketNode = bucketNode;
                    }
                }
            }

            //Find isolated vertices and add them to the end of peo
            for (int i = 0; i < vertexChecklist.Length; i++)
            {
                if (vertexChecklist[i] == false)
                {
                    tw.WriteLine(i + "\t0");
                }
            }

            tw.Close();
        }
    }
}
