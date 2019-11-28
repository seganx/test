using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RacerFactory
{
    public static class Path
    {
        public const string racers = "Racers/";
        public const string sharedHeights = racers + "Customs/Heights/";
        public const string sharedHorns = racers + "Customs/Horns/";
        public const string sharedHoods = racers + "Customs/Hoods/";
        public const string sharedRoofs = racers + "Customs/Roofs/";
        public const string sharedSpoilers = racers + "Customs/Spoilers/";
        public const string sharedVinyls = racers + "Customs/Vinyls/";
        public const string sharedWheels = racers + "Customs/Wheels/";
        public const string sharedColors = racers + "Customs/Colors/";
        public const string sharedColor = racers + "Customs/Colors/ColorPalette";
    }

    public static class Racer
    {
#if !UNITY_EDITOR
        private static List<RacerConfig> racers = new List<RacerConfig>();
#endif
        public static List<RacerConfig> AllConfigs
        {
            get
            {
#if UNITY_EDITOR
                var res = new List<RacerConfig>();
                var files = ResourceEx.LoadAll(Path.racers, false);
                foreach (var item in files)
                {
                    var loaded = ResourceEx.Load<RacerConfig>(item.path);
                    if (loaded != null && res.Contains(loaded) == false)
                    {
                        loaded.Id = item.id;
                        res.Add(loaded);
                    }
                }
                res.Sort((x, y) => x.Id - y.Id);
                return res;
#else
                if (racers.Count < 1)
                {
                    var files = ResourceEx.LoadAll(Path.racers, false);
                    foreach (var item in files)
                    {
                        var loaded = ResourceEx.Load<RacerConfig>(item.path);
                        if (loaded != null && racers.Contains(loaded) == false)
                        {
                            loaded.Id = item.id;
                            racers.Add(loaded);
                        }
                    }
                    racers.Sort((x, y) => x.Id - y.Id);
                }
                return racers;
#endif
            }
        }

        public static RacerConfig GetConfig(int id)
        {
            return AllConfigs.Find(x => x.Id == id);
        }

        public static RacerConfig GetConfigByIndex(int index)
        {
            return AllConfigs[Mathf.Clamp(index, 0, AllConfigs.Count - 1)];
        }

        public static RacerPresenter Create(int id, Transform parent)
        {
            var config = GetConfig(id);
            return InstantiateFromPath<RacerPresenter>(Path.racers + config.name, parent).SetId(id, config.GroupId);
        }
    }

    public static class Hood
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedHoods, Racer.GetConfig(racerId).hoods);
        }

        public static RacerCustomPresenter GetPrefab(int racerId, int id)
        {
            return ResourceEx.Load<RacerCustomPresenter>(Path.sharedHoods, id);
        }

        public static RacerCustomPresenter Create(int racerId, int id, Transform parent)
        {
            return InstantiateFromPath<RacerCustomPresenter>(Path.sharedHoods, id, parent);
        }
    }

    public static class Horns
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedHorns, Racer.GetConfig(racerId).horns);
        }

        public static RacerCustomPresenter GetPrefab(int racerId, int id)
        {
            return ResourceEx.Load<RacerCustomPresenter>(Path.sharedHorns, id);
        }

        public static RacerCustomPresenter Create(int racerId, int id, Transform parent)
        {
            return InstantiateFromPath<RacerCustomPresenter>(Path.sharedHorns, id, parent);
        }
    }

    public static class Roof
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedRoofs, Racer.GetConfig(racerId).roofs);
        }

        public static RacerCustomPresenter GetPrefab(int racerId, int id)
        {
            return ResourceEx.Load<RacerCustomPresenter>(Path.sharedRoofs, id);
        }

        public static RacerCustomPresenter Create(int racerId, int id, Transform parent)
        {
            return InstantiateFromPath<RacerCustomPresenter>(Path.sharedRoofs, id, parent);
        }
    }

    public static class Spoiler
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedSpoilers, Racer.GetConfig(racerId).spoilers);
        }

        public static RacerCustomPresenter GetPrefab(int racerId, int id)
        {
            return ResourceEx.Load<RacerCustomPresenter>(Path.sharedSpoilers, id);
        }

        public static RacerCustomPresenter Create(int racerId, int id, Transform parent)
        {
            return InstantiateFromPath<RacerCustomPresenter>(Path.sharedSpoilers, id, parent);
        }
    }

    public static class Vinyl
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedVinyls, Racer.GetConfig(racerId).vinyls);
        }

        public static RacerCustomPresenter GetPrefab(int racerId, int id)
        {
            return ResourceEx.Load<RacerCustomPresenter>(Path.sharedVinyls, id);
        }

        public static void SetVinylTexture(int racerId, int id, Material dest)
        {
            RacerMaterial.SetVinylTexture(dest, ResourceEx.Load<Texture>(Path.sharedVinyls, id));
        }

        public static void SetVinylColor(int id, Material dest)
        {
            RacerMaterial.SetVinylColor(dest, Colors.GetById(id));
        }
    }

    public static class Height
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedHeights, false);
        }
    }

    public static class Wheel
    {
        public static List<RacerCustomPresenter> GetPrefabs(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedWheels, Racer.GetConfig(racerId).wheels);
        }

        public static WheelPresenter GetPrefab(int racerId, int id)
        {
            return ResourceEx.Load<WheelPresenter>(Path.sharedWheels, id);
        }

        public static WheelPresenter Create(int racerId, int id, Transform parent)
        {
            return InstantiateFromPath<WheelPresenter>(Path.sharedWheels, id, parent);
        }
    }

    public static class Colors
    {
        private static ColorPalette instanceColorPalette = null;
        private static ColorPalette Palette { get { return instanceColorPalette == null ? (instanceColorPalette = Resources.Load<ColorPalette>(Path.sharedColor)) : instanceColorPalette; } }

        public static int Default { get { return Palette.sharedColors[0].id; } }

        public static List<RacerCustomPresenter> GetModels(int racerId)
        {
            return ResourceEx.LoadAll<RacerCustomPresenter>(Path.sharedColors, false);
        }

        public static ColorModel GetModel(int id)
        {
            return ResourceEx.Load<ColorModel>(Path.sharedColors, id);
        }

        public static void SetModel(int id, Material dest, RacerMaterial.BaseParam baseParam, int matId = 1)
        {
            var model = GetModel(id);
            if (model == null) return;
            RacerMaterial.SetMaterialModel(dest, baseParam, model, matId);
        }

        public static List<ColorPalette.GameColor> AllColors
        {
            get { return Palette.sharedColors; }
        }

        public static void SetDiffuseColor(int id, Material dest, int matId, bool alpha)
        {
            RacerMaterial.SetDiffuseColor(dest, matId, Palette.GetColorById(id), alpha);
            RacerMaterial.SetGlossColor(dest, Palette.GetGlossById(id));
        }

        public static Color GetById(int id)
        {
            return Palette.GetColorById(id);
        }
    }

    #region Private helper function
    private static T InstantiateFromPath<T>(string path, int id, Transform parent) where T : Component
    {
        var prefab = ResourceEx.Load<T>(path, id);
        if (prefab == null) return null;
        var res = parent != null ? prefab.Clone<T>(parent) : prefab.Clone<T>();
        res.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        return res;
    }

    private static T InstantiateFromPath<T>(string path, Transform parent) where T : Component
    {
        var prefab = ResourceEx.Load<T>(path);
        if (prefab == null) return null;
        var res = parent != null ? prefab.Clone<T>(parent) : prefab.Clone<T>();
        res.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        return res;
    }
    #endregion
}