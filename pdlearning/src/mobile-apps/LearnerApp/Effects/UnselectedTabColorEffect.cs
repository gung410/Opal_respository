using Xamarin.Forms;

namespace LearnerApp.Effects
{
    public class UnselectedTabColorEffect : RoutingEffect
    {
        public UnselectedTabColorEffect()
            : base($"LearnerApp.{nameof(UnselectedTabColorEffect)}")
        {
        }
    }
}
