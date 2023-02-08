using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RaceTo21
{
    public class ConnectIdAndImgName
    {
        Dictionary<int, string> Lists = new Dictionary<int, string>();

        //Pass a known path and an array of lists as parameters
        public void GetPNGFile()
        {
            int index = 0;
            DirectoryInfo theFolder = new DirectoryInfo("/Users/lihongyi/Desktop/neu/2023Winter/Week4/6308C#/RaceTo21_W4_HongyiLi/RaceTo21_W3/card_assets");

            FileInfo[] fileInfos = theFolder.GetFiles(); //Get an array of all files in the current folder

            foreach (FileInfo NextFile in fileInfos)  //traverse files
            {
                index++;
                if (NextFile.Extension == ".png") //get the format you want
                {
                    //Add key-value pairs
                    Lists.Add(index, NextFile.Name);
                }
            }
        }
    }
}
