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
    public partial class ZoneLotSizeSystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private EntityQuery _spawnableBuildings;
        private LocalizationManager _localizationManager;
        private static readonly Dictionary<string, List<(int, int)>> ZoneLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _spawnableBuildings = GetEntityQuery(new EntityQueryDesc()
            {
                All = [ComponentType.ReadWrite<SpawnableBuildingData>()]
            });
            _localizationManager = GameManager.instance.localizationManager;

            var allSpawnableBuildings = _spawnableBuildings.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allSpawnableBuildings)
            {
                _prefabSystem.TryGetPrefab(entity, out PrefabBase bldgPrefab);
                BuildingPrefab buildingPrefab = (BuildingPrefab)bldgPrefab;
                if (bldgPrefab.TryGet(out SpawnableBuilding sbd) && buildingPrefab is not null)
                {
                    string zoneName = sbd.m_ZoneType?.GetPrefabID().ToString() ?? "";
                    if (!ZoneLots.ContainsKey(zoneName))
                    {
                        ZoneLots[zoneName] = new List<(int, int)>();
                    }
                    ZoneLots[zoneName].Add((buildingPrefab.m_LotWidth, buildingPrefab.m_LotDepth));
                }
            }

            AddTextToDescriptions();
            _localizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
            Mod.log.Info("ZoneLotSizeSystem initialized");
        }

        private void AddTextToDescriptions()
        {
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
                    ? string.Join(", ", sortedLots.Take(sortedLots.Count - 1)) + " " + LocalizationProvider.GetLocalizedAnd(_localizationManager.activeLocaleId) + " " + sortedLots.Last()
                    : sortedLots.FirstOrDefault() ?? "";
                
                if (_localizationManager.activeDictionary.TryGetValue($"Assets.DESCRIPTION[{zoneName}]", out var entry))
                {
                    if (string.IsNullOrEmpty(entry)) continue;
                    string localizedText = LocalizationProvider.GetLocalizedText(_localizationManager.activeLocaleId)
                        .Replace("%data%", lotSize); // Make sure text isn't appended multiple times
                    string zoneDescNew = $"{entry}\r\n{localizedText}";
                    if (entry.Contains(localizedText)) continue;
                    _localizationManager.activeDictionary.Add($"Assets.DESCRIPTION[{zoneName}]", zoneDescNew);
                }
            }
        }

        public void OnActiveDictionaryChanged()
        {
            AddTextToDescriptions();
        }

        protected override void OnUpdate()
        {
        }
    }
}
