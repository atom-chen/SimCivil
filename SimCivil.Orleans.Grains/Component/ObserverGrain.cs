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
// SimCivil - SimCivil.Orleans.Grains - ObserverGrain.cs
// Create Date: 2019/05/13
// Update Date: 2019/05/31

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SimCivil.Contract;
using SimCivil.Orleans.Interfaces;
using SimCivil.Orleans.Interfaces.Component;
using SimCivil.Orleans.Interfaces.Option;
using SimCivil.Orleans.Interfaces.Service;

namespace SimCivil.Orleans.Grains.Component
{
    [PublicAPI]
    public class ObserverGrain : BaseGrain<ObserverState>, IObserver
    {
        public IMapService           MapService  { get; }
        public IOptions<SyncOptions> SyncOptions { get; }
        public HashSet<Guid>         Entities    { get; set; }

        public ObserverGrain(ILoggerFactory factory, IMapService mapService, IOptions<SyncOptions> syncOptions) : base(
            factory)
        {
            MapService  = mapService;
            SyncOptions = syncOptions;
        }

        public Task OnEntityEntered(Guid id)
        {
            Entities.Add(id);

            return Task.CompletedTask;
        }

        public Task OnEntityLeft(Guid id)
        {
            Entities.Remove(id);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task OnTileChanged(Tile tile) => throw new NotImplementedException();

        public Task<uint> GetNotifyRange() => GrainFactory.Get<IUnit>(this).GetSightRange();

        public Task<IEnumerable<Guid>> PopAllEntities()
        {
            var entities = Entities.ToArray();
            Entities.Clear();

            return Task.FromResult((IEnumerable<Guid>) entities);
        }

        public async Task<ViewChange> UpdateView()
        {
            PositionState position = await GrainFactory.Get<IPosition>(this).GetData();
            var viewChange = new ViewChange
            {
                Position   = position,
                AtlasIndex = position.Tile.DivDown(SyncOptions.Value.ChunkSize)
            };

            if (await GrainFactory.GetEntity(this).Has<IUnit>())
                viewChange.Speed = await GrainFactory.Get<IUnit>(this).GetMoveSpeed();

            var entities = await Task.WhenAll(Entities.Select(ToEntityDto));
            Entities.Clear();

            viewChange.EntityChange = entities;

            return viewChange;
        }

        private async Task<EntityDto> ToEntityDto(Guid e)
        {
            var entity = GrainFactory.GetGrain<IEntity>(e);
            PositionState position = await GrainFactory.Get<IPosition>(e).GetData();
            float? maxSpeed = null;
            float? hp = null;
            if (await entity.Has<IUnit>())
            {
                var unit = GrainFactory.Get<IUnit>(e);
                hp       = await unit.GetHp();
                maxSpeed = await MapService.GetEntityActualMaxSpeed(entity);
            }

            var dto = new EntityDto
            {
                Name     = await entity.GetName(),
                Pos      = position,
                Id       = e,
                Hp       = hp,
                MaxSpeed = maxSpeed
            };

            return dto;
        }

        /// <summary>
        /// This method is called at the end of the process of activating a grain.
        /// It is called before any messages have been dispatched to the grain.
        /// For grains with declared persistent state, this method is called after the State property has been populated.
        /// </summary>
        public override async Task OnActivateAsync()
        {
            Entities = new HashSet<Guid>();

            await base.OnActivateAsync();
        }
    }
}