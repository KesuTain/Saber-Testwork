using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NodeListWithRand
{
    class Program
    {
        public const string Path = "D://Dotnet/Testovoe/text.txt";
        static void Main(string[] args)
        {
            ListRand listRand = new ListRand();

            ListNode l1 = new ListNode() { Data = "Test_1" };
            ListNode l2 = new ListNode() { Data = "Test_2" };
            ListNode l3 = new ListNode() { Data = "Test_3" };
            ListNode l4 = new ListNode() { Data = "Test_4" };
            ListNode l5 = new ListNode() { Data = "Test_5" };
            ListNode l6 = new ListNode() { Data = "Test_6" };

            l1.Prev = null; l1.Next = l2; l1.Rand = l1;
            l2.Prev = l1; l2.Next = l3; l2.Rand = l3;
            l3.Prev = l2; l3.Next = l4; l3.Rand = l4;
            l4.Prev = l3; l4.Next = l5; l4.Rand = l2;
            l5.Prev = l4; l5.Next = l6; l5.Rand = l6;
            l6.Prev = l5; l6.Next = null; l6.Rand = l5;

            listRand.Head = l1;
            listRand.Tail = l6;
            listRand.Count = 6;

            DebugOutputInfo(listRand);

            using (FileStream fstream = new FileStream(Path, FileMode.OpenOrCreate))
            {
                listRand.Serialize(fstream);
            }

            ListRand listRand2 = new ListRand();

            using (FileStream fstream = File.OpenRead(Path))
            {
                listRand2.Deserialize(fstream);
            }

            DebugOutputInfo(listRand2);
        }

        public static void DebugOutputInfo(ListRand listRand)
        {
            ListNode current = listRand.Head;
            string endFile = string.Empty;
            while(current != null)
            {
                endFile += $"Element {current.Data} with link rand to element {current.Rand.Data} \n";
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
            ListNode[] arrNode = new ListNode[Count];

            byte[] bytesCount = Encoding.Default.GetBytes($"{Count};");
            s.Write(bytesCount);

            ListNode current = Head;
            int counter = 0;
            while(current != null)
            {
                byte[] bytesData = Encoding.Default.GetBytes($"{current.Data};");
                s.Write(bytesData);

                arrNode[counter] = current;
                current = current.Next;
                counter++;
            }

            Dictionary<ListNode, int> dicNode = new Dictionary<ListNode, int>();
            for (int i = 0; i < arrNode.Length; i++)
            {
                dicNode.Add(arrNode[i], i);
            }

            string numbersRand = string.Empty;
            for (int i = 0; i < arrNode.Length; i++)
            {
                numbersRand += dicNode[arrNode[i].Rand] + "-";
            }

            byte[] bytesNumbers = Encoding.Default.GetBytes(numbersRand.TrimEnd('-'));
            s.Write(bytesNumbers);
        }

        public void Deserialize(FileStream s)
        {
            byte[] bytes = new byte[s.Length];
            s.Read(bytes, 0, bytes.Length);
            string text = Encoding.Default.GetString(bytes);

            string[] rows = text.Split(new char[] { ';' });

            Count = int.Parse(rows[0]);

            ListNode[] arrNode = new ListNode[Count];
            for(int i = 0; i < Count; i++)
            {
                arrNode[i] = new ListNode();
                arrNode[i].Data = rows[i + 1];
            }

            string[] stringNumbersRand = rows[rows.Length - 1].Split(new char[] { '-' });
            for (int i = 0; i < Count; i++)
            {
                int randElementIndex = int.Parse(stringNumbersRand[i]);
                arrNode[i].Rand = arrNode[randElementIndex];
            }

            Head = arrNode[0];
            Head.Prev = null;
            Head.Next = arrNode[1];

            Tail = arrNode[arrNode.Length - 1];
            Tail.Prev = arrNode[arrNode.Length - 2];
            Tail.Next = null;

            ListNode current = arrNode[1];
            int counter = 1;
            while (current != Tail)
            {
                current.Next = arrNode[counter + 1];
                current.Prev = arrNode[counter - 1];
                current = current.Next;
                counter++;
            }
        }
    }
}
