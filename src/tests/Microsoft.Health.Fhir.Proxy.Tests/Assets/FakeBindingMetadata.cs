﻿using Microsoft.Azure.Functions.Worker;

namespace Microsoft.Health.Fhir.Proxy.Tests.Assets
{
    public class FakeBindingMetadata : BindingMetadata
    {
        public FakeBindingMetadata(string type, BindingDirection direction)
        {
            Type = type;
            Direction = direction;
        }

        public override string Type { get; }

        public override BindingDirection Direction { get; }
    }
}
