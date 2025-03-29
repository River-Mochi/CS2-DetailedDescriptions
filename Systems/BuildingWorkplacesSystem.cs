using System.Collections.Generic;
using Colossal.Entities;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingWorkplacesSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _workplacesQuery;
        private static readonly Dictionary<string, WorkplaceData> BuildingLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _workplacesQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<WorkplaceData>() }
            });

            var buildings = _workplacesQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in buildings)
            {
                if (EntityManager.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    string prefabName = PrefabSystem.GetPrefabName(entity);
                    BuildingLots.Add(prefabName, workplaceData);
                }
            }
            
            

            AddTextToAllDescriptions();
            Mod.log.Info("BuildingLotSizeSystem initialized");
        }

        protected override void OnUpdate()
        {
            //Entity economyParamsEntity = SystemAPI.GetSingletonEntity<EconomyParameterData>();
        }

        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowBuildingWorkplaces) return;
            foreach (var item in BuildingLots)
            {
                var workplaceData = item.Value;
                var workplaceText = $"Workplaces: {workplaceData.m_MaxWorkers}";
                if (workplaceData.m_MaxWorkers > 0)
                    AddTextToDescription(item.Key, workplaceText);
            }
        } 
    }
}
