using Albion_RMT_Empire_Tool_v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
                        ResourceQuantity[resourceindex] += resource1q;
                        ResourceQuantity[resourceindex + 1] += resource2q;
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

        public void ClearItem(string name)
        {
            int n = Item.Count;
            if (n > 0)
            {
                if (Checkifempty() == true)
                {
                    ClearCart();
                }
                else
                {
                    int index = Item.FindIndex(x => x == name);
                    int resourceindex = index * 2;

                    Item[index] = "nothing";
                    ItemQuantity[index] = 0;

                    Resource[resourceindex] = "nothing";
                    Resource[resourceindex + 1] = "nothing";

                    ResourceQuantity[resourceindex] = 0;
                    ResourceQuantity[resourceindex + 1] = 0;

                }
            }
        }

        private bool Checkifempty()
        {
            return !Item.Exists(x => x != "nothing");
        }

        public void ClearCart()
        {
            Item.Clear();
            ItemQuantity.Clear();

            Resource.Clear();
            ResourceQuantity.Clear();
        }

        public string DisplayCartResource()
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
                    if (k != l && ResourceQuantitySorted[k] > ResourceQuantitySorted[l])
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
                    cart += ResourceSorted[o] + ": " + ResourceQuantitySorted[o] + "\r\n";
                }
            }

            return cart;
        }

        public string DisplayCart()
        {
            List<string> ItemSorted = new List<string>(Item);
            List<int> ItemQuantitySorted = new List<int>(ItemQuantity);

            int n = ItemSorted.Count;
            string cart = "";

            string tempname = "nothing";
            int tempquantity = 0;

            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    if (k != l && ItemQuantitySorted[k] > ItemQuantitySorted[l])
                    {
                        tempname = ItemSorted[k];
                        tempquantity = ItemQuantitySorted[k];

                        ItemSorted[k] = ItemSorted[l];
                        ItemQuantitySorted[k] = ItemQuantitySorted[l];

                        ItemSorted[l] = tempname;
                        ItemQuantitySorted[l] = tempquantity;
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (ItemSorted[i] != "nothing")
                {
                    cart += ItemSorted[i] + ": " + ItemQuantitySorted[i] + "\r\n";
                }
            }

            return cart;
        }


        public List<string> OnlyItems()
        {
            List<string> list = new List<string>(Item);
            list.RemoveAll(x => x == "nothing");

            return list;
        }

        
    }
}
