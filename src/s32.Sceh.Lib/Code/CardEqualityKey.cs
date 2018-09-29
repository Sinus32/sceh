﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
    public struct CardEqualityKey : IEquatable<CardEqualityKey>
    {
        public long ClassId;
        public long InstanceId;
        public string MarketHashName;

        public CardEqualityKey(long classId, long instanceId, string marketHashName = null)
        {
            ClassId = classId;
            InstanceId = instanceId;
            MarketHashName = marketHashName;
        }

        public bool Equals(CardEqualityKey other)
        {
            if (this.ClassId != other.ClassId)
                return false;
            if (String.IsNullOrEmpty(this.MarketHashName) || String.IsNullOrEmpty(other.MarketHashName))
                return this.InstanceId == other.InstanceId;
            return String.Equals(this.MarketHashName, other.MarketHashName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CardEqualityKey))
                return false;

            return this.Equals((CardEqualityKey)obj);
        }

        public override int GetHashCode()
        {
            return (int)ClassId;
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(MarketHashName)
                ? String.Concat(ClassId, '_', InstanceId)
                : String.Concat(ClassId, '_', InstanceId, '_', MarketHashName);
        }
    }
}
