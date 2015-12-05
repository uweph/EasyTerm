using System;
using System.Collections.Generic;

// --------------------------------------------------------------------------------
/// <summary>
/// Manges ranges. If a range is added to a list of ranges, 
/// existing ranges are merged and/or adjusted as necessary
/// </summary>
// --------------------------------------------------------------------------------
public class RangeMap
{
    public bool OverlapsRange(int from, int len)
    {
        int to = from + len - 1;


        foreach (Range range in _Ranges)
        {
            if (to < range.From)
                return false;

            if (from <= range.To)
                return true;

        }

        return false;
    }


    // ********************************************************************************
    /// <summary>
    /// Adds a new range to the range list
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    /// <created>UPh,01.12.2015</created>
    /// <changed>UPh,01.12.2015</changed>
    // ********************************************************************************
    public void AddRange(int from, int len)
    {
        int to = from + len - 1;

        int nRanges = _Ranges.Count;

        if (nRanges == 0 || from > _Ranges[nRanges - 1].To + 1)
        {
            // Add new range at end
            _Ranges.Add(new Range(from, to));
            return;
        }

        if (to < _Ranges[0].From - 1)
        {
            // Insert new range at begin
            _Ranges.Insert(0, new Range(from, to));
            return;
        }

        int iRange0 = -1;   // New range starts before or in this range
        for (int i = 0; i < nRanges; i++)
        {
            if (from <= _Ranges[i].To + 1)
            {
                iRange0 = i;
                break;
            }
        }

        if (to < _Ranges[iRange0].From - 1)
        {
            // New range lies completely before iRange0
            // Insert new range before
            _Ranges.Insert(iRange0, new Range(from, to));
            return;
        }

        // Find last affected range
        int iRange1;
        for (iRange1 = nRanges - 1; iRange1 > iRange0; iRange1--)
        {
            if (to >= _Ranges[iRange1].From - 1)
                break;
        }

        // Remove unnecessary ranges
        if (iRange1 > iRange0)
        {
            Ranges[iRange0].To = Ranges[iRange1].To;
            Ranges.RemoveRange(iRange0 + 1, iRange1 - iRange0);
            iRange1 = iRange0;
        }

        // Adjust ranges
        _Ranges[iRange0].From = Math.Min(_Ranges[iRange0].From, from);
        _Ranges[iRange1].To = Math.Max(_Ranges[iRange1].To, to);
    }

    // ********************************************************************************
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <created>UPh,01.12.2015</created>
    /// <changed>UPh,01.12.2015</changed>
    // ********************************************************************************
    public void Clear()
    {
        _Ranges.Clear();
    }


    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class Range
    {
        public int From;
        public int To;
        public Range(int from, int to)
        {
            From = from;
            To = to;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", From, To);
        }
    }

    List<Range> _Ranges = new List<Range>();



    public List<Range> Ranges {get{return _Ranges;}}


}


