using System;

namespace Nlpcn.Net.Commons.Lang.Tire.Domain
{
    public interface IWoodInterface : IEquatable<char>
    {
        string[] Param { get; set; }
        byte Status { get; set; }
        IWoodInterface Add(IWoodInterface paramWoodInterface);
        IWoodInterface Get(char paramChar);
        bool Contains(char paramChar);
        char GetChar();
        int CompareTo(char c);
    }
}