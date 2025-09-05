using System.Collections.Generic;
using System.Linq;
using Colossal.Localization;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class ZoneLotSizeSystem : AssetDescriptionDisplaySystem
    {
        private PrefabSystem _prefabSystem = null!;
        private EntityQuery _spawnableBuildings;
        private static readonly Dictionary<string, List<(int, int)>> ZoneLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _spawnableBuildings = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<SpawnableBuildingData>() }
            });

            GameManager.instance.RegisterUpdater(AddTextToAllDescriptions);
            Mod.log.Info("ZoneLotSizeSystem initialized");
        }
        
        protected override void AddTextToAllDescriptions()
        {
            if (Setting.Instance == null || !Setting.Instance.ShowZoneLotSizes) return;
            if (_prefabSystem == null) return;

            ZoneLots.Clear();   // avoid carry-over between runs or language switches

            using var allSpawnableBuildings = _spawnableBuildings.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allSpawnableBuildings)
            {
                // Guard: prefab may not resolve in some saves, skip those
                if (!_prefabSystem.TryGetPrefab(entity, out PrefabBase bldgPrefab) || bldgPrefab == null)
                    continue;

                // Guard: only BuildingPrefab has lot size, skip others
                if (!(bldgPrefab is BuildingPrefab))
                    continue;
                var buildingPrefab = (BuildingPrefab)bldgPrefab;

                // Guard: SpawnableBuilding required to get m_ZoneType, skip others
                if (!bldgPrefab.TryGet(out SpawnableBuilding sbd))
                    continue;

                string zoneName = sbd.m_ZoneType?.GetPrefabID().ToString() ?? "";
                if (string.IsNullOrEmpty(zoneName))
                    continue;

                if (!ZoneLots.ContainsKey(zoneName))
                    ZoneLots[zoneName] = new List<(int, int)>();

                // Optional: skip invalid sizes
                if (buildingPrefab.m_LotWidth > 0 && buildingPrefab.m_LotDepth > 0)
                    ZoneLots[zoneName].Add((buildingPrefab.m_LotWidth, buildingPrefab.m_LotDepth));
                
            }
            
            foreach (var item in ZoneLots)
            {
                string zoneName = item.Key.Replace("ZonePrefab:","");
                if (zoneName == "") continue;
                
                var sortedLots = item.Value
                    .OrderBy(lot => lot.Item1)
                    .ThenBy(lot => lot.Item2)
                    .Select(lot => $"{lot.Item1}x{lot.Item2}")
                    .Distinct()
                    .ToList();

                string lotSize = sortedLots.Count > 1
                    ? string.Join(", ", sortedLots.Take(sortedLots.Count - 1)) + " " + LocalizationProvider.GetLocalizedAnd(LocalizationManager.activeLocaleId) + " " + sortedLots.Last()
                    : sortedLots.FirstOrDefault() ?? "";
                
                string localizedText = LocalizationProvider.GetLocalizedText(LocalizationManager.activeLocaleId).Replace("%data%", lotSize);
                
                AddTextToDescription(zoneName, localizedText);
            }
        } 

        protected override void OnUpdate()
        {
        }
    }
}
