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

        public CardEqualityKey(Card card)
        {
            if (card != null)
            {
                ClassId = card.ClassId;
                InstanceId = card.InstanceId;
            }
            else
            {
                ClassId = 0;
                InstanceId = 0;
            }
        }

        public CardEqualityKey(long classId, long instanceId)
        {
            ClassId = classId;
            InstanceId = instanceId;
        }

        public static implicit operator CardEqualityKey(Card card)
        {
            return new CardEqualityKey(card);
        }

        public bool Equals(CardEqualityKey other)
        {
            return this.ClassId == other.ClassId && this.InstanceId == other.InstanceId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CardEqualityKey))
                return false;

            return this.Equals((CardEqualityKey)obj);
        }

        public override int GetHashCode()
        {
            return (int)ClassId ^ (int)InstanceId;
        }

        public override string ToString()
        {
            return String.Concat(ClassId, '_', InstanceId);
        }
    }
}