using System.Collections.Generic;

namespace GothmogToolkit.Tools.Core.Id.Controller
{
	public abstract class IdController<T>: IIdController<T>
	{
		private readonly HashSet<T> _ids=new HashSet<T>(128);
		private T _lastId;
		public bool Register(IIdContainer<T> source)
		{
			var id = source.Id;
			
			if (source.IsSet())
			{
				if (_ids.Contains(source.Id))
					return false;
			}
			else
			{
				id = _lastId;
				do
				{
					id = GetNextId(id);
				} while (_ids.Contains(id));
			}

			_ids.Add(id);
			_lastId = id;
			source.SetId(id);

			return true;
		}
		

		protected abstract T GetNextId(T id);
	}
}