using MMOGS.Measurement.Units;
using MMOGS.Models;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMOGS.Measurement
{
    public static class GeoDataValidator
    {
        private static readonly double _marginZ = 0.5;

        public static Task<BoxedData> ValidateMovementTaskStart(List<GeoDataElement> geoDataList, CharacterMovementDetails movementDetails)
        {
            var t = new Task<BoxedData>(() => ValidateMovement(geoDataList, movementDetails));
            t.Start();
            return t;
        }

        public static Point3<double> GetRandomExitPosition(List<GeoDataElement> geoDataList)
        {
            if (geoDataList.Count == 0)
                return null;
            
            int randomIndex = (geoDataList.Count > 1 ? Measure.GetRandomNumber(0, geoDataList.Count - 1) : 0);
            GeoDataElement geoElement = geoDataList[randomIndex];

            int collisionMinX, collisionMaxX, collisionMinY, collisionMaxY, collisionMinZ, collisionMaxZ;
            GetColliderPosition(geoElement, out collisionMinX, out collisionMaxX, out collisionMinY, out collisionMaxY, out collisionMinZ, out collisionMaxZ);

            return new Point3<double>
            (
                Measure.Lerp(Convert.ToDouble(collisionMinX), Convert.ToDouble(collisionMaxX), 0.5) + 0.5,
                Measure.Lerp(Convert.ToDouble(collisionMinY), Convert.ToDouble(collisionMaxY), 0.5) + 0.5,
                Convert.ToDouble(collisionMaxZ) + 1
            );
        }

        public static BoxedData ValidateMovement(List<GeoDataElement> geoDataList, CharacterMovementDetails movementDetails)
        {
            BoxedData data = new BoxedData();
            GeoDataValidationDetails details = new GeoDataValidationDetails();
            string msg = "";
            
            try
            {
                #region Sub arrays
                //SUB ARRAYS
                GeoDataElement[] terrainSubArray = new GeoDataElement[geoDataList.Count];
                GeoDataElement[] obstacleSubArray = new GeoDataElement[geoDataList.Count];
                int terrainFilledSize = 0;
                int obstacleFilledSize = 0;

                for (int i = 0; i < geoDataList.Count; i++)
                {
                    switch (geoDataList[i].ElementType)
                    {
                        case GeoDataElement.Type.Terrain:
                            terrainSubArray[terrainFilledSize] = geoDataList[i];
                            terrainFilledSize++;
                            break;
                        case GeoDataElement.Type.Obstacle:
                            obstacleSubArray[obstacleFilledSize] = geoDataList[i];
                            obstacleFilledSize++;
                            break;
                        case GeoDataElement.Type.Platform:
                            terrainSubArray[terrainFilledSize] = geoDataList[i];
                            obstacleSubArray[obstacleFilledSize] = geoDataList[i];
                            terrainFilledSize++;
                            obstacleFilledSize++;
                            break;
                    }
                }

                #endregion

                //PARAMETERS

                bool valid = true;
                Point3<double> lastValidMovementPoint = movementDetails.OldLocationLocal.Copy();
                double tStep = Measure.GetTParamStep(movementDetails.OldLocationLocal, movementDetails.NewLocationLocal)
                    / 2;
                
                double tParam = 0;
                double tParamClamped = 0;
                bool tParamReached = false;

                //VALIDATION

                if (tStep > 0 && tStep < 1)
                {
                    double currentXDbl, currentYDbl, currentZDbl;
                    int currentX, currentY, currentZ;
                    int collisionMinX, collisionMaxX, collisionMinY, collisionMaxY, collisionMinZ, collisionMaxZ;

                    GeoDataElement geoDataElement;
                    bool obstacleDetected = false;
                    bool hasGroundUnderFeet;

                    while (!tParamReached)
                    {
                        tParamClamped = Measure.Clamp(0.0000, 1.0000, tParam);
                        currentXDbl = Measure.Lerp(movementDetails.OldLocationLocal.X, movementDetails.NewLocationLocal.X, tParamClamped);
                        currentYDbl = Measure.Lerp(movementDetails.OldLocationLocal.Y, movementDetails.NewLocationLocal.Y, tParamClamped);
                        currentZDbl = Measure.Lerp(movementDetails.OldLocationLocal.Z, movementDetails.NewLocationLocal.Z, tParamClamped) 
                            + _marginZ;

                        currentX = Convert.ToInt32(Math.Floor(currentXDbl));
                        currentY = Convert.ToInt32(Math.Floor(currentYDbl));
                        currentZ = Convert.ToInt32(Math.Floor(currentZDbl));

                        #region Obstacle check
                        //OBSTACLE CHECK
                        for (int i = 0; i < obstacleFilledSize; i++)
                        {
                            geoDataElement = obstacleSubArray[i];
                            GetColliderPosition(geoDataElement, out collisionMinX, out collisionMaxX, out collisionMinY, out collisionMaxY, out collisionMinZ, out collisionMaxZ);
                            
                            if
                            (
                                currentX >= collisionMinX && currentX <= collisionMaxX &&
                                currentY >= collisionMinY && currentY <= collisionMaxY &&
                                currentZ >= collisionMinZ && currentZ <= collisionMaxZ
                            )
                            {
                                obstacleDetected = true;
                                break;
                            }
                        }

                        #endregion

                        #region Ground check
                        //GROUND CHECK
                        hasGroundUnderFeet = false;

                        if (!obstacleDetected) //NOTE: ground check is not necessary if player collides with obstacle (performance)
                        {
                            for (int i = 0; i < terrainFilledSize; i++)
                            {
                                geoDataElement = terrainSubArray[i];
                                GetColliderPosition(geoDataElement, out collisionMinX, out collisionMaxX, out collisionMinY, out collisionMaxY, out collisionMinZ, out collisionMaxZ);

                                if 
                                (
                                    currentX >= collisionMinX && currentX <= collisionMaxX &&
                                    currentY >= collisionMinY && currentY <= collisionMaxY &&
                                    currentZ >= collisionMinZ - 1 && currentZ <= collisionMaxZ + 2
                                )
                                {
                                    hasGroundUnderFeet = true;
                                    break;
                                }
                            }
                        }

                        #endregion

                        if (obstacleDetected || !hasGroundUnderFeet)
                        {
                            valid = false;
                            break;
                        }
                        else
                        {
                            lastValidMovementPoint.X = currentXDbl;
                            lastValidMovementPoint.Y = currentYDbl;
                            lastValidMovementPoint.Z = currentZDbl - _marginZ;
                        }

                        //Console.WriteLine($"tClamped [{tParamClamped}] pos [{currentX}; {currentY}; {currentZ}] obstacle [{obstacleDetected}]");

                        if (tParam >= 1)
                            tParamReached = true;

                        tParam += tStep;
                    }
                }
                

                details.Valid = valid;
                details.LastValidMovementPoint = lastValidMovementPoint;
                details.LastTParamValue = tParamClamped;
            }
            catch (Exception exception)
            {
                msg = $"Error occured while geo data movement validation for character's ID [{movementDetails.CharId}]: {exception.Message}";
            }

            data.Data = details;
            data.Msg = msg;
            return data;
        }

        public static void GetColliderPosition
        (
            GeoDataElement geoDataElement, 
            out int collisionMinX, 
            out int collisionMaxX, 
            out int collisionMinY, 
            out int collisionMaxY, 
            out int collisionMinZ, 
            out int collisionMaxZ
        )
        {
            collisionMinX = geoDataElement.Location.X;
            collisionMinY = geoDataElement.Location.Y;
            collisionMinZ = geoDataElement.Location.Z;
            collisionMaxX = geoDataElement.Location.X + (geoDataElement.ColliderSize.X > 0 ? (geoDataElement.ColliderSize.X - 1) : 0);
            collisionMaxY = geoDataElement.Location.Y + (geoDataElement.ColliderSize.Y > 0 ? (geoDataElement.ColliderSize.Y - 1) : 0);
            collisionMaxZ = geoDataElement.Location.Z + (geoDataElement.ColliderSize.Z > 0 ? (geoDataElement.ColliderSize.Z - 1) : 0);
        }
    }
}
