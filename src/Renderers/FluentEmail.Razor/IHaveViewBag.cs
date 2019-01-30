using System.Dynamic;

namespace FluentEmail.Razor
{
	public interface IHaveViewBag
	{
		ExpandoObject ViewBag { get; }
	}
}