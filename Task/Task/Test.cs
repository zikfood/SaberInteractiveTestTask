using System.IO;

namespace Task
{
    internal class Test
    {
        public static void Main(string[] args)
        {
            ListNode head = new ListNode();
            ListNode tail = new ListNode();
            ListNode rand = new ListNode();

            head.Data = "h3ad";
            head.Next = rand;
            head.Rand = head;

            rand.Data = "midd}le";
            rand.Prev = head;
            rand.Next = tail;
            rand.Rand = tail;

            tail.Data = "tai\" \n l";
            tail.Prev = rand;
            tail.Rand = head;
            
            ListRand listRand = new ListRand()
            {
                Count = 3,
                Head = head,
                Tail = tail
            };
            
            using (FileStream fs = new FileStream("test.json", FileMode.OpenOrCreate))
            {
                listRand.Serialize(fs);
            }

            using (FileStream fs = new FileStream("test.json", FileMode.Open))
            {
                listRand.Deserialize(fs);
            }
        }
    }
}