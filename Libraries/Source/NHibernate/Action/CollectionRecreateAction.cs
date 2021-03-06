using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class CollectionRecreateAction : CollectionAction
	{
		public CollectionRecreateAction(IPersistentCollection collection, ICollectionPersister persister, object key, ISessionImplementor session)
			: base(persister, collection, key, session) { }

		/// <summary> Execute this action</summary>
		public override void Execute()
		{
			IPersistentCollection collection = Collection;

			Persister.Recreate(collection, Key, Session);

			Session.PersistenceContext.GetCollectionEntry(collection).AfterAction(collection);

			Evict();

			if (Session.Factory.Statistics.IsStatisticsEnabled)
			{
			  Session.Factory.StatisticsImplementor.RecreateCollection(Persister.Role);
			}
		}
	}
}