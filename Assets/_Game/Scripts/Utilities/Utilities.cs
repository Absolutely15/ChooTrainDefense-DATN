using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NVTT
{
    [DefaultExecutionOrder(-4)]
    public static class Utilities
    {
        #region Cache Data
        private static readonly Dictionary<float, WaitForSeconds> Wfs = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWfs(float key)
        {
            if (!Wfs.ContainsKey(key))
            {
                Wfs[key] = new WaitForSeconds(key);
            }

            return Wfs[key];
        }
        public static GPRainbowDefense Gameplay => GPExecutor.Instance.gameplay;
        public static PlayerController Player => Gameplay.player;
        public static MapManager MapManager => MapManager.Instance;
        public static GameDatabase GameDB => GameDatabase.Instance;

        #endregion

        #region Rotation
        public static void LookAtDirection(this Transform trans, Vector3 direction, float rotateSpeed = 50f, bool isIgnoreYAxis = false)
        {
            if (isIgnoreYAxis)
                direction.y = 0;
		
            trans.rotation = Quaternion.Lerp(
                trans.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotateSpeed);
        }
        public static float GetShiftedAngle(int wayIndex, float baseAngle, float betweenAngle)
        {
            var angle = wayIndex % 2 == 0 ?
                baseAngle - betweenAngle * wayIndex / 2f :
                baseAngle + betweenAngle * Mathf.Ceil(wayIndex / 2f);
            return angle;
        }
        public static Vector2 Rotate(this Vector2 v, float degrees) {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            v.x = cos * v.x - sin * v.y;
            v.y = sin * v.x + cos * v.y;
            return v;
        }
        #endregion

        #region Tutorial
        public static bool IsPassBasicTutorial => PlayerSave.CurrentGameLevel + 1 >= 5 || Gameplay.tutorial.skipTutorial;
        public static bool IsPassLevelTutAndRebuildTut => IsPassBasicTutorial || (PlayerSave.CurrentGameLevel >= 2 && PlayerPrefs.GetInt("rebuild_tutorial", 0) == 1);
        public static bool IsInventoryBtnActive => MapManager.id != 0 || MapManager.room[2].activeInHierarchy || Gameplay.tutorial.skipTutorial;
        public static bool IsShowUpgradeRewardAds => PlayerSave.CurrentGameLevel + 1 >= 7;

        #endregion
        
        #region Util
        public static T Rand<T>(this IList<T> list, int from)
        {
            return list[Random.Range(from, list.Count)];
        }

        public static void UpgradeFeedback(this Transform transform, Action action = null)
        {
            transform.DOScale(0f, 0.2f).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    action?.Invoke();
                    transform.DOScale(1f, 0.2f).SetEase(Ease.InOutSine);
                });
        }
        #endregion
    }
}

