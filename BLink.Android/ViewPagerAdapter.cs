using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang;

namespace BLink.Droid
{
    public class ViewPagerAdapter : FragmentPagerAdapter
    {
        private List<Fragment> _fragments = new List<Fragment>();
        private List<string> _fragmentTitles = new List<string>();

        public ViewPagerAdapter(FragmentManager manager) : base(manager)
        {
        }

        public override int Count
        {
            get
            {
                return _fragments.Count;
            }
        }

        public override Fragment GetItem(int postion)
        {
            return _fragments[postion];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(_fragmentTitles[position].ToLower());// display the title
            //return null;// display only the icon
        }

        public void AddFragment(Fragment fragment, string title)
        {
            _fragments.Add(fragment);
            _fragmentTitles.Add(title);
        }
    }
}