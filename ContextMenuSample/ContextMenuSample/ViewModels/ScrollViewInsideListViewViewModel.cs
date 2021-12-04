using System;
using System.Collections.Generic;

namespace ContextMenuSample.ViewModels
{
    public class ScrollViewInsideListViewViewModel
    {
        public List<List<int>> Items { get; }

        public ScrollViewInsideListViewViewModel()
        {
            Items = new List<List<int>>();

            for (int i = 0; i < 500; i++)
            {
                var internalItems = new List<int>();

                for (int j = 0; j < 9; j++)
                {
                    internalItems.Add(j + 1);
                }

                Items.Add(internalItems);
            }
        }
    }
}
