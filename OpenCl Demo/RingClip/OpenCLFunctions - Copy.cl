const sampler_t smp = CLK_NORMALIZED_COORDS_FALSE | //Natural coordinates
                      CLK_ADDRESS_NONE | //Clamp to zeros
                      CLK_FILTER_NEAREST; //Don't interpolate

constant float FloatMin = -3.40282347E+38;
constant float FloatMax = 3.40282347E+38;

typedef struct
{
    int2 Coords;
    uint4 Color;
} Pixel;

typedef struct 
{
    float X;
    float Y;
} Point;

typedef struct 
{
    float X;
    float Y;
    float Magnitude;
} Vector;

typedef struct
{
    float X;
    float Y;
    float Width;
    float Height;
} RectAngle;

float CrossProduct(Vector v1, Vector v2)
{
    return (v1.X * v2.Y) - (v1.Y * v2.X);
}

void Division(Vector* vector, float c)
{
    vector->X /= c;
    vector->Y /= c;
}

bool IsCCW(Point pt1, Point pt2, Point pt3)
{
    Vector BA, BC;
    BA.X = pt1.X - pt2.X;
    BA.Y = pt1.Y - pt2.Y;

    BC.X = pt3.X - pt2.X;
    BC.Y = pt3.Y - pt2.Y;
    return CrossProduct(BA, BC) > 0;
}

bool IsOnBorder(Point pt, Point* points)
{
    if (IsCCW(pt, points[0], points[1]))
    {
                   // printf("p1X :%f p1Y :%f\n", points[3].X,points[3].Y);
        if (IsCCW(pt, points[1], points[2]))
        {
            if (IsCCW(pt, points[2], points[3]))
            {
                if (IsCCW(pt, points[3], points[0]))
                {
                    return true;   
                }
            }
        }
    }
    return false;
}

void Swap(int* a, int* b)
{
    int t = *a;
    *a = *b;
    *b = t;
}

void GetLinePoints(
    int x0, 
    int y0, 
    int x1, 
    int y1,
    int orginalY, 
    float centerX,
    float centerY,
    read_only image2d_t srcImg,
    write_only image2d_t outputImg)
{
    bool steep = abs((y1 - y0)) > abs((x1 - x0));
    if (steep) { Swap(&x0, &y0); Swap(&x1, &y1); }
    if (x0 > x1) { Swap(&x0, &x1); Swap(&y0, &y1); }
    int dX = abs((x1 - x0)), dY = abs((y1 - y0)), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

    int realX = 0;
    int count = x1 - x0;
    for (int x = x0; x <= x1; ++x)
    {
        int2 coords;
        if (steep)
        {
            coords.x = y + centerX;
            coords.y = x + centerY;
        }
        else
        {
            coords.x = x + centerX;
            coords.y = y + centerY;
        }

        int2 srcCoords = (int2)(realX++, orginalY);
        uint4 color = read_imageui(srcImg, smp, srcCoords);

        write_imageui(outputImg, coords, color);

        err = err - dY;
        if (err < 0) { y += ystep; err += dX; }
    }
}

void GetPoints(
    float angle, 
    float stepAngle, 
    int radiusW, 
    int radiusH,
    int fillW, 
    int fillH, 
    float centerX,
    float centerY,
    read_only image2d_t srcImg,
    write_only image2d_t outputImg)
{
    int minRadiusW = radiusW - fillW;
    int minRadiusH = radiusH - fillH;

    float scalH = get_image_height(srcImg) / fillH;

    Point lastOutPoint, lastInnerPoint;

    float currentRadian = angle / 180.0f * M_PI;
    float stepRadian = stepAngle / 180.0f * M_PI;

    // Get radian of 45°.
    float scaleRadian = atan(1.0f);
    float equalRadian = sin(scaleRadian);
    float y = equalRadian * radiusW;
    float x = equalRadian * radiusW;
    float accuracy = atan2(y + 1.0f, x) - scaleRadian;
    
    //printf("%f\n", accuracy);


    for (int h = 0; h < fillH; h++)
    {
        for (float i = 0; i < stepRadian; i+=accuracy)
        {
            currentRadian += accuracy;

            float cosRadian = cos(currentRadian);
            float sinRadian = sin(currentRadian);
            //printf("angle: %f cos: %f sin: %f\n", angle,cosRadian,sinRadian);

            int outX = (int)(radiusW * cosRadian);
            int outY = (int)(radiusH * sinRadian);
            int innerX = (int)(minRadiusW * cosRadian);
            int innerY = (int)(minRadiusH * sinRadian);

            Point outPoint,innerPoint;
            outPoint.X = outX;
            outPoint.Y = outY;
            innerPoint.X = innerX;
            innerPoint.Y = innerY;

            int orginalY = (int)round(scalH * h);
            //printf("outX: %d outY: %d innerX: %d innerY: %d\n", innerX, innerY, outX, outY);
            GetLinePoints(innerX, innerY, outX, outY, orginalY, centerX, centerY, srcImg, outputImg);
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
    float spileAngle,
    read_only image2d_t srcImg,
    write_only image2d_t outputImg)
{
    int index = get_global_id(0);

    float currentAngle = index * spileAngle;
    float betweenAngle = (fillH * 180) / (float)(radiusW) / M_PI;
    float stepAngle = betweenAngle / fillH;

    GetPoints(currentAngle, stepAngle, radiusW, radiusH, fillW, fillH, centerX, centerY, srcImg, outputImg);
}
}