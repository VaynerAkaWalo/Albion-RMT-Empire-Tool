using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Albion_RMT_Empire_Tool_Beta
{
    class CustomPictureLoader
    {
        private Image roughLogs;
        private Image roughStone;
        private Image t2Journal;
        private Image t1Trash;
        public CustomPictureLoader()
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead("http://blamedevs.com/Rough_Logs.png"))
                {
                    roughLogs =  Image.FromStream(stream);
                }
                using (Stream stream = webClient.OpenRead("http://blamedevs.com/Rough_Stone.png"))
                {
                    roughStone = Image.FromStream(stream);
                }
                using (Stream stream = webClient.OpenRead("http://blamedevs.com/T2_JOURNAL_TROPHY_GENERAL.png"))
                {
                    t2Journal = Image.FromStream(stream);
                }
                using (Stream stream = webClient.OpenRead("http://blamedevs.com/T1_TRASH.png"))
                {
                    t1Trash = Image.FromStream(stream);
                }
            }
        }

        public Image GetItemImageFromAlbionApi(String tier, String ending, String enchantment)
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(Constants.URLItemAPIString + tier + ending + enchantment + ".png"))
                {
                    Image image = Image.FromStream(stream);
                    return image;
                }
            }
        }

        public Image GetResourceImageFromAlbionApi(String tier, String resource, String enchantment)
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                String url = Constants.URLItemAPIString + tier + "_" + resource.Replace("_", "").ToUpper();
                if (enchantment != "")
                    url += "_LEVEL" + enchantment.Replace("@", "");

                using (Stream stream = webClient.OpenRead(url))
                {
                    Image image = Image.FromStream(stream);
                    return image;
                }
            }
        }

        public Image GetJournalImageFromAlbionApi(String tier, String station)
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(Constants.URLItemAPIString + tier + "_JOURNAL_" + station))
                {
                    Image image = Image.FromStream(stream);
                    return image;
                }
            }
            
        }

        public Image GetRoughLogs()
        {
            return roughLogs;
        }
        public Image GetRoughStone()
        {
            return roughStone;
        }
        public Image GetT2Journal()
        {
            return t2Journal;
        }
        public Image GetT1Trash()
        {
            return t1Trash;
        }

    }
}
