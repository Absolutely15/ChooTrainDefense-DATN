using ATSoft;
using ATSoft.Ads;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NVTT.Utilities;

namespace NVTT
{
    public static class UIUtilities
    {
        public static bool IsMaxLevel(int cur, int max) => cur == max;

        public static bool IsEnoughGold(int amount) => PlayerSave.Gold >= amount;
        
        public static bool IsEnoughDiamond(int amount) => PlayerSave.Diamond >= amount;

        public static bool InteractableBtnResponse(this Transform ui, int amount)
        {
            if (IsEnoughGold(amount))
            {
                InteractableResponse(ui);
                return true;
            }
            NotInteractableResponse(ui);
            return false;
        }
        public static bool InteractableBtnDiamondResponse(this Transform ui, int amount)
        {
            if (IsEnoughDiamond(amount))
            {
                InteractableResponse(ui);
                return true;
            }
            NotInteractableResponse(ui);
            return false;
        }
        public static void InteractableBtnResponse(this Transform ui, int amount, int next, int max, bool ads, UnityAction action, string placement)
        {
            if (IsMaxLevel(next, max))
            {
                NotInteractableResponse(ui);
                return;
            }
            
            if (IsEnoughGold(amount))
            {
                InteractableResponse(ui);
                PlayerSave.Gold -= amount;
                action?.Invoke();
                //Event
                AnalyticsManager.LogEventGoldSpend(amount, "upgrade", placement);
                return;
            }

            if (!IsShowUpgradeRewardAds)
            {
                NotInteractableResponse(ui);
                return;
            }
            
            if (!ads)
                NotInteractableResponse(ui);
            
            //Already if check ads available inside showAds function 
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                if (isSuccess)
                {
                    InteractableResponse(ui);
                    action?.Invoke();
                }
                else
                    NotInteractableResponse(ui);
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, placement);
        }
        public static void InteractableBtnDiamondResponse(this Transform ui, int amount, bool ads, UnityAction action, string placement)
        {
            if (IsEnoughDiamond(amount))
            {
                InteractableResponse(ui);
                PlayerSave.Diamond -= amount;
                action?.Invoke();
                return;
            }

            if (!IsPassBasicTutorial)
            {
                NotInteractableResponse(ui);
                return;
            }

            if (!ads)
                NotInteractableResponse(ui);
            
            //Already check ads available inside showAds function 
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                if (isSuccess)
                {
                    InteractableResponse(ui);
                    action?.Invoke();
                }
                else
                {
                    NotInteractableResponse(ui);
                }

            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, placement);
        }

        public static void EnoughDiamondTextResponse(int amount, TMP_Text text)
        {
            text.text = amount.ToString();
            text.color = IsEnoughDiamond(amount) ? Color.white : Color.red;
        }
        public static void EnoughGoldTextResponse(int amount, TMP_Text text)
        {
            text.text = amount.ToString();
            text.color = IsEnoughGold(amount) ? Color.white : Color.red;
        }

        public static void InteractableResponse(this Transform trans) => 
            trans.DOScale(Vector3.one * 1.35f, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => trans.DOScale(1, 0.15f).SetEase(Ease.InSine));
        

        public static void NotInteractableResponse(this Transform trans) => trans.DOShakePosition(0.2f, Vector3.right * 50f, 50, 0).SetUpdate(true);

        public static void Fade<T>(this T image, float alpha) where T : Graphic
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}

