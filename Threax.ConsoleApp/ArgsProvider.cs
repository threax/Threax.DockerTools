﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.ConsoleApp
{
    public class ArgsProvider : IArgsProvider
    {
        public ArgsProvider(String[] args)
        {
            Args = args;
        }

        public string[] Args { get; }
    }
}
