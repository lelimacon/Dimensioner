using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Labels
{
    public class GenericLabelReader : TaxonomyComponentReader
    {
        private const string LabelArcrole = "http://xbrl.org/arcrole/2008/element-label";
        private readonly ConcurrentBag<KeyValuePair<Href, Label>> _labels;
        private readonly ConcurrentBag<Label> _orphanLabels;
        private readonly ConcurrentDictionary<string, Linkbase> _linkbases;

        public IReadOnlyCollection<Label> OrphanLabels => _orphanLabels;

        public GenericLabelReader()
        {
            _labels = new ConcurrentBag<KeyValuePair<Href, Label>>();
            _orphanLabels = new ConcurrentBag<Label>();
            _linkbases = new ConcurrentDictionary<string, Linkbase>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            _linkbases[linkbase.Path] = linkbase;

            // Filter on generic linkbases.
            if (!string.IsNullOrEmpty(linkbase.Role))
                return null;
            bool hasLabelArcrole = document.Root
                .Children(Ns.Link, "arcroleRef")
                .Any(n => n.Attr("arcroleURI") == LabelArcrole);
            if (!hasLabelArcrole)
                return null;

            // Read labels.
            var linkNodes = document.Root.Children(Ns.Gen, "link");
            foreach (XElement linkNode in linkNodes)
            {
                var labels = linkNode.Children(Ns.Label, "label")
                    .Select(l => new XlinkNode(linkbase.Path, l))
                    .ToDictionary(l => l.Label);
                var locs = linkNode.Children(Ns.Link, "loc")
                    .Select(l => new XlinkNode(linkbase.Path, l))
                    .ToDictionary(l => l.Label);
                var labelArcs = linkNode.Children(Ns.Gen, "arc")
                    .Select(l => new XlinkNode(linkbase.Path, l))
                    .Where(n => n.ArcRole == LabelArcrole)
                    .ToList();
                foreach (XlinkNode arc in labelArcs)
                {
                    var locNode = locs[arc.From];
                    var labelNode = labels[arc.To];
                    Label label = new Label(labelNode.XNode);
                    _labels.Add(new KeyValuePair<Href, Label>(locNode.Href, label));
                }
            }

            return new List<TaxonomyComponent>();
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Assign all labels to the components, if found.
            foreach (var labelEncaps in _labels)
            {
                Href href = labelEncaps.Key;
                Label label = labelEncaps.Value;
                //string id = href.ResourceId;
                //var component = schemaSet.GetComponent(href) as ILabelized;
                //var schema = schemaSet.GetSchema(href.AbsolutePath);
                //var comp = schema?.GetComponent(href.ResourceId);
                if (!_linkbases.ContainsKey(href.AbsolutePath))
                {
                    _orphanLabels.Add(label);
                    continue;
                }
                var linkbase = _linkbases[href.AbsolutePath];
                var component = linkbase?.GetComponent(href.ResourceId) as ILabelized;
                if (component == null)
                {
                    _orphanLabels.Add(label);
                    continue;
                }
                if (component.Labels == null)
                    component.Labels = new XbrlLabels();
                component.Labels.Add(label);
            }

            return new List<TaxonomyComponent>();
        }
    }
}
