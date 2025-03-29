using System.Collections.Generic;
using Colossal.Entities;
using DetailedDescriptions.Helpers;
using Game.Common;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class SpeedLimitSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _roadsQuery;
        private static readonly Dictionary<string, RoadData> SpeedLimits = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _roadsQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<RoadData>() }
            });

            AddTextToAllDescriptions();
            Mod.log.Info("BuildingLotSizeSystem initialized");
        }

        protected override void OnUpdate()
        {
            //Entity economyParamsEntity = SystemAPI.GetSingletonEntity<EconomyParameterData>();
        }

        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowRoadSpeedLimit) return;
            
            var roads = _roadsQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in roads)
            {
                if (EntityManager.TryGetComponent(entity, out RoadData roadData))
                {
                    // Half speed limit
                    roadData.m_SpeedLimit /= 2;
                    //EntityManager.SetComponentData(entity, roadData);
                    //EntityManager.AddComponent<BatchesUpdated>(entity);
                    
                    string prefabName = PrefabSystem.GetPrefabName(entity);
                    SpeedLimits.Add(prefabName, roadData);
                }
            }
            
            foreach (var item in SpeedLimits)
            {
                var speedText = $"Speed Limit: {UnitHelper.FormatSpeedLimit(item.Value.m_SpeedLimit)}";
                AddTextToDescription(item.Key, speedText);
            }
        } 
    }
}
