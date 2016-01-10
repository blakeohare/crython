using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crython.ParseTree
{
	internal class ContinueStatement : Executable
	{
		public ContinueStatement(Token token) : base(token) { }

		public override IList<Executable> Resolve() { return Listify(this); }
	}
}
