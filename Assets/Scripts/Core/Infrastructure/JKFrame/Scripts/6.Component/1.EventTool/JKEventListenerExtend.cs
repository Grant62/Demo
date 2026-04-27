using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JKFrame
{
    public static class JKEventListenerExtend
    {
        #region 工具函数
        private static JKEventListener GetOrAddJKEventListener(Component com)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis == null) return com.gameObject.AddComponent<JKEventListener>();
            return lis;
        }

        public static void AddEventListener<T>(this Component com, JKEventType eventType, Action<T> action)
        {
            com.AddEventListener((int)eventType, action);
        }

        public static void AddEventListener<T>(this Component com, int customEventTypeInt, Action<T> action)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.AddListener(customEventTypeInt, action);
        }

        public static void AddEventListener<T, TEventArg>(this Component com, JKEventType eventType, Action<T, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener((int)eventType, action, args);
        }

        public static void AddEventListener<T, TEventArg>(this Component com, int customEventTypeInt, Action<T, TEventArg> action, TEventArg args = default)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.AddListener(customEventTypeInt, action, args);
        }

        public static void RemoveEventListener<T, TEventArg>(this Component com, int customEventTypeInt, Action<T, TEventArg> action)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveListener(customEventTypeInt, action);
        }

        public static void RemoveEventListener<T, TEventArg>(this Component com, JKEventType eventType, Action<T, TEventArg> action)
        {
            com.RemoveEventListener((int)eventType, action);
        }

        public static void RemoveEventListener<T>(this Component com, int customEventTypeInt, Action<T> action)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveListener(customEventTypeInt, action);
        }

        public static void RemoveEventListener<T>(this Component com, JKEventType eventType, Action<T> action)
        {
            com.RemoveEventListener((int)eventType, action);
        }

        public static void RemoveAllListener(this Component com, int customEventTypeInt)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveAllListener(customEventTypeInt);
        }

        public static void RemoveAllListener(this Component com, JKEventType eventType)
        {
            com.RemoveAllListener((int)eventType);
        }

        public static void RemoveAllListener(this Component com)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveAllListener();
        }

        public static void TriggerCustomEvent<T>(this Component com, int customEventTypeInt, T eventData)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.TriggerAction(customEventTypeInt, eventData);
        }
        #endregion

        #region 鼠标相关事件
        public static void OnMouseEnter<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnMouseEnter, action, args);
        }

        public static void OnMouseEnter(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnMouseEnter, action);
        }

        public static void OnMouseExit<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnMouseExit, action, args);
        }

        public static void OnMouseExit(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnMouseExit, action);
        }

        public static void OnClick<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnClick, action, args);
        }

        public static void OnClick(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnClick, action);
        }


        public static void OnClickDown<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnClickDown, action, args);
        }

        public static void OnClickDown(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnClickDown, action);
        }

        public static void OnClickUp<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnClickUp, action, args);
        }

        public static void OnClickUp(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnClickUp, action);
        }


        public static void OnDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnDrag, action, args);
        }

        public static void OnDrag(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnDrag, action);
        }

        public static void OnBeginDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnBeginDrag, action, args);
        }

        public static void OnBeginDrag(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnBeginDrag, action);
        }

        public static void OnEndDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnEndDrag, action, args);
        }

        public static void OnEndDrag(this Component com, Action<PointerEventData> action)
        {
            com.AddEventListener(JKEventType.OnEndDrag, action);
        }

        public static void RemoveOnClick<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnClick, action);
        }

        public static void RemoveOnClick(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnClick, action);
        }

        public static void RemoveOnClickDown<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnClickDown, action);
        }

        public static void RemoveOnClickDown(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnClickDown, action);
        }

        public static void RemoveOnMouseEnter<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnMouseEnter, action);
        }

        public static void RemoveOnMouseEnter(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnMouseEnter, action);
        }

        public static void RemoveOnMouseExit<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnMouseExit, action);
        }

        public static void RemoveOnMouseExit(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnMouseExit, action);
        }

        public static void RemoveOnClickUp<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnClickUp, action);
        }

        public static void RemoveOnClickUp(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnClickUp, action);
        }

        public static void RemoveOnDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnDrag, action);
        }

        public static void RemoveOnDrag(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnDrag, action);
        }

        public static void RemoveOnBeginDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnBeginDrag, action);
        }

        public static void RemoveOnBeginDrag(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnBeginDrag, action);
        }

        public static void RemoveOnEndDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnEndDrag, action);
        }

        public static void RemoveOnEndDrag(this Component com, Action<PointerEventData> action)
        {
            com.RemoveEventListener(JKEventType.OnEndDrag, action);
        }
        #endregion

        #region 碰撞相关事件
        public static void OnCollisionEnter<TEventArg>(this Component com, Action<Collision, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnCollisionEnter, action, args);
        }

        public static void OnCollisionEnter(this Component com, Action<Collision> action)
        {
            com.AddEventListener(JKEventType.OnCollisionEnter, action);
        }


        public static void OnCollisionStay<TEventArg>(this Component com, Action<Collision, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnCollisionStay, action, args);
        }

        public static void OnCollisionStay(this Component com, Action<Collision> action)
        {
            com.AddEventListener(JKEventType.OnCollisionStay, action);
        }

        public static void OnCollisionExit<TEventArg>(this Component com, Action<Collision, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnCollisionExit, action, args);
        }

        public static void OnCollisionExit(this Component com, Action<Collision> action)
        {
            com.AddEventListener(JKEventType.OnCollisionExit, action);
        }

        public static void OnCollisionEnter2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnCollisionEnter2D, action, args);
        }

        public static void OnCollisionEnter2D(this Component com, Action<Collision2D> action)
        {
            com.AddEventListener(JKEventType.OnCollisionEnter2D, action);
        }

        public static void OnCollisionStay2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnCollisionStay2D, action, args);
        }

        public static void OnCollisionStay2D(this Component com, Action<Collision2D> action)
        {
            com.AddEventListener(JKEventType.OnCollisionStay2D, action);
        }

        public static void OnCollisionExit2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnCollisionExit2D, action, args);
        }

        public static void OnCollisionExit2D(this Component com, Action<Collision2D> action)
        {
            com.AddEventListener(JKEventType.OnCollisionExit2D, action);
        }

        public static void RemoveOnCollisionEnter<TEventArg>(this Component com, Action<Collision, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionEnter, action);
        }

        public static void RemoveOnCollisionEnter(this Component com, Action<Collision> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionEnter, action);
        }

        public static void RemoveOnCollisionStay<TEventArg>(this Component com, Action<Collision, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionStay, action);
        }

        public static void RemoveOnCollisionStay(this Component com, Action<Collision> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionStay, action);
        }

        public static void RemoveOnCollisionExit<TEventArg>(this Component com, Action<Collision, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionExit, action);
        }

        public static void RemoveOnCollisionExit(this Component com, Action<Collision> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionExit, action);
        }

        public static void RemoveOnCollisionEnter2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionEnter2D, action);
        }

        public static void RemoveOnCollisionEnter2D(this Component com, Action<Collision2D> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionEnter2D, action);
        }

        public static void RemoveOnCollisionStay2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionStay2D, action);
        }

        public static void RemoveOnCollisionStay2D(this Component com, Action<Collision2D> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionStay2D, action);
        }

        public static void RemoveOnCollisionExit2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionExit2D, action);
        }

        public static void RemoveOnCollisionExit2D(this Component com, Action<Collision2D> action)
        {
            com.RemoveEventListener(JKEventType.OnCollisionExit2D, action);
        }
        #endregion

        #region 触发相关事件
        public static void OnTriggerEnter<TEventArg>(this Component com, Action<Collider, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnTriggerEnter, action, args);
        }

        public static void OnTriggerEnter(this Component com, Action<Collider> action)
        {
            com.AddEventListener(JKEventType.OnTriggerEnter, action);
        }

        public static void OnTriggerStay<TEventArg>(this Component com, Action<Collider, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnTriggerStay, action, args);
        }

        public static void OnTriggerStay(this Component com, Action<Collider> action)
        {
            com.AddEventListener(JKEventType.OnTriggerStay, action);
        }

        public static void OnTriggerExit<TEventArg>(this Component com, Action<Collider, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnTriggerExit, action, args);
        }

        public static void OnTriggerExit(this Component com, Action<Collider> action)
        {
            com.AddEventListener(JKEventType.OnTriggerExit, action);
        }

        public static void OnTriggerEnter2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnTriggerEnter2D, action, args);
        }

        public static void OnTriggerEnter2D(this Component com, Action<Collider2D> action)
        {
            com.AddEventListener(JKEventType.OnTriggerEnter2D, action);
        }

        public static void OnTriggerStay2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnTriggerStay2D, action, args);
        }

        public static void OnTriggerStay2D(this Component com, Action<Collider2D> action)
        {
            com.AddEventListener(JKEventType.OnTriggerStay2D, action);
        }

        public static void OnTriggerExit2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnTriggerExit2D, action, args);
        }

        public static void OnTriggerExit2D(this Component com, Action<Collider2D> action)
        {
            com.AddEventListener(JKEventType.OnTriggerExit2D, action);
        }

        public static void RemoveOnTriggerEnter<TEventArg>(this Component com, Action<Collider, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerEnter, action);
        }

        public static void RemoveOnTriggerEnter(this Component com, Action<Collider> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerEnter, action);
        }

        public static void RemoveOnTriggerStay<TEventArg>(this Component com, Action<Collider, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerStay, action);
        }

        public static void RemoveOnTriggerStay(this Component com, Action<Collider> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerStay, action);
        }

        public static void RemoveOnTriggerExit<TEventArg>(this Component com, Action<Collider, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerExit, action);
        }

        public static void RemoveOnTriggerExit(this Component com, Action<Collider> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerExit, action);
        }

        public static void RemoveOnTriggerEnter2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerEnter2D, action);
        }

        public static void RemoveOnTriggerEnter2D(this Component com, Action<Collider2D> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerEnter2D, action);
        }

        public static void RemoveOnTriggerStay2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerStay2D, action);
        }

        public static void RemoveOnTriggerStay2D(this Component com, Action<Collider2D> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerStay2D, action);
        }

        public static void RemoveOnTriggerExit2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerExit2D, action);
        }

        public static void RemoveOnTriggerExit2D(this Component com, Action<Collider2D> action)
        {
            com.RemoveEventListener(JKEventType.OnTriggerExit2D, action);
        }
        #endregion

        #region 资源相关事件
        public static void OnReleaseAddressableAsset<TEventArg>(this Component com, Action<GameObject, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnReleaseAddressableAsset, action, args);
        }

        public static void OnReleaseAddressableAsset(this Component com, Action<GameObject> action)
        {
            com.AddEventListener(JKEventType.OnReleaseAddressableAsset, action);
        }

        public static void RemoveOnReleaseAddressableAsset<TEventArg>(this Component com, Action<GameObject, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnReleaseAddressableAsset, action);
        }

        public static void RemoveOnReleaseAddressableAsset(this Component com, Action<GameObject> action)
        {
            com.RemoveEventListener(JKEventType.OnReleaseAddressableAsset, action);
        }

        public static void OnDestroy<TEventArg>(this Component com, Action<GameObject, TEventArg> action, TEventArg args = default)
        {
            com.AddEventListener(JKEventType.OnDestroy, action, args);
        }

        public static void OnDestroy(this Component com, Action<GameObject> action)
        {
            com.AddEventListener(JKEventType.OnDestroy, action);
        }

        public static void RemoveOnDestroy<TEventArg>(this Component com, Action<GameObject, TEventArg> action)
        {
            com.RemoveEventListener(JKEventType.OnDestroy, action);
        }

        public static void RemoveOnDestroy(this Component com, Action<GameObject> action)
        {
            com.RemoveEventListener(JKEventType.OnDestroy, action);
        }
        #endregion
    }
}