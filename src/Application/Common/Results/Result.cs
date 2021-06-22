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
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Results
{
    public class Result
    {
        protected Result() { }

        internal Result(bool succeeded, IEnumerable<string> errors = null)
        {
            Succeeded = succeeded;
            Errors = errors?.ToArray() ?? Array.Empty<string>();
        }

        public bool Succeeded { get; }

        public string[] Errors { get; }

        public bool IsSucceeded => Succeeded;

        public bool IsFailed => !Succeeded;

        public bool HasError => Errors.Any();

        public static Result Fail() => new Result(false);

        public static Result Fail(params string[] errors) => new Result(false, errors);

        public static Task<Result> FailAsync() => Task.FromResult(Fail());

        public static Task<Result> FailAsync(params string[] errors) => Task.FromResult(Fail(errors));

        public static Result Success() => new Result(true);

        public static Task<Result> SuccessAsync() => Task.FromResult(Success());
    }

    public class Result<T> : Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors = null, T data = default)
            : base(succeeded, errors)
        {
            Data = data;
        }

        public T Data { get; }

        public static new Result<T> Fail() => new Result<T>(false);

        public static new Result<T> Fail(params string[] errors) => new Result<T>(false, errors);

        public static new Task<Result<T>> FailAsync() => Task.FromResult(Fail());

        public static new Task<Result<T>> FailAsync(params string[] errors) => Task.FromResult(Fail(errors));

        public static Result<T> Success(T data) => new Result<T>(true, null, data);

        public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));
    }
}
