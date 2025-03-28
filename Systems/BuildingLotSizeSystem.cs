using System.Collections.Generic;
using System.Linq;
using Colossal.Entities;
using Colossal.Localization;
using DetailedDescriptions.Helpers;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Settings;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingLotSizeSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _buildingsQuery;
        private static readonly Dictionary<string, (int, int)> BuildingLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _buildingsQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<BuildingData>() }
            });

            var buildings = _buildingsQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in buildings)
            {
                if (EntityManager.TryGetComponent(entity, out BuildingData buildingData))
                {
                    string prefabName = PrefabSystem.GetPrefabName(entity);
                    BuildingLots.Add(prefabName, (buildingData.m_LotSize.x, buildingData.m_LotSize.y));
                }
            }

            AddTextToAllDescriptions();
            Mod.log.Info("BuildingLotSizeSystem initialized");
        }

        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowBuildingLotSizes) return;
            foreach (var item in BuildingLots)
            {
                int width = item.Value.Item1;
                int depth = item.Value.Item2;
                var lotText = $"Lot Size: {width}x{depth}";
                if (Setting.Instance.ShowLotSizeUnits)
                {
                    lotText += $" ({UnitHelper.FormatUnit(width)} x {UnitHelper.FormatUnit(depth)})";
                }
                AddTextToDescription(item.Key, lotText);
            }
        } 
    }
}
