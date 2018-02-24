using Android.Content;
using Android.Views;
using Android.Support.V4.View;
using Android.Support.Design.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

using TabbedPageBottom.Render;
using TabbedPageBottom.Droid.Renderer;

[assembly: ExportRendererAttribute(handler: typeof(TabbedPageOnBottom), target: typeof(TabbedPageBottomRenderer))]
namespace TabbedPageBottom.Droid.Renderer
{
    public class TabbedPageBottomRenderer : TabbedPageRenderer
    {
        private static TabLayout tabLayout = null;
        private static ViewPager viewPager = null;

        public TabbedPageBottomRenderer(Context context) : base(context) { }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (changed)
            {
                ViewGroup.ScaleY = -1;

                for (int i = 0; i < ChildCount; ++i)
                {
                    Android.Views.View view = (Android.Views.View)GetChildAt(i);
                    if (view is TabLayout) tabLayout = (TabLayout)view;
                    else if (view is ViewPager) viewPager = (ViewPager)view;
                }

                tabLayout.ScaleY = -1;
                viewPager.ScaleY = -1;
                viewPager.SetPadding(0, -tabLayout.MeasuredHeight, 0, 0);
            }
        }
    }
}