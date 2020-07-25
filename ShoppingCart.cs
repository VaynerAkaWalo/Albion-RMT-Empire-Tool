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

        private List<int> ItemSellPrice = new List<int>();
        private List<int> ResourceCost = new List<int>();
        private List<int> ResourceCostFocus = new List<int>();

        public void AddtoCart(string itemname, string resource1, string resource2, int itemq, int resource1q, int resource2q, int sellprice, int resourcecost, int resourcecostfocus)
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

                        ItemSellPrice[i] += sellprice;
                        ResourceCost[i] += resourcecost;
                        ResourceCostFocus[i] += resourcecostfocus;

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

                ItemSellPrice.Add(sellprice);
                ResourceCost.Add(resourcecost);
                ResourceCostFocus.Add(resourcecostfocus);
            }
        }

        public void ClearItem(string name)
        {
            int n = Item.Count;
            if (n > 0)
            {
                int index = Item.FindIndex(x => x == name);
                int resourceindex = index * 2;

                Item[index] = "nothing";
                ItemQuantity[index] = 0;

                Resource[resourceindex] = "nothing";
                Resource[resourceindex + 1] = "nothing";

                ResourceQuantity[resourceindex] = 0;
                ResourceQuantity[resourceindex + 1] = 0;

                ItemSellPrice[index] = 0;
                ResourceCost[index] = 0;
                ResourceCostFocus[index] = 0;

                if (Checkifempty() == true)
                {
                    ClearCart();
                }
            }
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

            Sort(ResourceSorted, ResourceQuantitySorted);

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

            Sort(ItemSorted, ItemQuantitySorted);

            for (int i = 0; i < n; i++)
            {
                if (ItemSorted[i] != "nothing")
                {
                    cart += ItemSorted[i] + ": " + ItemQuantitySorted[i] + "\r\n";
                }
            }

            return cart;
        }

        public string DisplayMoney()
        {
            string money = "";

            int totalcost = Counting(ResourceCost);
            int totalcostfocus = Counting(ResourceCostFocus);
            int sellprice = Counting(ItemSellPrice);

            int profit = sellprice - totalcost;
            int profitfocus = sellprice - totalcostfocus;

            money = "Total sell price: " + sellprice.ToString() + "\r\n" + "\r\n";

            money += "Total cost without focus: " + totalcost.ToString() + "\r\n";
            money += "Total profit without focus: " + profit.ToString() + "\r\n" + "\r\n";

            money += "Total cost with focus: " + totalcostfocus.ToString() + "\r\n";
            money += "Total profit with focus: " + profitfocus.ToString() + "\r\n" + "\r\n";
            return money;
        }

        public List<string> OnlyItems()
        {
            List<string> list = new List<string>(Item);
            list.RemoveAll(x => x == "nothing");

            return list;
        }

        private bool Checkifempty()
        {
            return !Item.Exists(x => x != "nothing");
        }

        private void Sort(List<string> stringlist, List<int> intlist)
        {
            int n = stringlist.Count;

            string tempname = "nothing";
            int tempquantity = 0;

            for (int k = 0; k < n; k++)
            {
                for (int l = 0; l < n; l++)
                {
                    if (k != l && intlist[k] > intlist[l])
                    {
                        tempname = stringlist[k];
                        tempquantity = intlist[k];

                        stringlist[k] = stringlist[l];
                        intlist[k] = intlist[l];

                        stringlist[l] = tempname;
                        intlist[l] = tempquantity;
                    }
                }
            }
        }

        private int Counting(List<int> list)
        {
            int total = 0;

            foreach (int i in list)
            {
                total += i;
            }

            return total;
        }
    }
}
