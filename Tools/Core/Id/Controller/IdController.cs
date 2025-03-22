using System.Collections.Generic;

namespace GothmogToolkit.Tools.Core.Id
{
	public abstract class IdController<T>: IIdController<T>
	{
		private readonly HashSet<T> _ids = new(128);
		private T _lastId;
		public bool Register(IIdContainer<T> source)
		{
			if (source.IsSet() && _ids.Contains(source.Id))
				return false;
			var id = _lastId;
			do
			{
				id = GetNextId(id);
			} while (_ids.Contains(id));
			
			_ids.Add(id);
			_lastId = id;
			source.SetId(id);
			
			return true;
		}

		protected abstract T GetNextId(T id);
	}
}