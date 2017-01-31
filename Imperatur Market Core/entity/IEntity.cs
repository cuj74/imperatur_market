﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.entity
{
    public interface IEntity
    {
        string EntityIdentifier { get; }
        IEntity SubEntity { get; }
    }
}
