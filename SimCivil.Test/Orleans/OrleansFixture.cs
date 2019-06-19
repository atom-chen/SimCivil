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
// SimCivil - SimCivil.Test - OrleansFixture.cs
// Create Date: 2019/05/08
// Update Date: 2019/06/18

using System;
using System.Text;

using Orleans.TestingHost;

using Xunit;

namespace SimCivil.Test.Orleans
{
    [CollectionDefinition(Name)]
    public class ClusterCollection : ICollectionFixture<OrleansFixture>
    {
        public const string Name = "ClusterCollection";
    }

    public class OrleansFixture : IDisposable
    {
        public TestCluster Cluster { get; }

        public OrleansFixture( /*ITestOutputHelper outputHelper*/)
        {
            var builder = new TestClusterBuilder();

            //TestSiloConfigurator.OutputHelper = outputHelper;
            builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();

            Cluster = builder.Build();
            Cluster.Deploy();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Cluster.StopAllSilos();
        }
    }
}