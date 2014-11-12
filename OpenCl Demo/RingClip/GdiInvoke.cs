using System;
using System.Drawing;

namespace RingClip
{
    public class GdiInvoke
    {
        private static LockBitmap _srcLock;
        private static LockBitmap _outLock;
        private static Bitmap _outputImg;

        public static void SetImageData(Bitmap srcImg, Bitmap outputImg)
        {
            _srcLock = new LockBitmap(srcImg);
            _outLock = new LockBitmap(outputImg);
            _outputImg = outputImg;
        }

        public static Image ComputeImage(
            ClPoint center,
            int radiusW,
            int radiusH,
            int fillW,
            int fillH,
            int count,
            float[] offsets,
            float[] offsetRadians,
            float accuracy)
        {
            _srcLock.LockBits();
            _outLock.LockBits();
            _outLock.Clear(Color.Black);

            int srcImgWidth = _srcLock.Width;
            int srcImgHeigh = _srcLock.Height;

            int outImgWidth = _outLock.Width;
            int outImgHeigh = _outLock.Height;


            int minRadiusW = radiusW - fillW;
            int minRadiusH = radiusH - fillH;

            bool needOpposite = count % 2 == 0;
            for (int index = 0; index < count; index++)
            {
                float startRadian = offsetRadians[index];
                float startArcLength = offsets[index];
                float endArcLength = offsets[index + 1];
                float currentArcLength = startArcLength;

                float x = (float) (radiusW*Math.Cos(startRadian));
                float y = (float) (radiusH*Math.Sin(startRadian));

                for (float currentRadian = startRadian;; currentRadian += accuracy)
                {
                    var cosRadian = (float) Math.Cos(currentRadian);
                    var sinRadian = (float) Math.Sin(currentRadian);

                    float nextX = radiusW*cosRadian;
                    float nextY = radiusH*sinRadian;

                    float distX = Math.Abs(nextX - x);
                    float distY = Math.Abs(nextY - y);

                    x = nextX;
                    y = nextY;

                    float dist = (float) Math.Sqrt(distX*distX + distY*distY);
                    currentArcLength += dist;

                    for (float currentX = 0.0f; currentX < fillW; currentX += 1.0f)
                    {
                        float innerX = (minRadiusW + currentX)*cosRadian;
                        float innerY = (minRadiusH + currentX)*sinRadian;

                        float currentY = currentArcLength - startArcLength;
                        int srcX = (int) ((currentX/fillW)*srcImgWidth);
                        int srcY = (int) ((currentY/fillH)*srcImgHeigh);

                        if ((srcX >= 0) && (srcX < srcImgWidth) && (srcY >= 0) && (srcY < srcImgHeigh))
                        {
                            int newX = (int) (innerX + center.X);
                            int newY = (int) (innerY + center.Y);

                            var color = _srcLock[srcX, srcY];

                            if ((newX >= 0) && (newX < outImgWidth) && (newY >= 0) && (newY < outImgHeigh))
                            {
                                _outLock[newX, newY] = color;
                            }

                            if (needOpposite)
                            {
                                int oppositeX = (int) (innerX - (innerX*2) + center.X);
                                int oppositeY = (int) (innerY - (innerY*2) + center.Y);
                                if ((oppositeX >= 0) && (oppositeX < outImgWidth) && (oppositeY >= 0) &&
                                    (oppositeY < outImgHeigh))
                                {
                                _outLock[oppositeX, oppositeY] = color;
                                }
                            }
                        }
                    }

                    if (currentArcLength > endArcLength)
                    {
                        break;
                    }

                }
            }




            _srcLock.UnlockBits();
            _outLock.UnlockBits();

            return _outputImg;
        }
    }
}