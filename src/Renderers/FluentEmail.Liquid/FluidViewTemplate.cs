using FluentEmail.Liquid.Tags;

using Fluid;

namespace FluentEmail.Liquid
{
    public class FluidViewTemplate : BaseFluidTemplate<FluidViewTemplate>
    {
        static FluidViewTemplate()
        {
            Factory.RegisterTag<LayoutTag>("layout");
            Factory.RegisterTag<RenderBodyTag>("renderbody");
            Factory.RegisterBlock<RegisterSectionBlock>("section");
            Factory.RegisterTag<RenderSectionTag>("rendersection");
        }
    }
}
