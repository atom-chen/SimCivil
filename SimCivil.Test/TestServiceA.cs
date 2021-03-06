﻿// Copyright (c) 2017 TPDT
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// SimCivil - SimCivil.Test - TestServiceA.cs
// Create Date: 2019/05/05
// Update Date: 2019/06/05

using System;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

using SimCivil.Rpc.Session;
using SimCivil.Utilities.AutoService;

namespace SimCivil.Test
{
    [AutoService(ServiceLifetime.Transient)]
    public class TestServiceA : ITestServiceA
    {
        public IRpcSession Session { get; }
        public string      Name    { get; set; }

        public TestServiceA(IRpcSession session)
        {
            Session = session;
        }

        public string GetName() => Name;

        public string HelloWorld(string name)
        {
            Name = name;

            return $"Hello {name}!";
        }

        public int NotImplementedFuc(int i) => throw new NotImplementedException();

        public string GetSession(string key) => Session[key].ToString();

        public void Echo(string str, Action<string> callback)
        {
            callback(str);
        }

        public (double, double) TupleEcho((double, double) dump) => dump;

        public PropertyTuple PropertyTupleEcho(PropertyTuple dump) => dump;

        public DateTime EchoTime(DateTime time) => time;
    }
}