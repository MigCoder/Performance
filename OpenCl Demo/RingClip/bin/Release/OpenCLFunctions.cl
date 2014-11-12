const sampler_t smp = CLK_NORMALIZED_COORDS_FALSE | //Natural coordinates
                      CLK_ADDRESS_NONE | //Clamp to zeros
                      CLK_FILTER_NEAREST; //Don't interpolate

constant float FloatMin = -3.40282347E+38;
constant float FloatMax = 3.40282347E+38;

void GetPoints(
    float startRadian,
    int radiusW, 
    int radiusH,
    int fillW, 
    int fillH, 
    float centerX,
    float centerY,
    float startArcLength,
    float endArcLength,
    float accuracy,
    read_only image2d_t srcImg,
    write_only image2d_t outputImg,
    bool needOpposite)
{
    int srcImgWidth = get_image_width(srcImg);
    int srcImgHeigh = get_image_height(srcImg);

    int outImgWidth = get_image_width(outputImg);
    int outImgHeigh = get_image_height(outputImg);

    int minRadiusW = radiusW - fillW;
    int minRadiusH = radiusH - fillH;

    float currentArcLength = startArcLength;

    float x = radiusW * cos(startRadian);
    float y = radiusH * sin(startRadian);

    for (float currentRadian = startRadian; ; currentRadian += accuracy)
    {
        float cosRadian = cos(currentRadian);
        float sinRadian = sin(currentRadian);

        float nextX = radiusW * cosRadian;
        float nextY = radiusH * sinRadian;

        float distX = fabs(nextX - x);
        float distY = fabs(nextY - y);

        x = nextX;
        y = nextY;

        float dist = sqrt(distX * distX + distY * distY);
        currentArcLength += dist;

        for (float currentX = 0.0f; currentX < fillW; currentX += 1.0f)
        {
            float innerX = (minRadiusW + currentX) * cosRadian;
            float innerY = (minRadiusH + currentX) * sinRadian;

            float currentY = currentArcLength - startArcLength;
            int srcX = (int)((currentX / fillW) * srcImgWidth);
            int srcY = (int)((currentY / fillH) * srcImgHeigh);

            if ((srcX >= 0) && (srcX < srcImgWidth) && (srcY >= 0) && (srcY < srcImgHeigh))
            {
                int newX = (int)innerX + centerX;
                int newY = (int)innerY + centerY;

                int2 srcCoords = (int2)(srcX, srcY);
                uint4 color = read_imageui(srcImg, smp, srcCoords);

                if ((newX >= 0) && (newX < outImgWidth) && (newY >= 0) && (newY < outImgHeigh))
                {
                    int2 outCoords = (int2)(newX, newY);
                    write_imageui(outputImg, outCoords, color);
                }

                if(needOpposite)
                {
                    int oppositeX = (int)innerX - (innerX * 2) + centerX;
                    int oppositeY = (int)innerY - (innerY * 2) + centerY;
                    if ((oppositeX >= 0) && (oppositeX < outImgWidth) && (oppositeY >= 0) && (oppositeY < outImgHeigh))
                    {
                        int2 outCoords = (int2)(oppositeX, oppositeY);
                        write_imageui(outputImg, outCoords, color);
                    }
                }
            }
        }

        if(currentArcLength > endArcLength)
        {
            return;
        }

    }
}

kernel void ComputeImage(
    float centerX,
    float centerY,
    int radiusW, 
    int radiusH,
    int fillW,
    int fillH,
    int count,
    read_only image2d_t srcImg,
    write_only image2d_t outputImg,
    global float* offsets,
    global float* offsetRadians,
    float accuracy)
{
    int index = get_global_id(0);

    float startRadian = offsetRadians[index];
    float startArcLength = offsets[index];
    float endArcLength = offsets[index + 1];

    bool needOpposite = count % 2 == 0;

    GetPoints(startRadian, radiusW, radiusH, fillW, fillH, centerX, centerY, startArcLength, endArcLength, accuracy, srcImg, outputImg, needOpposite);
}