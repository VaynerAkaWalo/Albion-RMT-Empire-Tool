using Albion_RMT_Empire_Tool_v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Albion_RMT_Empire_Tool_Beta
{
    class ShoppingCart
    {
        private List<string> Item = new List<string>();
        private List<int> ItemQuantity = new List<int>();

        private List<string> Resource = new List<string>();
        private List<int> ResourceQuantity = new List<int>();


        public void AddtoCart(string itemname, string resource1, string resource2, int itemq, int resource1q, int resource2q)
        {
            int n = Item.Count;
            bool exist = false;
            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    if (Item[i] == itemname)
                    {
                        ItemQuantity[i] += itemq;
                        int resourceindex = 2 * i;
                        ResourceQuantity[resourceindex] = resource2q;
                        ResourceQuantity[resourceindex - 1] = resource1q;
                        exist = true;
                        break;
                    }
                }
            }
            if (exist == false)
            {
                Item.Add(itemname);
                ItemQuantity.Add(itemq);

                Resource.Add(resource1);
                Resource.Add(resource2);

                ResourceQuantity.Add(resource1q);
                ResourceQuantity.Add(resource2q);
            }
        }

        public void ClearItem()
        {

        }

        public void ClearCart()
        {
            Item.Clear();
            ItemQuantity.Clear();

            Resource.Clear();
            ResourceQuantity.Clear();
        }

        public string DisplayCart()
        {
            List<string> ResourceSorted = new List<string>(Resource);
            List<int> ResourceQuantitySorted = new List<int>(ResourceQuantity);

            int n = ResourceSorted.Count;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j && ResourceSorted[i] == ResourceSorted[j])
                    {
                        ResourceSorted[j] = "nothing";
                        ResourceQuantitySorted[i] += ResourceQuantitySorted[j];
                        ResourceQuantitySorted[j] = 0;
                    }
                }
            }

            string tempname = "nothing";
            int tempquantity = 0;

            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    if (k != l && ResourceQuantitySorted[k] < ResourceQuantitySorted[l])
                    {
                        tempname = ResourceSorted[k];
                        tempquantity = ResourceQuantitySorted[k];

                        ResourceSorted[k] = ResourceSorted[l];
                        ResourceQuantitySorted[k] = ResourceQuantitySorted[l];

                        ResourceSorted[l] = tempname;
                        ResourceQuantitySorted[l] = tempquantity;
                    }
                }
            }

            string cart = "";

            for (int o = 0; o < n; o++)
            {
                if (ResourceSorted[o] != "nothing")
                {
                    cart += ResourceSorted[o] + ": " + ResourceQuantitySorted[o] + "\n";
                }
            }

            return cart;
        }
    }
}
