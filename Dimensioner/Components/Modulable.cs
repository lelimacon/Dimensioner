using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dimensioner.Components
{
    public abstract class Modulable
    {
        internal ConcurrentBag<ITaxonomyComponent> Comps { get; set; }
        private readonly ConcurrentDictionary<string, ITaxonomyComponent> _compIds;

        public Modulable()
        {
            Comps = new ConcurrentBag<ITaxonomyComponent>();
            _compIds = new ConcurrentDictionary<string, ITaxonomyComponent>();
        }

        public T Component<T>()
            where T : ITaxonomyComponent
        {
            return Comps.OfType<T>().Single();
        }

        public T Component<T>(string id)
            where T : ITaxonomyComponent
        {
            return Comps.OfType<T>().Single(c => c.Id == id);
        }

        public ITaxonomyComponent Component(string id)
        {
            return Comps.Single(c => c.Id == id);
        }

        public T GetComponent<T>()
            where T : ITaxonomyComponent
        {
            return Comps.OfType<T>().SingleOrDefault();
        }

        public T GetComponent<T>(string id)
            where T : ITaxonomyComponent
        {
            return Comps.OfType<T>().SingleOrDefault(c => c.Id == id);
        }

        public ITaxonomyComponent GetComponent(string id)
        {
            return Comps.SingleOrDefault(c => c.Id == id);
        }

        public IEnumerable<ITaxonomyComponent> Components()
        {
            return Comps;
        }

        public IEnumerable<T> Components<T>()
            where T : ITaxonomyComponent
        {
            return Comps.OfType<T>();
        }

        internal void Add(IEnumerable<ITaxonomyComponent> components)
        {
            if (components == null)
                return;
            foreach (var comp in components)
                Add(comp);
        }

        private void Add(ITaxonomyComponent comp)
        {
            Comps.Add(comp);
            string key = comp.GetType().FullName + ":" + comp.Id;
            _compIds.TryGetValue(key, out ITaxonomyComponent cmp);
            _compIds[key] = comp;
        }
    }
}
