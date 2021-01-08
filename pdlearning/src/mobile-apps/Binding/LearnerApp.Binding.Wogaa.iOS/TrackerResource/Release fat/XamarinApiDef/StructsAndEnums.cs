using ObjCRuntime;

namespace Binding
{
	[Native]
	public enum Environment : long
	{
		Development = 0,
		Staging = 1,
		Production = 2
	}
}
