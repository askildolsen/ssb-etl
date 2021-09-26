using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Indexes;
using static Digitalisert.Dataplattform.ResourceModel;
using static Digitalisert.Dataplattform.ResourceModelExtensions;

namespace ssb_etl
{
    public class SSBResourceModel {
        public class SSB {
            public IEnumerable<Dictionary<string, string>> Data { get; set; }
        }

        public class SSBResourceIndex : AbstractMultiMapIndexCreationTask<Resource>
        {
            public SSBResourceIndex()
            {
                AddMap<SSB>(statistikker =>
                    from statistikk in statistikker
                    let metadata = MetadataFor(statistikk)
                    where metadata.Value<string>("@id") == "SSB/1108"
                    from data in statistikk.Data
                    select new Resource
                    {
                        ResourceId = data["region"].Substring(0, 4),
                        Type = new[] { "Kommune" },
                        SubType = new string [] { data["kvartal"] },
                        Title = new[] { data["region"].Substring(5) },
                        Code =  new[] { data["region"].Substring(0, 4) },
                        Status = new string[] { },
                        Tags = new string[] { },
                        Properties = new[] {
                            new Property { Name = data["statistikkvariabel"], Value = new[] { data["01222: Befolkning og kvartalsvise endringar, etter region, kvartal og statistikkvariabel"] } }
                        },
                        Source = new[] { metadata.Value<string>("@id") }
                    }
                );

                AddMap<SSB>(statistikker =>
                    from statistikk in statistikker
                    let metadata = MetadataFor(statistikk)
                    where metadata.Value<string>("@id") == "SSB/26975"
                    from data in statistikk.Data
                    select new Resource
                    {
                        ResourceId = data["region"].Substring(2, 4),
                        Type = new string[] { "Kommune" },
                        SubType = new string [] { },
                        Title = new[] { data["region"].Substring(7) },
                        Code =  new[] { data["region"].Substring(2, 4) },
                        Status = new string[] { },
                        Tags = new string[] { },
                        Properties = new[] {
                            new Property {
                                Name = data["statistikkvariabel"],
                                Value = new[] { data["07459: Befolkning, etter region, år og statistikkvariabel"] },
                                Tags = new[] { "@history" },
                                Thru = new DateTime(int.Parse(data["år"]), 12, 31)
                            }
                        },
                        Source = new[] { metadata.Value<string>("@id") }
                    }
                );

                Reduce = results =>
                    from result in results
                    group result by result.ResourceId into g
                    select new Resource
                    {
                        ResourceId = g.Key,
                        Type = g.SelectMany(r => r.Type).Distinct(),
                        SubType = g.SelectMany(r => r.SubType).Distinct(),
                        Title = g.SelectMany(r => r.Title).Distinct(),
                        Code = g.SelectMany(r => r.Code).Distinct(),
                        Status = g.SelectMany(r => r.Status).Distinct(),
                        Tags = g.SelectMany(r => r.Tags).Distinct(),
                        Properties = (IEnumerable<Property>)Properties(g.SelectMany(r => r.Properties)),
                        Source = g.SelectMany(resource => resource.Source).Distinct()
                    };

                Index(Raven.Client.Constants.Documents.Indexing.Fields.AllFields, FieldIndexing.No);

                OutputReduceToCollection = "SSBResource";

                AdditionalAssemblies = new HashSet<AdditionalAssembly> {
                    AdditionalAssembly.FromPath("Digitalisert.Dataplattform.ResourceModel.dll", new HashSet<string> { "Digitalisert.Dataplattform" })
                };
            }
        }
    }
}
