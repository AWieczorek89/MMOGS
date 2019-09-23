using System.Collections.Generic;
using System.Drawing;

namespace ArgbConverter.DataModels
{
    public class AreaBoundings
    {
        public int AreaId { get; private set; } = -1;
        public Point BoundsX { get; set; } = new Point(0, 0);
        public Point BoundsY { get; set; } = new Point(0, 0);

        public AreaBoundings(int areaId)
        {
            this.AreaId = areaId;
        }

        public AreaBoundings Copy()
        {
            AreaBoundings newInstance = new AreaBoundings(this.AreaId);
            newInstance.BoundsX = new Point(this.BoundsX.X, this.BoundsX.Y);
            newInstance.BoundsY = new Point(this.BoundsY.X, this.BoundsY.Y);
            return newInstance;
        }

        /// <summary>
        /// Metoda rozszerzająca ograniczenia strefy, jeżeli zostanie przekazany punkt wykraczający poza nią
        /// </summary>
        public void ExtendAreaBoundings(List<Point> pointList, int maxX, int maxY)
        {
            bool minXChanged = false;
            bool maxXChanged = false;
            bool minYChanged = false;
            bool maxYChanged = false;
            int tempMinX = 0;
            int tempMaxX = 0;
            int tempMinY = 0;
            int tempMaxY = 0;

            foreach (Point point in pointList)
            {
                minXChanged = false;
                maxXChanged = false;
                minYChanged = false;
                maxYChanged = false;
                tempMinX = 0;
                tempMaxX = 0;
                tempMinY = 0;
                tempMaxY = 0;
                
                //SPRAWDZANIE
                if (point.X < this.BoundsX.X)
                {
                    tempMinX = point.X;
                    minXChanged = true;
                }

                if (point.X > this.BoundsX.Y)
                {
                    tempMaxX = point.X;
                    maxXChanged = true;
                }

                if (point.Y < this.BoundsY.X)
                {
                    tempMinY = point.Y;
                    minYChanged = true;
                }
                 
                if (point.Y > this.BoundsY.Y)
                {
                    tempMaxY = point.Y;
                    maxYChanged = true;
                }
                    
                //ZAWĘŻANIE ZAKRESU
                if (tempMinX < 0)
                    tempMinX = 0;

                if (tempMinY < 0)
                    tempMinY = 0;

                if (tempMaxX > maxX)
                    tempMaxX = maxX;

                if (tempMaxY > maxY)
                    tempMaxY = maxY;

                if (minXChanged || maxXChanged || minYChanged || maxYChanged)
                {
                    this.BoundsX = new Point
                    (
                        (minXChanged ? tempMinX : this.BoundsX.X),
                        (maxXChanged ? tempMaxX : this.BoundsX.Y)
                    );

                    this.BoundsY = new Point
                    (
                        (minYChanged ? tempMinY : this.BoundsY.X),
                        (maxYChanged ? tempMaxY : this.BoundsY.Y)
                    );
                }
            }
        }
    }
}
