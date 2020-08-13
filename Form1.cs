using System;
using System.IO;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Net;
using System.Linq;
//using System.Text;
using System.Xml;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using Albion_RMT_Empire_Tool_Beta;
using System.Collections.Generic;
using System.ComponentModel;

namespace Albion_RMT_Empire_Tool_v1
{
    public partial class Form1 : Form
    {

        private CustomXMLReader customReader;
        private CustomPictureLoader customPictureLoader;
        private ShoppingCart cart;

        public Form1()
        {
            InitializeComponent();
            customReader = new CustomXMLReader();
            customPictureLoader = new CustomPictureLoader();
            cart = new ShoppingCart();
        }


       

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureClass.GetCultureInfo((CultureInfo)Thread.CurrentThread.CurrentCulture.Clone());

            WebRequest webRequest = WebRequest.Create(Constants.URLStringItems);
            webRequest.Timeout = 1200; // miliseconds
            webRequest.Method = "HEAD";
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                response.Close();
            }
            catch
            {
                NoConnection();
            }

            comboBoxTier.SelectedIndex = 0;
            comboBoxEnchantment.SelectedIndex = 0;

            CheckBetaStatus();

            SaveXMLsToFiles();

            comboBoxCity.SelectedIndex = 0;
            ReturnRate();

            Marketcheck();
            Laborercheck();

