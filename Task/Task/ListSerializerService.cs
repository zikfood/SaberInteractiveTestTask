using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task
{
    public static class ListSerializerService
    {
        private static readonly int FS_END = -1;
        private static readonly string DATA_FIELD = "Data";
        private static readonly string RAND_FIELD = "Rand";
        public static void Serialize(FileStream s, ListRand listRand)
        {
            ListNode curNode = listRand.Head;
            StringBuilder sb = new StringBuilder();
            Dictionary<ListNode, int> nodeIndexes = new Dictionary<ListNode, int>();

            for (int i = 0; i < listRand.Count; i++)
            {
                nodeIndexes.Add(curNode, i);
                curNode = curNode.Next;
            }

            sb.Append("[");
            foreach (var node in nodeIndexes)
            {
                var data = node.Key.Data;
                data = Regex.Escape(data);
                data = Regex.Replace(data, "\"", "\\\"");
                
                sb.Append("{");
                sb.Append($"\"Data\":\"{data}\",");
                sb.Append($"\"Rand\":\"{node.Value.ToString()}\"");
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1); // removing extra comma
            sb.Append("]");
            
            var utf8 = new UTF8Encoding();
            var result = utf8.GetBytes(sb.ToString());

            s.Write(result, 0, result.Length);
        }

        public static ListRand Deserialize(FileStream s)
        {
            char objectFirstByte = '{';
            string rawObjectData;
            ListNode[] nodeArr;
            List<Dictionary<string, string>> nodesData = new List<Dictionary<string, string>>();

            for (int curByte = s.ReadByte(); curByte != FS_END; curByte = s.ReadByte())
            {
                if (curByte == objectFirstByte)
                {
                    nodesData.Add(ReadObject(s));
                }
            }

            nodeArr = new ListNode[nodesData.Count];

            for (int i = 0; i < nodesData.Count; i++)
            {
                nodeArr[i] = new ListNode();
            }

            for (int i = 0; i < nodesData.Count; i++)
            {
                nodeArr[i].Data = nodesData[i][DATA_FIELD];
                nodeArr[i].Next = nodeArr.ElementAtOrDefault(i + 1) ?? null;
                nodeArr[i].Prev = nodeArr.ElementAtOrDefault(i - 1) ?? null;
                nodeArr[i].Rand = nodeArr[Convert.ToInt32(nodesData[i][RAND_FIELD])];
            }

            var result = new ListRand()
            {
                Count = nodesData.Count,
                Head = nodeArr[0],
                Tail = nodeArr[nodeArr.Length - 1]
            };

            return result;
        }

        private static Dictionary<string, string> ReadObject(FileStream s)
        {
            StringBuilder sb = new StringBuilder();
            bool isReadingKeyOrValue = false;
            bool isReadingKey = true;
            Dictionary<string, string> objectParams = new Dictionary<string, string>();
            string keyName = null;
            for (int curByte = s.ReadByte(); curByte != '}' || isReadingKeyOrValue; curByte = s.ReadByte())
            {
                if (curByte == '"')
                {
                    isReadingKeyOrValue = !isReadingKeyOrValue;
                    if (!isReadingKeyOrValue)
                    {
                        if (!isReadingKey)
                        {
                            objectParams.Add(keyName, Regex.Unescape(sb.ToString()));
                        }
                        else
                        {
                            keyName = sb.ToString();

                        }
                        isReadingKey = !isReadingKey;
                        sb.Clear();
                    }
                    continue;
                }

                if (isReadingKeyOrValue)
                {
                    sb.Append((char)curByte);
                
                    if (curByte == '\\')
                    {
                        curByte = s.ReadByte();
                        sb.Append((char) curByte);
                    }
                }
            }
            return objectParams;
        }
    }
}