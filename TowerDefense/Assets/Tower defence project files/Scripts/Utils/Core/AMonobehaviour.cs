using UnityEngine;

namespace Andrius.Core
{
    public class AMonobehaviour : MonoBehaviour
    {
		void Awake() => OnAwake();
        void Start() => OnStart();
        void OnDestroy() => OnReleaceResources();

		protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnReleaceResources() { }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}

