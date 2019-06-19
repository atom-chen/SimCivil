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
// SimCivil - SimCivil.IntegrationTest - MovementTest.cs
// Create Date: 2019/05/26
// Update Date: 2019/06/18

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Orleans;

using SimCivil.Contract;

namespace SimCivil.IntegrationTest.Testcase
{
    public class MovementTest : EntityTestBase
    {
        public MovementTest(ILogger<MovementTest> logger, IClusterClient cluster) : base(logger, cluster) { }

        public override async Task Test()
        {
            var synced = false;
            IsRunning = true;
            await UseRole();

            var sync = Client.Import<IViewSynchronizer>();
            (float x, float y) pos = (0, 0);
            float speed = 0;
            sync.RegisterViewSync(
                vc =>
                {
                    synced = true;
                    pos    = vc.Position;
                    speed  = vc.Speed;
                    if (vc.EntityChange?.Any() ?? false)
                        Logger.LogInformation(vc.EntityChange.First().ToString());
                    else
                        Logger.LogDebug(vc.ToString());
                });

            var controller = Client.Import<IPlayerController>();

            await Task.Delay(500);
            for (var i = 0; i < 100; i++)
            {
                await controller.MoveTo((pos.x + speed * 0.05f, pos.y), DateTime.UtcNow);

                await Task.Delay(50);

                if (!synced)
                    Logger.LogWarning("no view sync received");
                synced = false;
            }

            Logger.LogInformation($"{RoleName} test end");
        }
    }
}