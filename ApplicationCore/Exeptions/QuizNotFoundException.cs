﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Exeptions;

public class QuizNotFoundException : Exception
{
    public QuizNotFoundException(string? message) : base(message)
    {
    }
}
