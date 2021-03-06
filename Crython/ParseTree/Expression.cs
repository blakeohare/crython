﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crython.ParseTree
{
	internal abstract class Expression
	{
		public Token FirstToken { get; private set; }

		public Expression(Token firstToken)
		{
			this.FirstToken = firstToken;
		}

		public abstract Expression Resolve();
	}
}
