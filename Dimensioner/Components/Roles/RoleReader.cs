using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Roles
{
    public class RoleReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<string, Role> _roles;
        private readonly ConcurrentDictionary<string, XbrlSchema> _schemas;

        public RoleReader()
        {
            _roles = new ConcurrentDictionary<string, Role>();
            _schemas = new ConcurrentDictionary<string, XbrlSchema>();
        }

        public override IEnumerable<TaxonomyComponent> Read(XbrlSchema schema, XDocument document)
        {
            if (_schemas.ContainsKey(schema.Path))
                throw new Exception("Schema already read...");
            _schemas[schema.Path] = schema;

            var roles = document.Root.Children(Ns.Xs, "annotation")
                .SelectMany(n => n.Children(Ns.Xs, "appinfo"))
                .SelectMany(n => n.Children(Ns.Link, "roleType"))
                .Select(n => ReadRole(schema, n))
                .ToList();
            return roles;
        }

        private Role ReadRole(XbrlSchema schema, XElement node)
        {
            string id = node.Attr("id");
            string uri = node.Attr("roleURI");

            Role role;
            lock (_roles)
            {
                if (_roles.ContainsKey(uri))
                {
                    // Retrieve the existing arcrole shell.
                    role = _roles[uri];
                    if (role.Name != null)
                    {
                        Console.WriteLine($"Old role = {role.Id} ({role.Name})");
                        Console.WriteLine($"- Schema {role.Schema.Path}");
                        Console.WriteLine($"New role = {uri} ({id})");
                        Console.WriteLine($"- Schema {schema.Path}");
                        throw new Exception("A role with this uri already exists");
                    }
                }
                else
                {
                    // Create and add the arcrole to the dictionary (as soon as possible since its async).
                    role = new Role(schema, uri);
                    _roles[uri] = role;
                }
            }

            // Fill in the remaining properties.
            role.Name = id;
            role.Schema = schema;
            role.Definition = node.GetChild(Ns.Link, "definition")?.Value;
            role.UsedOn = node.Children(Ns.Link, "usedOn").Select(n => n.Value).ToList();

            return role;
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            var roles = document.Root.Children(Ns.Link, "roleRef")
                .Select(n => ReadRoleRef(linkbase, n))
                .ToList();
            return roles;
        }

        private TaxonomyComponent ReadRoleRef(Linkbase linkbase, XElement node)
        {
            string uri = node.Attr("roleURI");
            var href = new Href(linkbase.Path, node.Attr(Ns.Xlink, "href"));
            string id = href.ResourceId;

            // Queue schema where role is defined.
            XbrlSchema schema = QueueSchema(href.AbsolutePath);

            Role role;
            lock (_roles)
            {
                if (_roles.ContainsKey(uri))
                {
                    // Retrieve the existing role shell.
                    role = _roles[uri];
                }
                else
                {
                    // Create and add the role to the dictionary.
                    role = new Role(schema, uri);
                    _roles[uri] = role;
                }
            }

            return role;
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Return the arcroles (can also be found in the linkbases via the schemaSet).
            return _roles.Select(a => a.Value);
        }
    }
}
