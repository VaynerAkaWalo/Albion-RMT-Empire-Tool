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
using System.Collections.Generic;

namespace Albion_RMT_Empire_Tool_Beta
{

    public partial class Transmutation : Form
    {
        private CustomXMLReader customReader;
        private CustomPictureLoader customPictureLoader;

        public Transmutation()
        {
            InitializeComponent();
            customReader = new CustomXMLReader();
            customPictureLoader = new CustomPictureLoader();
        }

        private static List<string> Transmutationlist = new List<string>();
        private static List<int> Transmutationquantity = new List<int>();

        private static List<string> Transmutationlistsimp = new List<string>();
        private static List<int> Transmutationquantitysimp = new List<int>();

        private static List<string> Tocraftlist = new List<string>();
        private static List<int> Tocraftquantity = new List<int>();


        private void Transmutation_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureClass.GetCultureInfo((CultureInfo)Thread.CurrentThread.CurrentCulture.Clone());

            comboBoxTier.SelectedIndex = 0;
            comboBoxEnchantment.SelectedIndex = 0;


            XmlReader xmlReader = customReader.GetItemsXml();
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "resource"))
                {
                    xmlReader.Read();
                    comboBoxResource.Items.Add(xmlReader.Value.Replace("_", " "));

                }
            }

            XmlReader xmlReader1 = customReader.GetDataXml();
            while (xmlReader1.Read())
            {
                if ((xmlReader1.NodeType == XmlNodeType.Element) && (xmlReader1.Name == "city"))
                {
                    xmlReader1.Read();
                    comboBoxCity.Items.Add(xmlReader1.Value.Replace("_", " "));

                }
            }
            comboBoxCity.SelectedIndex = 0;

        }

        private void resourcereload()
        {
            if (comboBoxResource.SelectedItem != null)
            {

                string tier = comboBoxTier.SelectedItem.ToString();
                string enchantment = "_" + comboBoxEnchantment.SelectedItem.ToString();
                string ending = comboBoxResource.SelectedItem.ToString().ToUpper().Replace(" ", "");
                string tierandenchantment = tier + enchantment.Replace("_","");

                XmlReader xmlReader1 = customReader.GetItemsXml();
                string resource = "nothing";
                string rawresource = "nothing";
                string city = "Any";
                while (xmlReader1.Read() && resource != comboBoxResource.SelectedItem.ToString())
                {
                    if ((xmlReader1.NodeType == XmlNodeType.Element) && (xmlReader1.Name == "resource"))
                    {
                        city = xmlReader1.GetAttribute("city");
                        rawresource = xmlReader1.GetAttribute("resource");
                        xmlReader1.Read();
                        resource = xmlReader1.Value.Replace("_", " ");

                    }
                }

                groupBoxresource.Tag = rawresource;

                XmlReader xmlReader = customReader.GetItemsXml();
                string name = "name";
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == comboBoxResource.SelectedItem.ToString().Replace(" ", "_") && xmlReader.GetAttribute("tier") == comboBoxTier.SelectedItem.ToString())
                    {
                        name = xmlReader.GetAttribute("name");
                    }

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == comboBoxResource.SelectedItem.ToString().Replace(" ", "_") && xmlReader.GetAttribute("tier") == "T3")
                    {
                        labelBuy1.Text = xmlReader.GetAttribute("name") + " T3";
                    }

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == rawresource && xmlReader.GetAttribute("tier") == "T4")
                    {
                        labelBuy2.Text = xmlReader.GetAttribute("name") + " T4.0";
                    }
                }


                labelRecCity.Text = city.Replace("_", "");
                groupBoxresource.Text = name + " " + comboBoxTier.SelectedItem.ToString() + comboBoxEnchantment.SelectedItem.ToString();
                
                if (comboBoxEnchantment.SelectedItem.ToString() != ".0")
                {
                    enchantment = enchantment.Replace(".", "LEVEL");
                }
                else
                {
                    enchantment = "";
                }

                try
                {
                    pictureBox.Image = customPictureLoader.GetResourceImageFromAlbionApi(tier, ending, enchantment);
                }
                catch
                {
                    pictureBox.Image = customPictureLoader.GetT1Trash();
                }
                ClearLists();
                Recipemain(comboBoxTier.SelectedItem.ToString());
            }
        }
            
        private void Recipemain(string tier)
        {
            if (comboBoxResource.SelectedItem != null)
            {
                XmlReader xmlReader = customReader.GetDataXml();
                int rawquantity = 0;
                string returnrate = "0,633";
                string returnratefocus = "0,461";

                if (tier != "T3")
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "resourcequantity" && xmlReader.GetAttribute("tier") == tier)
                        {
                            rawquantity = int.Parse(xmlReader.GetAttribute("quantity"));
                        }
                    }
                    string redtier = "T" + (int.Parse(tier.Replace("T","")) - 1).ToString();
                    string rawresource = "nothing";
                    string name = "nothing";
                    XmlReader xmlReader1 = customReader.GetItemsXml();
                    while (xmlReader1.Read())
                    {
                        if (xmlReader1.NodeType == XmlNodeType.Element && xmlReader1.Name == groupBoxresource.Tag.ToString() && xmlReader1.GetAttribute("tier") == tier)
                        {
                            rawresource = xmlReader1.GetAttribute("name");
                        }

                        if (xmlReader1.NodeType == XmlNodeType.Element && xmlReader1.Name == comboBoxResource.SelectedItem.ToString().Replace(" ", "_") && xmlReader1.GetAttribute("tier") == tier)
                        {
                            name = xmlReader1.GetAttribute("name");
                        }

                    }
                    if (tier == comboBoxTier.SelectedItem.ToString())
                    {
                        Tocraftlist.Add(groupBoxresource.Text);
                        Tocraftquantity.Add(int.Parse(textBoxQuantity.Text));
                        Transmutationlist.Add(rawresource + " " + tier + comboBoxEnchantment.SelectedItem.ToString());
                        Transmutationquantity.Add((int)Math.Ceiling(rawquantity * int.Parse(textBoxQuantity.Text) * float.Parse(returnrate)));
                    }
                    else
                    {
                        Tocraftlist.Add(name + " " + tier + comboBoxEnchantment.SelectedItem.ToString());
                        int i = Tocraftquantity.Count;
                        Tocraftquantity.Add((int)Math.Ceiling(Tocraftquantity[i - 1] * float.Parse(returnrate)));
                        Transmutationlist.Add(rawresource + " " + tier + comboBoxEnchantment.SelectedItem.ToString());
                        Transmutationquantity.Add((int)Math.Ceiling(rawquantity * Tocraftquantity[i] * float.Parse(returnrate)));
                    }
                    Recipemain(redtier);
                }
                else
                {
                    int i = Tocraftquantity.Count;
                    labelBuy1q.Text = Math.Ceiling(Tocraftquantity[i - 1] * float.Parse(returnrate)).ToString();

                    Transmutationlistsimp = new List<string>(Transmutationlist);
                    Transmutationquantitysimp = new List<int>(Transmutationquantity);

                    resorcedisassembly();
                    duplicatedestroyer();
                    sort();
                    Cost();
                    Display();
                    


                }
            }
        }

        private void resorcedisassembly()
        {
            string tier = "";
            int index = 0;
            int enchantmentlevel = 0;
            List<string> namespacesraw = new List<string>();
            List<string> namespaces = new List<string>();
            int n = Transmutationlist.Count;
            int m = Tocraftlist.Count;
            XmlReader xmlReader1 = customReader.GetItemsXml();
            while (xmlReader1.Read())
            {
                if (xmlReader1.NodeType == XmlNodeType.Element && xmlReader1.Name == groupBoxresource.Tag.ToString() && xmlReader1.GetAttribute("tier") != "T3")
                {
                    namespacesraw.Add(xmlReader1.GetAttribute("name"));
                }
                if (xmlReader1.NodeType == XmlNodeType.Element && xmlReader1.Name == comboBoxResource.SelectedItem.ToString().Replace(" ", "_") && xmlReader1.GetAttribute("tier") != "T3")
                {
                    namespaces.Add(xmlReader1.GetAttribute("name"));
                }
            }

            for (int i = 0; i < n; i++)
            {
                tier = Transmutationlist[i];
                index = tier.LastIndexOf('T');
                tier = tier.Remove(0, index + 1).Remove(1,2);
                enchantmentlevel = int.Parse(comboBoxEnchantment.SelectedItem.ToString().Replace(".", ""));
                if (enchantmentlevel == 0)
                {
                    for (int j = 0; j < (int.Parse(tier) - 4); j++)
                    {
                        Transmutationlist.Add(namespacesraw[j] + " T" + (j+4).ToString() + comboBoxEnchantment.SelectedItem.ToString());
                        Transmutationquantity.Add(Transmutationquantity[i]);
                    }
                }
                else
                {
                    for (int j = 0; j < (int.Parse(tier) - 3); j++)
                    {
                        for (int k = 0; k < enchantmentlevel; k++)
                        {
                            Transmutationlist.Add(namespacesraw[j] + " T" + (j + 4).ToString() + "." + k.ToString());
                            Transmutationquantity.Add(Transmutationquantity[i]);
                        }
                        if (j < (int.Parse(tier) - 4))
                        {
                            Transmutationlist.Add(namespacesraw[j] + " T" + (j + 4).ToString() + comboBoxEnchantment.SelectedItem.ToString());
                            Transmutationquantity.Add(Transmutationquantity[i]);
                        }
                    }
                    
                }
            }
        }

        private void sort()
        {
            int tempnum = 0;
            string tempname = "nothing";

            for (int i = 0; i < Transmutationquantity.Count; i++)
            {
                for (int j = 0; j < Transmutationquantity.Count; j++)
                {
                    if (i != j && Transmutationquantity[i] > Transmutationquantity[j])
                    {
                        tempname = Transmutationlist[i];
                        tempnum = Transmutationquantity[i];
                        Transmutationlist[i] = Transmutationlist[j];
                        Transmutationquantity[i] = Transmutationquantity[j];
                        Transmutationlist[j] = tempname;
                        Transmutationquantity[j] = tempnum;
                    }
                }
            }

            for (int i = 0; i < Transmutationquantitysimp.Count; i++)
            {
                for (int j = 0; j < Transmutationquantitysimp.Count; j++)
                {
                    if (i != j && Transmutationquantitysimp[i] > Transmutationquantitysimp[j])
                    {
                        tempname = Transmutationlistsimp[i];
                        tempnum = Transmutationquantitysimp[i];
                        Transmutationlistsimp[i] = Transmutationlistsimp[j];
                        Transmutationquantitysimp[i] = Transmutationquantitysimp[j];
                        Transmutationlistsimp[j] = tempname;
                        Transmutationquantitysimp[j] = tempnum;
                    }
                }
            }
        }

        private void duplicatedestroyer()
        {
            for (int i = 0; i < Transmutationlist.Count; i++)
            {
                for (int j = 0; j < Transmutationlist.Count; j++)
                {
                    if (i != j && Transmutationlist[i] == Transmutationlist[j])
                    {
                        Transmutationlist[j] = "nothing";
                        Transmutationquantity[i] += Transmutationquantity[j];
                        Transmutationquantity[j] = 0;
                    }
                }
            }
        }

        private void Display()
        {
            textBoxCraft.Clear();
            textBoxTransmutate.Clear();
            
            int n = Tocraftlist.Count;
            for (int i = 0; i < n; i++)
            {
                if (Tocraftquantity[i] != 0)
                {
                    textBoxCraft.Text += Tocraftlist[i] + ": " + Tocraftquantity[i] + Environment.NewLine; ;
                }
            }
            
            
            if (checkBoxTotrasfull.Checked == true)
            {
                int m = Transmutationlist.Count;
                for (int j = 0; j < m; j++)
                {
                    if (Transmutationquantity[j] != 0 && Transmutationlist[j] != labelBuy2.Text)
                    {
                        textBoxTransmutate.Text += Transmutationlist[j] + ": " + Transmutationquantity[j] + " " +  Environment.NewLine;
                    }

                }
            }
            else
            {
                int m = Transmutationlistsimp.Count;
                for (int j = 0; j < m; j++)
                {
                    if (Transmutationlistsimp[j] != labelBuy2.Text)
                    {
                        textBoxTransmutate.Text += Transmutationlistsimp[j] + ": " + Transmutationquantitysimp[j] + Environment.NewLine;
                    }
                }
            }
            int b = Transmutationlist.Count;
            for (int k = 0; k < b;k++)
            {
                if (Transmutationlist[k] == labelBuy2.Text)
                {
                    labelBuy2q.Text = Transmutationquantity[k].ToString();
                }
            }
         
        }

        private void ClearLists()
        {
            Tocraftlist.Clear();
            Tocraftquantity.Clear();
            Transmutationlist.Clear();
            Transmutationquantity.Clear();
        }

        private void Cost()
        {
            XmlReader xmlReader = customReader.GetDataXml();
            
            List<float> itemvalue0 = new List<float>();
            List<float> itemvalue1 = new List<float>();
            List<float> itemvalue2 = new List<float>();
            List<float> itemvalue3 = new List<float>();

            List<float> itemvalueraw0 = new List<float>();
            List<float> itemvalueraw1 = new List<float>();
            List<float> itemvalueraw2 = new List<float>();
            List<float> itemvalueraw3 = new List<float>();

            List<int> transmutationcost0 = new List<int>();
            List<int> transmutationcost1 = new List<int>();
            List<int> transmutationcost2 = new List<int>();
            List<int> transmutationcost3 = new List<int>();


            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "itemvalue")
                {
                    itemvalue0.Add(float.Parse(xmlReader.GetAttribute("value0")));
                    itemvalue1.Add(float.Parse(xmlReader.GetAttribute("value1")));
                    itemvalue2.Add(float.Parse(xmlReader.GetAttribute("value2")));
                    itemvalue3.Add(float.Parse(xmlReader.GetAttribute("value3")));
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "itemvalueraw")
                {
                    itemvalueraw0.Add(float.Parse(xmlReader.GetAttribute("value0")));
                    itemvalueraw1.Add(float.Parse(xmlReader.GetAttribute("value1")));
                    itemvalueraw2.Add(float.Parse(xmlReader.GetAttribute("value2")));
                    itemvalueraw3.Add(float.Parse(xmlReader.GetAttribute("value3")));
                } 

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "transmutation")
                {
                    transmutationcost0.Add(int.Parse(xmlReader.GetAttribute("value0")));
                    transmutationcost1.Add(int.Parse(xmlReader.GetAttribute("value1")));
                    transmutationcost2.Add(int.Parse(xmlReader.GetAttribute("value2")));
                    transmutationcost3.Add(int.Parse(xmlReader.GetAttribute("value3")));
                }
            }

            int transmutationcost = 0;
            int taxcost = 0;
            string tier = "0";
            string enchant = "0";
            int index = 0;

            textBoxTaxprice.Text = "0";
            textBoxTransmutationcost.Text = "0";

            for (int i = 0; i < Transmutationlist.Count;i++)
            {
                if (Transmutationlist[i] != "nothing")
                {
                    tier = Transmutationlist[i];
                    index = tier.LastIndexOf('T');
                    tier = tier.Remove(0, index + 1).Remove(1, 2);
                    
                    enchant = Transmutationlist[i];
                    enchant = enchant.Remove(0, index + 3);
                    
                    switch (enchant)
                    {
                        case "0":
                            taxcost = (int)Math.Ceiling(itemvalueraw0[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Transmutationquantity[i]);
                            transmutationcost = (int)Math.Floor((100 - float.Parse(textBoxGlobaldiscount.Text)) * transmutationcost0[int.Parse(tier) - 4] / 100 * Transmutationquantity[i]);
                            break;
                        case "1":
                            taxcost = (int)Math.Ceiling(itemvalueraw1[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Transmutationquantity[i]);
                            transmutationcost = (int)Math.Floor(((100 - float.Parse(textBoxGlobaldiscount.Text)) * transmutationcost1[int.Parse(tier) - 4] / 100) * Transmutationquantity[i]);
                            break;
                        case "2":
                            taxcost = (int)Math.Ceiling(itemvalueraw2[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Transmutationquantity[i]);
                            transmutationcost = (int)Math.Floor(((100 - float.Parse(textBoxGlobaldiscount.Text)) * transmutationcost2[int.Parse(tier) - 4] / 100) * Transmutationquantity[i]);
                            break;
                        case "3":
                            taxcost = (int)Math.Ceiling(itemvalueraw3[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Transmutationquantity[i]);
                            transmutationcost = (int)Math.Floor(((100 - float.Parse(textBoxGlobaldiscount.Text)) * transmutationcost3[int.Parse(tier) - 4] / 100) * Transmutationquantity[i]);
                            break;
                    }

                    if ((tier + enchant) != "40")
                    {
                        textBox3.Text = textBox3.Text + Math.Ceiling((100 - float.Parse(textBoxGlobaldiscount.Text)) * transmutationcost0[int.Parse(tier) - 4] / 100 * Transmutationquantity[i]).ToString() + " ";
                        textBoxTaxprice.Text = (int.Parse(textBoxTaxprice.Text) + taxcost).ToString();
                        textBoxTransmutationcost.Text = (int.Parse(textBoxTransmutationcost.Text) + transmutationcost).ToString();
                    }
                   
                }
                

            }
            for (int j = 0; j < Tocraftlist.Count; j++)
            {
                tier = Tocraftlist[j];
                index = tier.LastIndexOf('T');
                tier = tier.Remove(0, index + 1).Remove(1, 2);

                switch (comboBoxEnchantment.SelectedItem.ToString().Replace(".",""))
                {
                    case "0":
                        taxcost = (int)Math.Ceiling(itemvalue0[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Tocraftquantity[j]);
                        break;
                    case "1":
                        taxcost = (int)Math.Ceiling(itemvalue1[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Tocraftquantity[j]);
                        break;
                    case "2":
                        taxcost = (int)Math.Ceiling(itemvalue2[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Tocraftquantity[j]);
                        break;
                    case "3":
                        taxcost = (int)Math.Ceiling(itemvalue3[int.Parse(tier) - 4] / 20 * int.Parse(textBoxTax.Text) * Tocraftquantity[j]);
                        break;
                }
                textBoxTaxprice.Text = (int.Parse(textBoxTaxprice.Text) + taxcost).ToString();
            }
            textBoxTotalcost.Text = (int.Parse(textBoxTaxprice.Text) + int.Parse(textBoxTransmutationcost.Text)).ToString();
        }

        private void comboBoxResource_SelectedIndexChanged(object sender, EventArgs e)
        {
            resourcereload();
        }

        private void comboBoxTier_SelectedIndexChanged(object sender, EventArgs e)
        {
            resourcereload();
        }

        private void comboBoxEnchantment_SelectedIndexChanged(object sender, EventArgs e)
        {
            resourcereload();
        }

        private void checkBoxNofocus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNofocus.Checked == true)
            {
                checkBoxFullfocus.Checked = false;
                checkBoxLastfocus.Checked = false;
            }
        }

        private void checkBoxLastfocus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLastfocus.Checked == true)
            {
                checkBoxFullfocus.Checked = false;
                checkBoxNofocus.Checked = false;
            }
        }

        private void checkBoxFullfocus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFullfocus.Checked == true)
            {
                checkBoxNofocus.Checked = false;
                checkBoxLastfocus.Checked = false;
            }
        }

        private void textBoxQuantity_TextChanged(object sender, EventArgs e)
        {
            ClearLists();
            Recipemain(comboBoxTier.SelectedItem.ToString());
        }


        private void checkBoxTotrasfull_CheckedChanged(object sender, EventArgs e)
        {
            Display();
        }
    }

   

}
