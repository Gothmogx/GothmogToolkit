using UnityEngine;

namespace GothmogToolkit.Tools.Helpers.Proxy
{ 
	/// <summary>
	/// Implements Proxy pattern for MonoBehaviours.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractProxy<T> : MonoBehaviour where T : class
	{
		[SerializeField] private GameObject _provider;

		private T _value;

		public T Provider => _value ??= _provider.GetComponent<T>();

		private void Awake() => _value = Provider;

		private void OnValidate()
		{
			if (!_provider)
			{
				var provider = GetComponentInParent<T>();

				if (provider is MonoBehaviour monoBehaviour)
					_provider = monoBehaviour.gameObject;
			}
			
			//Do not log error if no suitable component found
			if (!_provider || _provider.GetComponent<T>() != null)
				return;

			_provider = null;
			Debug.LogError($"{this}: {nameof(_provider)} must have {typeof(T)} component");
		}
	}
}