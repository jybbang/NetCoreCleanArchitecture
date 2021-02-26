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

using Microsoft.AspNetCore.Mvc;
using NetCoreCleanArchitecture.Application.Common.Results;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.WebHosting.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            return new CustomResult(result);
        }

        public static Task<IActionResult> ToActionResultAsync(this Result result)
        {
            return Task.FromResult(new CustomResult(result) as IActionResult);
        }
    }

    public sealed class CustomResult : IActionResult
    {
        private readonly Result _result;

        internal CustomResult(Result result)
        {
            _result = result;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            if (_result.IsFailed)
            {
                return new UnprocessableEntityObjectResult(_result.Errors).ExecuteResultAsync(context);
            }

            if (_result.GetType().IsGenericType && _result.GetType().GetGenericTypeDefinition() == typeof(Result<>))
            {
                return new OkObjectResult((_result as dynamic)?.Data).ExecuteResultAsync(context);
            }

            return new NoContentResult().ExecuteResultAsync(context);
        }
    }
}
