using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NodeListWithRand
{
    class Program
    {
        public const string Path = "D://Dotnet/Testovoe/text.txt";
        public const int CountElements = 1000;
        static void Main(string[] args)
        {
            ListRand listRand = new ListRand();

            ListNode[] arrNode = new ListNode[CountElements];
            listRand.Count = CountElements;

            for(int i = 0; i < CountElements; i++)
            {
                arrNode[i] = new ListNode();
                arrNode[i].Data = $"El{i}";

                if(i != 0)
                {
                    arrNode[i].Prev = arrNode[i - 1];
                    arrNode[i - 1].Next = arrNode[i];
                }
            }

            listRand.Head = arrNode[0];
            listRand.Tail = arrNode[CountElements - 1];

            for (int i = 0; i < CountElements; i++)
            {
                Random rnd = new Random();
                int randElement = rnd.Next(0, CountElements);
                Random rndEmpty = new Random();
                int randEmpty = rndEmpty.Next(0, 5);
                if (randEmpty != 3)
                {
                    arrNode[i].Rand = arrNode[randElement];
                }
            }

            DebugOutputInfo(listRand);

            Stopwatch stopwatch = new();

            using (FileStream fstream = new FileStream(Path, FileMode.Create))
            {
                stopwatch.Start();

                listRand.Serialize(fstream);

                stopwatch.Stop();

            }

            Console.WriteLine($"Serialization time: {stopwatch.ElapsedMilliseconds}. Element count = {CountElements}");
            stopwatch.Restart();

            ListRand listRand2 = new ListRand();

            using (FileStream fstream = File.OpenRead(Path))
            {
                listRand2.Deserialize(fstream);
            }

            Console.WriteLine($"Deserialization time: {stopwatch.ElapsedMilliseconds}. Element count = {CountElements}");
            DebugOutputInfo(listRand2);
        }

        public static void DebugOutputInfo(ListRand listRand)
        {
            ListNode current = listRand.Head;
            string endFile = string.Empty;
            while(current != null)
            {
                if(current.Rand != null)
                {
                    endFile += $"Element {current.Data} with link rand to element {current.Rand.Data} \n";
                }
                else
                {
                    endFile += $"Element {current.Data} with link rand to element Empty \n";
                }
                current = current.Next;
            }
            Console.WriteLine(endFile);
        }
    }

    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand;
        public string Data;
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            byte[] bytesCount = Encoding.Default.GetBytes($"{Count};");
            s.Write(bytesCount);

            Dictionary<ListNode, Tuple<int, int>> dicNode = new Dictionary<ListNode, Tuple<int, int>>();
            ListNode current = Head;
            int counter = 0;

            if(Head == null)
            {
                Console.WriteLine("Head element is empty!");
                return;
            }

            while (current != null)
            {
                dicNode.Add(current, Tuple.Create(counter, -1));

                current = current.Next;
                counter++;
            }

            current = Head;
            counter = 0;
            while (current != null)
            {
                if (current.Rand != null)
                {
                    dicNode[current] = Tuple.Create(dicNode[current].Item1, dicNode[current.Rand].Item1);
                }

                byte[] bytesData = Encoding.Default.GetBytes($"{current.Data}:{dicNode[current].Item2};");
                s.Write(bytesData);

                current = current.Next;
                counter++;
            }
        }

        public void Deserialize(FileStream s)
        {
            byte[] bytes = new byte[s.Length];
            s.Read(bytes, 0, bytes.Length);
            string text = Encoding.Default.GetString(bytes);

            string[] rows = text.Split(new char[] { ';' });

            Count = int.Parse(rows[0]);

            if(Count == 0)
            {
                Console.WriteLine("File is empty!");
                return;
            }

            ListNode[] arrNode = new ListNode[Count];

            for(int i = 0; i < Count; i++)
            {
                arrNode[i] = new ListNode();
                arrNode[i].Data = rows[i + 1].Split(new char[] { ':' })[0];

                if(i != 0)
                {
                    arrNode[i].Prev = arrNode[i - 1];
                    arrNode[i - 1].Next = arrNode[i];
                }
            }

            Head = arrNode[0];
            Tail = arrNode[Count - 1];

            for (int i = 0; i < Count; i++)
            {
                int randElementIndex = int.Parse(rows[i + 1].Split(new char[] { ':' })[1]);
                if(randElementIndex == -1)
                {
                    arrNode[i].Rand = null;
                }
                else
                {
                    arrNode[i].Rand = arrNode[randElementIndex];
                }
            }
        }
    }
}
