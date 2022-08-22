// 
// Copyright (c) Rafael Garcia <https://github.com/rafaelfgx>
// 
// All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class BaseEvent : IEquatable<BaseEvent?>
    {
        protected BaseEvent()
        {
            Topic = this.GetType().Name;
        }

        public string Topic { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public Guid EventId { get; private set; }

        public bool IsPublished { get; private set; }

        public BaseEvent Publising(DateTimeOffset timestamp = default)
        {
            EventId = Guid.NewGuid();

            IsPublished = true;

            Timestamp = Timestamp == default
                ? timestamp == default ? DateTimeOffset.UtcNow : timestamp
                : Timestamp;

            return this;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BaseEvent);
        }

        public bool Equals(BaseEvent? other)
        {
            return other != null &&
                   Topic == other.Topic &&
                   Timestamp.Equals(other.Timestamp);
        }

        public override int GetHashCode()
        {
            var hashCode = 63862983;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Topic);
            hashCode = hashCode * -1521134295 + Timestamp.GetHashCode();
            return hashCode;
        }
    }
}
