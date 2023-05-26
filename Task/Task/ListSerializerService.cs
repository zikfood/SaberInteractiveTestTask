using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task
{
    /// <summary>
    /// This class handles linked list serialization and deserialization.
    /// </summary>
    public static class ListSerializerService
    {
        private static readonly int FS_END = -1;
        private static readonly string DATA_FIELD = "Data";
        private static readonly string RAND_FIELD = "Rand";
        
        /// <summary>
        /// Serialize linked list and write to file stream.
        /// </summary>
        /// <param name="fs">File stream to write linked list</param>
        /// <param name="listRand">Linked list to serialize</param>
        public static void Serialize(FileStream s, ListRand listRand)
        {
            var nodeIndexes = IndexingNodes(listRand);

            var utf8 = new UTF8Encoding();
            var result = utf8.GetBytes(WriteNodes(nodeIndexes));

            s.Write(result, 0, result.Length);
        }
        
        /// <summary>
        /// Indexing nodes 
        /// </summary>
        /// <param name="listRand"> Linked list to take nodes from</param>
        /// <returns> Dict with nodes linked with theirs index</returns>
        private static Dictionary<ListNode, int> IndexingNodes(ListRand listRand)
        {
            ListNode curNode = listRand.Head;
            Dictionary<ListNode, int> nodeIndexes = new Dictionary<ListNode, int>();
            
            for (int i = 0; i < listRand.Count; i++)
            {
                nodeIndexes.Add(curNode, i);
                curNode = curNode.Next;
            }

            return nodeIndexes;
        }
        
        /// <summary>
        /// Writes nodes data and rand field to Json format
        /// </summary>
        /// <param name="nodeIndexes"> dict of nodes paired with index</param>
        /// <returns> Json string with nodes params</returns>
        private static string WriteNodes(Dictionary<ListNode, int> nodeIndexes)
        {
            StringBuilder sb = new StringBuilder();
            
            if (nodeIndexes.Count == 0)
            {
                sb.Append("[]");
                return sb.ToString();
            }
            
            sb.Append("[");
            foreach (var node in nodeIndexes)
            {
                var data = node.Key.Data;
                data = Regex.Escape(data);
                data = Regex.Replace(data, "\"", "\\\"");
                
                sb.Append("{");
                sb.Append($"\"Data\":\"{data}\",");
                sb.Append($"\"Rand\":\"{nodeIndexes[node.Key.Rand]}\"");
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1); // removing extra comma
            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// Deserialize linked list from filestream.
        /// </summary>
        /// <param name="s">File stream with serialized linked list</param>
        /// <param name="listRand">Linked list to serialize</param>
        /// <returns> deserialized ListRand</returns>
        public static void Deserialize(FileStream s, ListRand listRand)
        {
            char objectFirstByte = '{';
            ListNode[] nodeArr;
            List<Dictionary<string, string>> nodesData = new List<Dictionary<string, string>>();

            for (int curByte = s.ReadByte(); curByte != FS_END; curByte = s.ReadByte())
            {
                if (curByte == objectFirstByte)
                {
                    nodesData.Add(ReadNode(s));
                }
            }

            nodeArr = CreateNodes(nodesData);

            if (nodeArr.Length == 0)
            {
                listRand.Count = 0;
                listRand.Head = null;
                listRand.Tail = null;
                return;
            }

            listRand.Count = nodesData.Count;
            listRand.Head = nodeArr[0];
            listRand.Tail = nodeArr[nodeArr.Length - 1];
        }

        /// <summary>
        /// Reads names and values of node params
        /// </summary>
        /// <param name="s">File stream with serialized linked list</param>
        /// <returns> Dict with pairs of names and values of nodes params</returns>
        private static Dictionary<string, string> ReadNode(FileStream s)
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

        /// <summary>
        /// Creates array of nodes and fill them
        /// </summary>
        /// <param name="nodesData"> data to create nodes from</param>
        /// <returns> array of filled linked nodes</returns>
        private static ListNode[] CreateNodes(List<Dictionary<string, string>> nodesData)
        {
            ListNode[] nodeArr = new ListNode[nodesData.Count];

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

            return nodeArr;
        }
    }
}