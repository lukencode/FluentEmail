using System.Dynamic;

namespace FluentEmail.Razor
{
	public interface IViewBagModel
	{
		ExpandoObject ViewBag { get; }
	}
}