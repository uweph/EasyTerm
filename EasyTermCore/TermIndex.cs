using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTermCore
{


    // --------------------------------------------------------------------------------
    /// <summary>
    /// Index for faster terminology search
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermIndex 
    {           

        IndexItem[] _Items;
        public string Language {get; private set;}

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        public TermIndex()
        {
            _Items = null;
        }

        int CompareHash(IndexItem item1, IndexItem item2)
        {
            if (item1.Hash < item2.Hash)
                return 1;

            if (item1.Hash > item2.Hash)
                return -1;
            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        public void BuildIndex(string language, TermListItems items)
        {
            Language = language;
            int count = items.Count;
            _Items = new IndexItem[count];

            for (int i = 0; i < count; i++)
            {
                TermListItem termItem = items[i];

                _Items[i].Hash = MakeGlossaryHashCode(termItem.Term);
                _Items[i].TermBaseID = termItem.TermBaseID;
                _Items[i].TermID = termItem.TermID;
            }

            Array.Sort(_Items);
        }

        // ********************************************************************************
        /// <summary>
        /// Enumerator for all matches of a given term
        /// </summary>
        /// <typeparam name="IndexItem"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        public IEnumerable<IndexItem> Matches(string text, int from = -1, int len = -1)
        {
            if (_Items == null)
                yield break;

            ulong hash = MakeGlossaryHashCode(text, from, len);

            int inx0 = Array.BinarySearch(_Items, hash);

            if (inx0 < 0)
                yield break;

            // Goto first item with this hash
            while (inx0 > 0 && _Items[inx0 - 1].Hash == hash)
                inx0--;

            for (int i = inx0; i < _Items.Length; i++)
            {
                if (_Items[i].Hash != hash)
                    yield break;

                yield return _Items[i];
            }
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        ulong MakeGlossaryHashCode(string text, int from = -1, int len = 0)
        {
            ulong code = 0;
            ulong n = 1;

            if (from < 0)
            {
                from = 0;
                len = text.Length;
            }

            for (int i = from; i < from + len; i++)
            {
                char c0 = text[i];

                if (!char.IsLetter(c0))
                    continue;

                char c = char.ToLower(c0);

                ulong overflow = (code & 0xF8000000) >> 27;
                code <<= 5;
                code += overflow;

                code ^= (n++ + (ulong) c);
            }

            return code;
        }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public struct IndexItem : IComparable, IComparable<IndexItem>, IComparable<ulong>
    {
        public ulong Hash;
        public int TermBaseID;
        public int TermID;

        public int CompareTo(IndexItem other)
        {
            if (Hash < other.Hash)
                return -1;
            if (Hash > other.Hash)
                return 1;
            return 0;
        }

        public int CompareTo(ulong other)
        {
            if (Hash < other)
                return -1;
            if (Hash > other)
                return 1;
            return 0;
        }

        public int CompareTo(object other)
        {
            if (other is ulong) 
                return CompareTo((ulong)other);

            if (other is IndexItem)
                return CompareTo((IndexItem)other);

            throw new InvalidOperationException("Other must be ulong or IndexItem.");
        }
    }

}
