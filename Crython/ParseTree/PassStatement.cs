using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crython.ParseTree
{
	class PassStatement : Executable
	{
		public PassStatement(Token token) : base(token) { }

		public override IList<Executable> Resolve()
		{
			return EMPTY_LIST;
		}
	}
}
