using System.Collections.Generic;
using System.Linq;
using Colossal.Entities;
using Colossal.Localization;
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
                All = [ComponentType.ReadWrite<BuildingData>()]
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
            foreach (var item in BuildingLots)
            {
                int width = item.Value.Item1;
                int depth = item.Value.Item2;
                var lotText = $"Lot Size: {width}x{depth}";
                if (Setting.Instance.ShowLotSizeUnits)
                {
                    if (IsMetric())
                    {
                        lotText += $" ({width*8}m x {depth * 8}m)";
                    }
                    else
                    {
                        // One unit is 8 meters, convert to feet
                        lotText += $" ({width*8/0.3048f}ft x {depth*8/0.3048f}ft)";
                    }
                }
                AddTextToDescription(item.Key, lotText);
            }
        } 
        
        private static bool IsMetric()
        {
            return GameManager.instance?.settings?.userInterface?.unitSystem is null or InterfaceSettings.UnitSystem.Metric;
        }
    }
}
