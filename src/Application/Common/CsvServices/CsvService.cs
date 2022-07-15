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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.CsvServices
{
    public class CsvService : ICsvService
    {
        public IReadOnlyList<T> ReadCsv<T>(in IReadOnlyList<string> lines, char separator, CancellationToken cancellationToken = default) where T : new()
        {
            var items = new List<T>();

            if (lines.Count < 2)
            {
                return items;
            }

            var columns = lines[0].Split(separator);

            if (columns.Length == 0)
            {
                return items;
            }

            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(separator);

                var item = new T();

                for (var i = 0; i < columns.Length; i++)
                {
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                    try
                    {
                        var property = item.GetType().GetProperty(columns[i]);

                        if (property is null) continue;

                        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                        var value = Convert.ChangeType(values[i], type, CultureInfo.InvariantCulture);

                        property.SetValue(item, value, null);
                    }
                    catch
                    {
                    }
                }

                items.Add(item);
            }

            return items;
        }

        public IReadOnlyList<string> WriteCsv<T>(in IReadOnlyList<T> items, char separator)
        {
            var lines = new List<string>();

            if (items is null)
            {
                return lines;
            }

            var columns = typeof(T).GetProperties().Select(property => property.Name).ToList();

            lines.Add(string.Join(separator.ToString(), columns));

            lines.AddRange(items.Select(item => columns.Select(column => typeof(T).GetProperty(column)?.GetValue(item, null))).Select(values => string.Join(separator.ToString(), values)));

            return lines;
        }
    }
}
