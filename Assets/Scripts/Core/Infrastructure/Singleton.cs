using UnityEngine;

namespace Core.Infrastructure.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _ins;

        public static T Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = FindObjectOfType<T>(true);
                    if (_ins != null)
                    {
                        (_ins as Singleton<T>)?.OnSingletonInitialize();
                    }
                }

                return _ins;
            }
            set => _ins = value;
        }

        protected virtual void Awake()
        {
            if (_ins != null && _ins != this)
            {
                Destroy(gameObject);
                return;
            }

            _ins = this as T;
            OnSingletonInitialize();
        }

        protected virtual void OnSingletonInitialize() { }

        protected virtual void OnDestroy()
        {
            if (_ins == this)
                _ins = null;
        }

        protected virtual void OnApplicationQuit()
        {
            Ins = null;
            Destroy(gameObject);
        }
    }

    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void OnSingletonInitialize()
        {
            base.OnSingletonInitialize();
            DontDestroyOnLoad(gameObject);
        }
    }
}