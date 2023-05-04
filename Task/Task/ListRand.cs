using System.IO;

namespace Task
{
    public class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            ListSerializerService.Serialize(s, this);
        }
        
        public void Deserialize(FileStream s)
        {
            ListSerializerService.Deserialize(s);
        } 
    }
}