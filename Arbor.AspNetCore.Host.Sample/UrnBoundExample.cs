using System.ComponentModel.DataAnnotations;
using Arbor.KVConfiguration.Core.Metadata;
using Arbor.KVConfiguration.Urns;

namespace Arbor.AspNetCore.Host.Sample
{
    [Urn(UrnKey)]
    public class UrnBoundExample
    {
        [Metadata(defaultValue: "test")]
        public const string DefaultInstanceName = UrnKey + ":default:name";

        public const string UrnKey = "urn:sample:bound:key";

        public UrnBoundExample(string name) => Name = name;

        [Required]
        public string Name { get; }

        public override string ToString() => Name;
    }
}