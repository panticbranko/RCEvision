using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RCEvision
{
    class Scanning
    {
        static int i = 1;
        static string screenRoot = "c:/RemedyAlarm/Screenshot.bmp";
        public string result;
        public void screenShot()
        {
            i++;
            result = screenRoot.Insert(25, i.ToString());
            //Create a new bitmap.
          using(  var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                // Create a graphics object from the bitmap.
                using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
                {
                    // Take the screenshot from the upper left corner to the right bottom corner.
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                                Screen.PrimaryScreen.Bounds.Y,
                                                0,
                                                0,
                                                Screen.PrimaryScreen.Bounds.Size,
                                                CopyPixelOperation.SourceCopy);
                }
                    
                    // Save the screenshot to the specified path that the user has chosen.
                    bmpScreenshot.Save(result, ImageFormat.Bmp);
                
            }
        }

        public string getResult() { return result; }
        // serachSubPicture
        public static List<Point> GetPositionsOfSubPictures(Bitmap mainPicture, Bitmap subPicture)
        {
            List<Point> subPositions = new List<Point>();
            int mainPictureWidith = mainPicture.Width;
            int mainPictureHeight = mainPicture.Height;

            int subPictureWidith = subPicture.Width;
            int subPictureHeight = subPicture.Height;

            System.Drawing.Color mainPixel, subPixel;

            for (int y = 0; y < mainPictureHeight; y++)
            {
                for (int x = 0; x < mainPictureWidith; x++)
                {
                    mainPixel = mainPicture.GetPixel(x, y);
                    subPixel = subPicture.GetPixel(0, 0);
                    if (mainPixel == subPixel)
                    {
                        bool indeticalFrameFlag = CheckFrame(mainPicture, subPicture, x, y, 10);
                        if (indeticalFrameFlag)
                        {
                            bool foundSubPicture = checkMainPicture(mainPicture, subPicture, x, y, subPictureWidith, subPictureHeight);
                            if (foundSubPicture)
                            {
                                subPositions.Add(new Point(x, y));
                                x = x + subPictureWidith;
                            }
                        }
                    }
                }
            }
            return subPositions;
        }

        public static bool IsMatchFound(Bitmap mainPicture, Bitmap subPicture)
        {
            int mainPictureWidith = mainPicture.Width;
            int mainPictureHeight = mainPicture.Height;

            int subPictureWidith = subPicture.Width;
            int subPictureHeight = subPicture.Height;

            System.Drawing.Color mainPixel, subPixel;

            for (int y = 0; y < mainPictureHeight; y++)
            {
                for (int x = 0; x < mainPictureWidith; x++)
                {
                    mainPixel = mainPicture.GetPixel(x, y);
                    subPixel = subPicture.GetPixel(0, 0);
                    if (mainPixel == subPixel)
                    {
                        bool indeticalFrameFlag = CheckFrame(mainPicture, subPicture, x, y, 10);
                        if (indeticalFrameFlag)
                        {
                            bool foundSubPicture = checkMainPicture(mainPicture, subPicture, x, y, subPictureWidith, subPictureHeight);
                            if (foundSubPicture)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool CheckFrame(Bitmap main, Bitmap sub, int x, int y, int offset)
        {
            Bitmap testMain = main;
            Bitmap testSub = sub;
            System.Drawing.Color mainPixel, subPixel;

            for (int z = x; z < x + offset; z++)
            {
                if (x < main.Width - offset)
                {

                    mainPixel = testMain.GetPixel(z, y);
                    subPixel = testSub.GetPixel(z - x, 0);
                    bool test = CheckChanells(mainPixel, subPixel, 10); //test is value of chanles in range of +/- int
                    if (test)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool checkMainPicture(Bitmap main, Bitmap sub, int x, int y, int subW, int subH) // subW - subPicture Weight, subH subPicture Hight
        {

            Bitmap testMain = main;
            Bitmap testSub = sub;
            System.Drawing.Color mainPixel, subPixel;

            for (int z = y; z < y + subH; z++)
            {
                for (int d = x; d < x + subW; d++)
                {
                    if (d < main.Width - subW)
                    {
                        mainPixel = testMain.GetPixel(d, z);
                        subPixel = testSub.GetPixel(d - x, z - y);
                        if (mainPixel == subPixel)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool CheckChanells(System.Drawing.Color mainPixel, System.Drawing.Color subPixel, int chanellDeviation)
        {
            int testR, testG, testB, subDeviation;
            bool isRCorrect, isGCorrect, isBCorrect;
            testR = mainPixel.R - subPixel.R;
            testG = mainPixel.G - subPixel.G;
            testB = mainPixel.B - subPixel.B;
            subDeviation = 0 - chanellDeviation;
            if (testR < chanellDeviation && testR > subDeviation)
            {
                isRCorrect = true;
            }
            else
            {
                return false;
            }
            if (testG < chanellDeviation && testG > subDeviation)
            {
                isGCorrect = true;
            }
            else
            {
                return false;
            }
            if (testB < chanellDeviation && testB > subDeviation)
            {
                isBCorrect = true;
            }
            else
            {
                return false;
            }
            if (isRCorrect && isGCorrect && isBCorrect)
            {
                return true;
            }
            return false;
        }

    }
}
