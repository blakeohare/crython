using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crython.ParseTree
{
	internal class BooleanConstant : Expression
	{
		public bool Value { get; private set; }

		public BooleanConstant(Token token, bool value)
			: base(token)
		{
			this.Value = value;
		}

		public override Expression Resolve()
		{
			return this;
		}
	}
}
