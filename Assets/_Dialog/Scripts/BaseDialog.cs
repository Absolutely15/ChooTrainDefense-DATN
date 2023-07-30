using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace ATSoft
{
    public class BaseDialog : MonoBehaviour
    {
        public static Stack<BaseDialog> StackBox = new Stack<BaseDialog>();

        // public static BaseDialog currentBaseDialog
        // {
        //     get
        //     {
        //         return StackBox.Count > 0 ? StackBox.Peek() : null;
        //     }
        // }

        public delegate void BoxCallbackDelegate();

        public BoxCallbackDelegate OnCloseBox;
        [SerializeField] protected RectTransform mainPanel;

        //[HideInInspector] public BackObj backObj;

        [SerializeField] protected bool isAnim = true;

        public UnityAction OnAppearDone;
        public UnityAction actionCloseBase;

        public bool isStack = true;
        private Canvas canvas;

        protected virtual void Awake()
        {
            canvas = this.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.worldCamera = Camera.main;
                canvas.sortingLayerID = SortingLayer.NameToID("GUI");
            }
        }

        protected virtual void Start()
        {
        }

        // public virtual void OnClickCloseButton()
        // {
        //     CloseCurrentBox();
        // }

        protected virtual void OnStart()
        {
            // if (backObj == null)
            //     backObj = this.GetComponent<BackObj>();
            // if (backObj != null)
            // {
            //     backObj.timeAnimClose = 0;
            //     backObj.actionDoOff = ActionDoOff;
            // }

            if(StackBox == null) return;
            if(StackBox.Count <= 0) return;
            
            var dialogs = StackBox.ToArray(); //FindObjectsOfType<BaseDialog>();
            int layerMax = 0;
            for (int i = 0; i < dialogs.Length; i++)
            {
                if (dialogs[i] == null) continue;
                if (!dialogs[i].gameObject.activeInHierarchy) continue;

                var boxCanvas = dialogs[i].GetComponent<Canvas>();
                if (boxCanvas != null)
                {
                    if (boxCanvas.sortingOrder >= layerMax)
                    {
                        layerMax = boxCanvas.sortingOrder;
                    }
                }
            }

            if(canvas != null) canvas.sortingOrder = layerMax + 1;
        }

        protected virtual void ActionDoOff()
        {
        }

        protected virtual void DoAppear()
        {
            if (isAnim)
            {
                if (mainPanel != null)
                {
                    mainPanel.localScale = Vector3.zero;
                    mainPanel.DOScale(1, 0.5f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        OnAppearDone?.Invoke();
                    });
                }
            }
            else
            {
                OnAppearDone?.Invoke();
            }
        }

        protected virtual void OnEnable()
        {
            if (!isStack)
                return;
            // if (currentBaseDialog != null && currentBaseDialog != this)
            // {
            //     currentBaseDialog.Hide();
            // }

            StackBox.Push(this);
            DoAppear();
            OnStart();
        }

        protected virtual void OnDisable()
        {
            if (!isStack)
                return;
            if (actionCloseBase != null)
            {
                actionCloseBase();
                actionCloseBase = null;
            }
        }

        protected virtual void OnDestroy()
        {
            
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        protected void DestroyBox()
        {
            OnCloseBox?.Invoke();
            // if (currentBaseDialog != null)
            // {
            //     StackBox.Pop();
            // }
            Destroy(gameObject);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        // public virtual bool CloseCurrentBox()
        // {
        //     if (backObj == null)
        //         return false;
        //
        //     if (!backObj.isDoOffed)
        //     {
        //         backObj.DoOff();
        //         backObj.isDoOffed = true;
        //         return false;
        //     }
        //
        //     return false;
        // }

        // public virtual void Close()
        // {
        //     if (backObj == null)
        //         return;
        //
        //     backObj.DoOff();
        // }
    }
}