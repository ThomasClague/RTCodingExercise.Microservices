using Bogus;
using Catalog.Domain;
using System.Linq;

namespace Catalog.UnitTests.Fakers
{
    public class PlateFaker : Faker<Plate>
    {
        public readonly int Seed = 123456;
        public PlateFaker()
        {
            UseSeed(Seed);
            RuleFor(p => p.Id, f => f.Random.Guid());

            RuleFor(p => p.Registration, f => f.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));
        }
    }
}