            CartUpdate();

        }



        private void ComboBoxSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlReader xmlReader = customReader.GetItemsXml();
            comboBoxItem.Items.Clear();
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == comboBoxSubCategory.SelectedItem.ToString().Replace(" ", "_")))
                {
                    comboBoxItem.Items.Add(xmlReader.GetAttribute("name").Replace("_", " "));
                }
            }
            comboBoxItem.SelectedIndex = 0;

        }

        private void FillComboboxincart()
        {
            List<string> items = new List<string>(cart.OnlyItems());
            int n = items.Count;

            comboBoxCartItemSelection.Items.Clear();

            for(int i = 0; i < n; i++)
            {
                comboBoxCartItemSelection.Items.Add(items[i]);
            }

            comboBoxCartItemSelection.Text = "Select item to delete";
        }

        private void ItemReload()
        {
            XmlReader xmlReader = customReader.GetItemsXml();
            if (comboBoxItem.SelectedItem != null)
            {
                Object Tier = comboBoxTier.SelectedItem;
                Object Enchantment = comboBoxEnchantment.SelectedItem;
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.GetAttribute("name") == comboBoxItem.SelectedItem.ToString().Replace(" ", "_")))
                    {
                        string ending = (xmlReader.GetAttribute("image"));
                        string tier = Tier.ToString().Replace(" ", "_");
                        string enchantment = Enchantment.ToString().Replace(" ", "_");
                        string tierandenchantment = tier + enchantment;
                        labelItemName.Text = (xmlReader.GetAttribute("name").Replace("_", " ") + " " + tierandenchantment);
                        if (Enchantment.ToString() != ".0")
                        {
                            enchantment = enchantment.Replace(".", "@");
                        }
                        else
                        {
                            enchantment = "";
                        }

                        try
                        {
                            pictureBox.Image = customPictureLoader.GetItemImageFromAlbionApi(Tier.ToString(), ending, enchantment);
                        }
                        catch
                        {
                            pictureBox.Image = customPictureLoader.GetT1Trash();
                        }
                        XmlReader xmlReaderload = customReader.GetItemsXml();
                        string tree = string.Empty;
                        string resource1 = string.Empty;
                        string resource2 = string.Empty;
                        string station = string.Empty;
                        string stringtier = string.Empty;
                        while (xmlReaderload.Read() && xmlReaderload.Value != comboBoxSubCategory.SelectedItem.ToString().Replace(" ", "_"))
                        {
                            tree = xmlReaderload.GetAttribute("tree");
                            resource1 = xmlReaderload.GetAttribute("resource1");
                            resource2 = xmlReaderload.GetAttribute("resource2");
                        }
                        xmlReaderload = customReader.GetDataXml();
                        double cloth = 0;
                        double leather = 0;
                        double metal = 0;
                        double planks = 0;
                        while (xmlReaderload.Read() && xmlReaderload.Value != tree)
                        {
                            if (xmlReaderload.HasAttributes && xmlReaderload.GetAttribute("clothper") != null)
                            {
                                cloth = double.Parse(xmlReaderload.GetAttribute("clothper")) * 100;
                                leather = double.Parse(xmlReaderload.GetAttribute("leatherper")) * 100;
                                metal = double.Parse(xmlReaderload.GetAttribute("metalper")) * 100;
                                planks = double.Parse(xmlReaderload.GetAttribute("planksper")) * 100;
                            }
                        }
                        switch (tier)
                        {
                            case "T4":
                                stringtier = "Adept";
                                break;
                            case "T5":
                                stringtier = "Expert";
                                break;
                            case "T6":
                                stringtier = "Master";
                                break;
                            case "T7":
                                stringtier = "Grandmaster";
                                break;
                            case "T8":
                                stringtier = "Elder";
                                break;
                        }
                        labelRatio.Text = xmlReader.GetAttribute("ratio");
                        labelResource1.Text = resource1.Replace("_", " ") + " " + tierandenchantment;
                        if (resource2 != "Nothing")
                        {
                            labelResource2.Text = resource2.Replace("_", " ") + " " + tierandenchantment;
                        }
                        else
                        {
                            labelResource2.Text = "Rough Stone";
                        }
                        labelJournal.Text = stringtier + " " + tree + " Journal";
                        labelCloth.Text = "Cloth: " + cloth.ToString() + "%";
                        labelLeather.Text = "Lether: " + leather.ToString() + "%";
                        labelMetal.Text = "Metal Bar: " + metal.ToString() + "%";
                        labelPlanks.Text = "Planks: " + planks.ToString() + "%";
                        switch (tree)
                        {
                            case "Blacksmith":
                                station = "WARRIOR";
                                break;
                            case "Fletcher":
                                station = "HUNTER";
                                break;
                            case "Imbuer":
                                station = "MAGE";
                                break;
                            case "Thinker":
                                station = "TOOLMAKER";
                                break;
                        }

                        textBoxResource1cost.Enabled = true;
                        textBoxResource2cost.Enabled = true;
                        if (resource2 != "Nothing")
                        {
                            try
                            {
                                //pictureBoxResource1.Load(Constants.URLItemAPIString + Tier.ToString() + "_" + resource1.Replace("_", "").ToUpper());
                                pictureBoxResource1.Image = customPictureLoader.GetResourceImageFromAlbionApi(Tier.ToString(), resource1, enchantment);
                                //pictureBoxResource2.Load(Constants.URLItemAPIString + Tier.ToString() + "_" + resource2.Replace("_", "").ToUpper());
                                pictureBoxResource2.Image = customPictureLoader.GetResourceImageFromAlbionApi(Tier.ToString(), resource2, enchantment);

                                //pictureBoxJournal.Load(Constants.URLItemAPIString + tier + "_JOURNAL_" + station);
                                pictureBoxJournal.Image = customPictureLoader.GetJournalImageFromAlbionApi(tier, station);
                            }
                            catch
                            {
                                pictureBoxResource1.Image = customPictureLoader.GetRoughLogs();
                                pictureBoxResource2.Image = customPictureLoader.GetRoughStone();
                                pictureBoxJournal.Image = customPictureLoader.GetT2Journal();
                            }
                        }
                        else
                        {
                            try
                            {
                                pictureBoxResource1.Image = customPictureLoader.GetResourceImageFromAlbionApi(Tier.ToString(), resource1, enchantment);
                                //pictureBoxJournal.Load(Constants.URLItemAPIString + tier + "_JOURNAL_" + station);
                                pictureBoxJournal.Image = customPictureLoader.GetJournalImageFromAlbionApi(tier, station);

                            }
                            catch
                            {
                                pictureBoxResource1.Image = customPictureLoader.GetRoughLogs();
                                pictureBoxJournal.Image = customPictureLoader.GetT2Journal();
                            }
                            pictureBoxResource2.Image = customPictureLoader.GetRoughStone();
                            textBoxResource2cost.Enabled = false;
                        }

                        XmlReader xmlReadercity = customReader.GetItemsXml();
                        string city;
                        while (xmlReadercity.Read())
                        {
                            if (xmlReadercity.NodeType == XmlNodeType.Element)
                            {
                                city = xmlReadercity.GetAttribute("city");
                                xmlReadercity.Read();
                                if (xmlReadercity.Value == comboBoxSubCategory.SelectedItem.ToString().Replace(" ", "_"))
                                {
                                    labelRecCity.Text = city.Replace("_", " ");
                                }
                            }
                        }
                        PriceReload();
                        ReturnRate();
                        Calculation();

                    }
                }
            }

        }

        private void Calculation()
        {
            if (comboBoxItem.SelectedItem != null)
            {
                XmlReader xmlReader = customReader.GetDataXml();
                float modifier = 0;
                float itemvalue = 0;
                int journalcapacity = 0;
                float totalfame = 0;
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                nfi.NumberDecimalDigits = 0;
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ratio" && xmlReader.GetAttribute("type") == labelRatio.Text)
                    {
                        labelQuantityResource1.Text = (int.Parse(xmlReader.GetAttribute("quantity1")) * int.Parse(textBoxQuantity.Text)).ToString();
                        labelQuantityResource2.Text = (int.Parse(xmlReader.GetAttribute("quantity2")) * int.Parse(textBoxQuantity.Text)).ToString();
                    }
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "famemodifier" && xmlReader.GetAttribute("tier") == comboBoxTier.SelectedItem.ToString())
                    {
                        modifier = float.Parse(xmlReader.GetAttribute("modifier" + comboBoxEnchantment.SelectedItem.ToString().Replace(".", "")));
                    }
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "journalcapacity" && xmlReader.GetAttribute("tier") == comboBoxTier.SelectedItem.ToString())
                    {
                        journalcapacity = int.Parse(xmlReader.GetAttribute("capacity"));
                    }
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "itemvalue" && xmlReader.GetAttribute("tier") == comboBoxTier.SelectedItem.ToString())
                    {
                        itemvalue = float.Parse(xmlReader.GetAttribute("value" + comboBoxEnchantment.SelectedItem.ToString().Replace(".", ""))) * (int.Parse(labelQuantityResource1.Text) + int.Parse(labelQuantityResource2.Text));
                    }
                }

                double RRwithoutfocusresource1 = int.Parse(labelQuantityResource1.Text) * float.Parse(textBoxRRwithoutfocus.Text) / 100;
                double RRwithoutfocusresource2 = int.Parse(labelQuantityResource2.Text) * float.Parse(textBoxRRwithoutfocus.Text) / 100;
                double RRwithfocusresource1 = int.Parse(labelQuantityResource1.Text) * float.Parse(textBoxRRwithFocus.Text) / 100;
                double RRwithfocusresource2 = int.Parse(labelQuantityResource2.Text) * float.Parse(textBoxRRwithFocus.Text) / 100;

                if (RRwithoutfocusresource1 % 1 != 0)
                {
                    labelExpectedRRwithoutfocusresource1.Text = labelResource1.Text + " : " + Math.Floor(RRwithoutfocusresource1).ToString() + "-" + Math.Ceiling(RRwithoutfocusresource1).ToString();
                }
                else
                {
                    labelExpectedRRwithoutfocusresource1.Text = labelResource1.Text + " : " + RRwithoutfocusresource1.ToString();
                }

                if (RRwithoutfocusresource2 % 1 != 0)
                {
                    labelExpectedRRwithoutfocusresource2.Text = labelResource2.Text + " : " + Math.Floor(RRwithoutfocusresource2).ToString() + "-" + Math.Ceiling(RRwithoutfocusresource2).ToString();
                }
                else
                {
                    labelExpectedRRwithoutfocusresource2.Text = labelResource2.Text + " : " + RRwithoutfocusresource2.ToString();
                }

                if (RRwithfocusresource1 % 1 != 0)
                {
                    labelExpectedRRwithfocusresource1.Text = labelResource1.Text + " : " + Math.Floor(RRwithfocusresource1).ToString() + "-" + Math.Ceiling(RRwithfocusresource1).ToString();
                }
                else
                {
                    labelExpectedRRwithfocusresource1.Text = labelResource1.Text + " : " + RRwithfocusresource1.ToString();
                }

                if (RRwithfocusresource2 % 1 != 0)
                {
                    labelExpectedRRwithfocusresource2.Text = labelResource2.Text + " : " + Math.Floor(RRwithfocusresource2).ToString() + "-" + Math.Ceiling(RRwithfocusresource2).ToString();
                }
                else
                {
                    labelExpectedRRwithfocusresource2.Text = labelResource2.Text + " : " + RRwithfocusresource2.ToString();
                }

                totalfame = (int.Parse(labelQuantityResource1.Text) + int.Parse(labelQuantityResource2.Text)) * modifier;

                if (journalcapacity != 0)
                {
                    labelJournalsQuantity.Text = "Will be filled: " + (totalfame / journalcapacity).ToString();
                }


                if (checkBoxLowerTierHouse.Checked == true || checkBoxSameTierHouse.Checked == true)
                {
                    XmlReader xmlReaderload = customReader.GetItemsXml();
                    string tree = string.Empty;
                    string resource1 = string.Empty;
                    string resource2 = string.Empty;
                    while (xmlReaderload.Read() && xmlReaderload.Value != comboBoxSubCategory.SelectedItem.ToString().Replace(" ", "_"))
                    {
                        tree = xmlReaderload.GetAttribute("tree");
                        resource1 = xmlReaderload.GetAttribute("resource1");
                        resource2 = xmlReaderload.GetAttribute("resource2");
                    }

                    xmlReaderload = customReader.GetDataXml();
                    float cloth = 0;
                    float leather = 0;
                    float metal = 0;
                    float planks = 0;
                    double returnquantity = 0;
                    while (xmlReaderload.Read() && xmlReaderload.Value != tree)
                    {
                        if (xmlReaderload.HasAttributes && xmlReaderload.GetAttribute("clothper") != null)
                        {
                            cloth = float.Parse(xmlReaderload.GetAttribute("clothper"));
                            leather = float.Parse(xmlReaderload.GetAttribute("leatherper"));
                            metal = float.Parse(xmlReaderload.GetAttribute("metalper"));
                            planks = float.Parse(xmlReaderload.GetAttribute("planksper"));
                        }
                    }

                    xmlReaderload = customReader.GetDataXml();
                    while (xmlReaderload.Read())
                    {
                        if (xmlReaderload.HasAttributes && xmlReaderload.Name == "journalreturnlowertier" && xmlReaderload.GetAttribute("tier") == comboBoxTier.SelectedItem.ToString() && xmlReaderload.GetAttribute("return") != null && xmlReaderload.GetAttribute("return") != "" && checkBoxLowerTierHouse.Checked == true)
                        {
                            returnquantity = double.Parse(xmlReaderload.GetAttribute("return"));
                        }
                        if (xmlReaderload.HasAttributes && xmlReaderload.Name == "journalreturnsametier" && xmlReaderload.GetAttribute("tier") == comboBoxTier.SelectedItem.ToString() && xmlReaderload.GetAttribute("return") != null && xmlReaderload.GetAttribute("return") != "" && checkBoxSameTierHouse.Checked == true)
                        {
                            returnquantity = double.Parse(xmlReaderload.GetAttribute("return"));
                        }
                    }
                    if (returnquantity * (totalfame / journalcapacity) % 1 != 0)
                    {
                        labelLaborersReturnedQuantity.Text = "Returned Resource Quantity: " + Math.Floor(returnquantity * (totalfame / journalcapacity)).ToString() + "-" + Math.Ceiling(returnquantity * (totalfame / journalcapacity)).ToString();
                    }
                    else
                    {
                        labelLaborersReturnedQuantity.Text = "Returned Resource Quantity: " + (returnquantity * (totalfame / journalcapacity)).ToString();
                    }

                    if (returnquantity * cloth * (totalfame / journalcapacity) % 1 != 0 || returnquantity * cloth * (totalfame / journalcapacity) != 0)
                    {
                        labelLaborersClothQuantity.Text = "Cloth: " + Math.Floor(returnquantity * cloth * (totalfame / journalcapacity)) + "-" + Math.Ceiling(returnquantity * cloth * (totalfame / journalcapacity));
                    }
                    else
                    {
                        labelLaborersClothQuantity.Text = "Cloth: " + returnquantity * cloth * (totalfame / journalcapacity);
                    }

                    if (returnquantity * leather * (totalfame / journalcapacity) % 1 != 0 || returnquantity * leather * (totalfame / journalcapacity) != 0)
                    {
                        labelLaborersLeatherQuantity.Text = "Leather: " + Math.Floor(returnquantity * leather * (totalfame / journalcapacity)) + "-" + Math.Ceiling(returnquantity * leather * (totalfame / journalcapacity));
                    }
                    else
                    {
                        labelLaborersLeatherQuantity.Text = "Leather: " + returnquantity * leather * (totalfame / journalcapacity);
                    }

                    if (returnquantity * metal * (totalfame / journalcapacity) % 1 != 0 || returnquantity * metal * (totalfame / journalcapacity) != 0)
                    {
                        labelLaborersMetalQuantity.Text = "Metal Bar: " + Math.Floor(returnquantity * metal * (totalfame / journalcapacity)) + "-" + Math.Ceiling(returnquantity * metal * (totalfame / journalcapacity));
                    }
                    else
                    {
                        labelLaborersMetalQuantity.Text = "Metal Bar: " + returnquantity * metal * (totalfame / journalcapacity);
                    }

                    if (returnquantity * planks * (totalfame / journalcapacity) % 1 != 0 || returnquantity * planks * (totalfame / journalcapacity) != 0)
                    {
                        labelLaborersPlanksQuantity.Text = "Planks: " + Math.Floor(returnquantity * planks * (totalfame / journalcapacity)) + "-" + Math.Ceiling(returnquantity * planks * (totalfame / journalcapacity));
                    }
                    else
                    {
                        labelLaborersPlanksQuantity.Text = "Planks: " + returnquantity * planks * (totalfame / journalcapacity);
                    }
                    textBoxReturnFromJournalsSilver.Text = ((returnquantity * cloth * (totalfame / journalcapacity) * float.Parse(textBoxClothprice.Text)) + (returnquantity * leather * (totalfame / journalcapacity) * float.Parse(textBoxLeatherprice.Text)) + (returnquantity * metal * (totalfame / journalcapacity) * float.Parse(textBoxMetalprice.Text)) + (returnquantity * planks * (totalfame / journalcapacity) * float.Parse(textBoxPlanksprice.Text))).ToString("N", nfi);
                }

                textBoxResourcecost.Text = ((float.Parse(labelQuantityResource1.Text) * float.Parse(textBoxResource1cost.Text)) + (float.Parse(labelQuantityResource2.Text) * float.Parse(textBoxResource2cost.Text))).ToString("N", nfi);
                textBoxTax.Text = Math.Ceiling((itemvalue / 20 * int.Parse(textBoxUsagefee.Text))).ToString("N", nfi);
                textBoxWithoutfocusRRsilver.Text = (float.Parse(textBoxResourcecost.Text.Replace(",", "")) * float.Parse(textBoxRRwithoutfocus.Text) / 100).ToString("N", nfi);
                textBoxWithfocusRRsilver.Text = (float.Parse(textBoxResourcecost.Text.Replace(",", "")) * float.Parse(textBoxRRwithFocus.Text) / 100).ToString("N", nfi);

                if (checkBoxMarket.Checked == true)
                {
                    textBoxReturnFromJournalsSilver.Text = ((float.Parse(textBoxFullJournalPrice.Text) - float.Parse(textBoxEmptyJournalPrice.Text)) * (totalfame / journalcapacity)).ToString("N", nfi);
                }

                textBoxWithoutfocustotalcost.Text = (float.Parse(textBoxResourcecost.Text.Replace(",", "")) + float.Parse(textBoxTax.Text.Replace(",", "")) - float.Parse(textBoxWithoutfocusRRsilver.Text.Replace(",", "")) - float.Parse(textBoxReturnFromJournalsSilver.Text.Replace(",", ""))).ToString("N", nfi);
                textBoxWithfocustotalcost.Text = (float.Parse(textBoxResourcecost.Text.Replace(",", "")) + float.Parse(textBoxTax.Text.Replace(",", "")) - float.Parse(textBoxWithfocusRRsilver.Text.Replace(",", "")) - float.Parse(textBoxReturnFromJournalsSilver.Text.Replace(",", ""))).ToString("N", nfi);
                textBoxTotalCostOfItem.Text = (int.Parse(textBoxWithoutfocustotalcost.Text.Replace(",", "")) / int.Parse(textBoxQuantity.Text)).ToString("N", nfi);
                textBoxTotalCostOfItemFocus.Text = (int.Parse(textBoxWithfocustotalcost.Text.Replace(",", "")) / int.Parse(textBoxQuantity.Text)).ToString("N", nfi);
                textBoxProfit.Text = (int.Parse(textBoxItemSellingPrice.Text) - int.Parse(textBoxTotalCostOfItem.Text.Replace(",", ""))).ToString("N", nfi);
                textBoxProfitFocus.Text = (int.Parse(textBoxItemSellingPrice.Text) - int.Parse(textBoxTotalCostOfItemFocus.Text.Replace(",", ""))).ToString("N", nfi);
            }
        }

        private void ReturnRate()
        {
            if (labelRecCity.Text == comboBoxCity.SelectedItem.ToString())
            {
                textBoxRRwithoutfocus.Text = "24,8";
                textBoxRRwithFocus.Text = "47,9";
            }
            else
            {
                textBoxRRwithoutfocus.Text = "15,2";
                textBoxRRwithFocus.Text = "43,5";
            }
        }

        private void ComboBoxItemTierEnchantment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemReload();
        }

        private void CheckBoxMarket_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMarket.Checked == true)
            {
                checkBoxLowerTierHouse.Checked = false;
                checkBoxSameTierHouse.Checked = false;
            }
            Marketcheck();
            Laborercheck();
            PriceReloadLaborers();
            Calculation();
        }

        private void CheckBoxLowerTierHouse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLowerTierHouse.Checked == true)
            {
                checkBoxMarket.Checked = false;
                checkBoxSameTierHouse.Checked = false;
            }
            Marketcheck();
            Laborercheck();
            PriceReloadLaborers();
            Calculation();
        }

        private void CheckBoxSameTierHouse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSameTierHouse.Checked == true)
            {
                checkBoxLowerTierHouse.Checked = false;
                checkBoxMarket.Checked = false;
            }
            Marketcheck();
            Laborercheck();
            PriceReloadLaborers();
            Calculation();
        }

        private void Marketcheck()
        {
            if (checkBoxMarket.Checked == true)
            {
                groupBoxMarket.Enabled = true;
            }
            else
            {
                groupBoxMarket.Enabled = false;
            }
        }

        private void Laborercheck()
        {
            if (checkBoxLowerTierHouse.Checked == true || checkBoxSameTierHouse.Checked == true)
            {
                groupBoxLaborers.Enabled = true;
            }
            else
            {
                groupBoxLaborers.Enabled = false;
            }
        }

        private void ComboBoxCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReturnRate();
        }

        private void TextBoxQuantity_TextChanged(object sender, EventArgs e)
        {
            if (!textBoxQuantity.Text.All(char.IsDigit) || textBoxQuantity.Text == "" || textBoxQuantity.Text == "0")
            {
                textBoxQuantity.Text = "1";
            }
            Calculation();
        }

        private void TextBoxResource1cost_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxResource1cost);
            Calculation();
        }

        private void TextBoxResource2cost_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxResource2cost);
            Calculation();
        }

        private void TextBoxUsagefee_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxUsagefee);
            Calculation();
        }

        private void TextBoxRRFocusValue_TextChanged(object sender, EventArgs e)
        {
            Calculation();
        }
        private void TextBoxEmptyJournalPrice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxEmptyJournalPrice);
            Calculation();
        }

        private void TextBoxFullJournalPrice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxFullJournalPrice);
            Calculation();
        }

        private void TextBoxClothprice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxClothprice);
            Calculation();
        }

        private void TextBoxLeatherprice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxLeatherprice);
            Calculation();
        }

        private void TextBoxMetalprice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxMetalprice);
            Calculation();
        }

        private void TextBoxPlanksprice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxPlanksprice);
            Calculation();
        }

        private void CheckText(TextBox box)
        {
            if (!box.Text.All(char.IsDigit) || box.Text == "")
            {
                box.Text = "0";
            }
        }

        private void TextBoxItemSellingPrice_TextChanged(object sender, EventArgs e)
        {
            CheckText(textBoxItemSellingPrice);
            Calculation();
        }

        private void PriceReloadLaborers()
        {
            if (comboBoxItem.SelectedItem != null)
            {
                XmlReader xmlReader = XmlReader.Create("prices.xml");
                while (xmlReader.Read())
                {
                    if (checkBoxLowerTierHouse.Checked == true || checkBoxSameTierHouse.Checked == true)
                    {
                        if (xmlReader.HasAttributes && xmlReader.Name == ("Cloth_" + comboBoxTier.SelectedItem.ToString() + ".0"))
                        {
                            textBoxClothprice.Text = xmlReader.GetAttribute("price");
                        }

                        if (xmlReader.HasAttributes && xmlReader.Name == ("Leather_" + comboBoxTier.SelectedItem.ToString() + ".0"))
                        {
                            textBoxLeatherprice.Text = xmlReader.GetAttribute("price");
                        }

                        if (xmlReader.HasAttributes && xmlReader.Name == ("Metal_Bar_" + comboBoxTier.SelectedItem.ToString() + ".0"))
                        {
                            textBoxMetalprice.Text = xmlReader.GetAttribute("price");
                        }

                        if (xmlReader.HasAttributes && xmlReader.Name == ("Planks_" + comboBoxTier.SelectedItem.ToString() + ".0"))
                        {
                            textBoxPlanksprice.Text = xmlReader.GetAttribute("price");
                        }

                    }
                }
                xmlReader.Close();
            }
        }

        private void save(TextBox textBox, string name)
        {
            XmlReader xmlReader = XmlReader.Create("prices.xml");
            string price = "nothing";
            while (xmlReader.Read())
            {
                if (xmlReader.HasAttributes && xmlReader.Name == name)
                {
                    price = xmlReader.GetAttribute("price");
                }
            }
            xmlReader.Close();
            XmlDocument doc = new XmlDocument();
            using (XmlReader reader = XmlReader.Create("prices.xml"))
                doc.Load(reader);
            
            if (price != "nothing")
            {
                XmlElement itemprice = (XmlElement)doc.SelectSingleNode("/list/" + name);
                itemprice.SetAttribute("price", textBox.Text);
            }
            else
            {
                XmlElement itemprice = doc.CreateElement(name);
                doc.DocumentElement.AppendChild(itemprice);
                XmlAttribute attribute = doc.CreateAttribute("price");
                attribute.Value = textBox.Text;
                itemprice.Attributes.Append(attribute);
            }
            doc.Save("prices.xml");
        }

        private void PriceReload()
        {
            if (comboBoxItem.SelectedItem != null)
            {
                XmlReader xmlReader = XmlReader.Create("prices.xml");
                while (xmlReader.Read())
                {
                    if (xmlReader.HasAttributes && xmlReader.Name == labelItemName.Text.Replace(" ", "_"))
                    {
                        textBoxItemSellingPrice.Text = xmlReader.GetAttribute("price");
                    }

                    if (xmlReader.HasAttributes && xmlReader.Name == labelResource1.Text.Replace(" ", "_"))
                    {
                        textBoxResource1cost.Text = xmlReader.GetAttribute("price");
                    }

                    if (xmlReader.HasAttributes && xmlReader.Name == labelResource2.Text.Replace(" ", "_"))
                    {
                        textBoxResource2cost.Text = xmlReader.GetAttribute("price");
                    }

                    if (xmlReader.HasAttributes && xmlReader.Name == (labelJournal.Text.Replace(" ", "_") + "_Empty"))
                    {
                        textBoxEmptyJournalPrice.Text = xmlReader.GetAttribute("price");
                    }

                    if (xmlReader.HasAttributes && xmlReader.Name == (labelJournal.Text.Replace(" ", "_") + "_Full"))
                    {
                        textBoxFullJournalPrice.Text = xmlReader.GetAttribute("price");
                    }

                    PriceReloadLaborers();

                }
                xmlReader.Close();
            }
        }

       

        private void ButtonItemPrice_Click(object sender, EventArgs e)
        {
            string name = labelItemName.Text.Replace(" ", "_");
            save(textBoxItemSellingPrice, name);
        }

        private void ButtonResource2_Click(object sender, EventArgs e)
        {
            string name = labelResource2.Text.Replace(" ", "_");
            save(textBoxResource2cost, name);
        }

        private void ButtonEmptyJournal_Click(object sender, EventArgs e)
        {
            string name = labelJournal.Text.Replace(" ", "_") + "_Empty";
            save(textBoxEmptyJournalPrice, name);
        }

        private void ButtonFullJournal_Click(object sender, EventArgs e)
        {
            string name = labelJournal.Text.Replace(" ", "_") + "_Full";
            save(textBoxFullJournalPrice, name);
        }

        private void ButtonCloth_Click(object sender, EventArgs e)
        {
            string name = "Cloth_" + comboBoxTier.SelectedItem.ToString() + ".0";
            save(textBoxClothprice, name);
        }

        private void ButtonLeather_Click(object sender, EventArgs e)
        {
            string name = "Leather_" + comboBoxTier.SelectedItem.ToString() + ".0";
            save(textBoxClothprice, name);
        }

        private void ButtonMetal_Click(object sender, EventArgs e)
        {
            string name = "Metal_Bar_" + comboBoxTier.SelectedItem.ToString() + ".0";
            save(textBoxClothprice, name);
        }

        private void ButtonPlanks_Click(object sender, EventArgs e)
        {
            string name = "Planks_" + comboBoxTier.SelectedItem.ToString() + ".0";
            save(textBoxClothprice, name);
        }

        private void ButtonResource1_Click(object sender, EventArgs e)
        {
            string name = labelResource1.Text.Replace(" ", "_");
            save(textBoxResource1cost, name);
        }

        private void ButtonAddToCart_Click(object sender, EventArgs e)
        {
            string itemname = labelItemName.Text;
            string resource1 = labelResource1.Text;
            string resource2 = "nothing";
            if (labelResource2.Text != "Rough Stone")
            {
                resource2 = labelResource2.Text;
            }

            int itemq = int.Parse(textBoxQuantity.Text);
            int resource1q = int.Parse(labelQuantityResource1.Text);
            int resource2q = int.Parse(labelQuantityResource2.Text);

            int itemsellprice = int.Parse(textBoxItemSellingPrice.Text) * int.Parse(textBoxQuantity.Text);
            int resourcecost = int.Parse(textBoxWithoutfocustotalcost.Text.Replace(",", ""));
            int resourcecostfocus = int.Parse(textBoxWithfocustotalcost.Text.Replace(",", ""));

            cart.AddtoCart(itemname, resource1, resource2, itemq, resource1q, resource2q, itemsellprice, resourcecost, resourcecostfocus);
            CartUpdate();
        }

      

        private bool IsUrlExist(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                string HTMLSource = wc.DownloadString(url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void NoConnection()
        {
            string message = "No connection to server";
            string caption = "Connection error";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
            if (result == DialogResult.OK)
            {
                // Closes the parent form.
                Application.ExitThread();
            }
        }

        private void Betaoff()
        {
            string message = "Service offline";
            string caption = "Beta Version";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
            if (result == DialogResult.OK)
            {
                // Closes the parent form.
                Application.ExitThread();
            }
        }

        private void ComboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlReader xmlReader = customReader.GetItemsXml();
            Object Item = comboBoxCategory.SelectedItem;
            comboBoxSubCategory.Items.Clear();
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == Item.ToString()))
                {
                    xmlReader.Read();
                    comboBoxSubCategory.Items.Add(xmlReader.Value.Replace("_", " "));
                }
            }
        }

        private void buttonTransmutation_Click(object sender, EventArgs e)
        {
            Transmutation transmutation = new Transmutation();
            transmutation.Show();
        }

        private void CheckBetaStatus()
        {
            XmlReader betareader = XmlReader.Create(Constants.URLStringData);
            string beta = "off";
            while (betareader.Read())
            {
                if ((betareader.NodeType == XmlNodeType.Element) && (betareader.Name == "beta"))
                {
                    beta = betareader.GetAttribute("online");
                }
            }

            if (beta == "off")
            {
                Betaoff();
            }
        }

        private void SaveXMLsToFiles()
        {
            if (File.Exists("prices.xml") == false)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement element1 = doc.CreateElement(string.Empty, "list", string.Empty);
                doc.AppendChild(element1);
                doc.Save("prices.xml");
            }
            XmlReader xmlReader = XmlReader.Create(Constants.URLStringItems);
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "category"))
                {
                    xmlReader.Read();
                    comboBoxCategory.Items.Add(xmlReader.Value.Replace("_", " "));

                }
            }
            XmlReader xmlReader1 = XmlReader.Create(Constants.URLStringData);
            while (xmlReader1.Read())
            {
                if ((xmlReader1.NodeType == XmlNodeType.Element) && (xmlReader1.Name == "city"))
                {
                    xmlReader1.Read();
                    comboBoxCity.Items.Add(xmlReader1.Value.Replace("_", " "));

                }
            }
        }

        private void buttonClearCart_Click(object sender, EventArgs e)
        {
            cart.ClearCart();
            CartUpdate();
        }

        private void CartUpdate()
        {
            FillComboboxincart();
            textBoxCartResource.Text = cart.DisplayCartResource();
            textBoxCart.Text = cart.DisplayCart();
            textBoxMoney.Text = cart.DisplayMoney();
        }

        private void buttonDeleteSelectedItem_Click(object sender, EventArgs e)
        {
            if (comboBoxCartItemSelection.SelectedItem != null)
            {
                cart.ClearItem(comboBoxCartItemSelection.SelectedItem.ToString());
            }
            CartUpdate();
        }
    }
}
