﻿using System;
using System.Collections.Generic;

namespace Digitalisert.Raven
{
    public static class ResourceModelExtensions
    {
        public static IEnumerable<dynamic> Properties(IEnumerable<dynamic> properties)
        {
            throw new NotSupportedException("This method is provided solely to allow query translation on the server");
        }
    }
}
